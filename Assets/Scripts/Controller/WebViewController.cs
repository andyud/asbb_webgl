using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WebViewController : MonoBehaviour
{
	public static WebViewController instance;
	public WebViewObject webView = null;
	[HideInInspector]
	public WebViewObject rankingWebView = null;
	[HideInInspector]
	public WebViewObject noticeWebView = null;
	[HideInInspector]
	public List<Notice_Response_Item> noticeList;

	public string urlString { get; set; }

	public string CallbackMessage { get; set; }

	private void Awake()
	{
		instance = this;
		urlString = string.Empty;
	}

	private void LateUpdate()
	{
		if (!string.IsNullOrEmpty(urlString))
		{
			SetWebView(urlString);
			urlString = string.Empty;
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(rankingWebView != null)
			{
				if (rankingWebView.CanGoBack())
					rankingWebView.GoBack();
				else
					CallbackMessage = "close";
			}

			if(noticeWebView != null)
			{
				if (noticeWebView.CanGoBack())
					noticeWebView.GoBack();
				else
					CallbackMessage = "close";
			}
		}

		if(!string.IsNullOrEmpty(CallbackMessage))
		{
			Debug.Log($"=====CallbackMessage: {CallbackMessage}");

			string todayCloseValue;
			switch (CallbackMessage)
			{
				case "close":
					if(rankingWebView != null)
					{
						rankingWebView.SetVisibility(false);
						ClearRankingWebView();
					}
					
					if(noticeWebView != null)
					{
						TouchInputRecognizer.SetPause(false);
						PopUpController.ActiveAlphaScreen(false);
						noticeWebView.SetVisibility(false);
						Debug.Log($"=====before noticeLength: {noticeList.Count}");

						noticeList.RemoveAt(0);
						Debug.Log($"=====after noticeLength: {noticeList.Count}");
						ClearNoticeWebView();
						ShowNoticeWebView();
					}
					
					break;
				case "clicklogin":
					if(rankingWebView != null)
						rankingWebView.SetVisibility(false);
					ClearRankingWebView();
                    PopUpController.Open_Login();
					break;
				case "today_close":
					todayCloseValue = PlayerPrefs.GetString(PlayerPrefs_Config.TodayClose, string.Empty);
					if (string.IsNullOrEmpty(todayCloseValue))
						todayCloseValue += noticeList[0].id.ToString();
					else
						todayCloseValue += string.Format($"_{noticeList[0].id.ToString()}");

					PlayerPrefs.SetString(PlayerPrefs_Config.TodayClose, todayCloseValue);
					PlayerPrefs.SetString(PlayerPrefs_Config.TodayCloseTime, DateTime.UtcNow.ToString());

					Debug.Log($"=====TodayClose: {todayCloseValue}");
					Debug.Log($"=====TodayCloseTime: {DateTime.UtcNow}");
					goto case "close";
					//break;
				case "today_open":
					todayCloseValue = PlayerPrefs.GetString(PlayerPrefs_Config.TodayClose, string.Empty);
					if (string.IsNullOrEmpty(todayCloseValue))
						break;
					else
					{
						todayCloseValue = todayCloseValue.Replace(string.Format($"{noticeList[0].id.ToString()}"), string.Empty);
						todayCloseValue = todayCloseValue.Replace(string.Format($"_{noticeList[0].id.ToString()}"), string.Empty);
					}

					PlayerPrefs.SetString(PlayerPrefs_Config.TodayClose, todayCloseValue);
					Debug.Log($"=====TodayClose: {todayCloseValue}");
					break;
				case "open_url":
					Application.OpenURL(noticeList[0].link);
					break;
			}

			CallbackMessage = string.Empty;
		}
	}

	public void ShowNoticeWebView()
	{
		if (noticeList == null)
			return;

		if (noticeList.Count <= 0)
		{
			Debug.Log("noticeUrlList is zero");
			noticeList = null;
			return;
		}

		var item = noticeList[0];
		ClearNoticeWebView();

		noticeWebView = (new GameObject("NoticeWebView")).AddComponent<WebViewObject>();
		noticeWebView.gameObject.name = "NoticeWebView";

		noticeWebView.Init((callbackMessage) =>
		{
			Debug.Log($"=====notice web View CallbackMessage: {callbackMessage}");
			CallbackMessage = callbackMessage;
		});

		TouchInputRecognizer.SetPause(true);
		PopUpController.ActiveAlphaScreen(true);

		try
		{
			int left, top, right, bottom;

			// 정사각형 타입
			if (item.popupType == "square")
			{
				if (Screen.height > Screen.width)
				{
					left = 0;
					right = 0;
					top = (int)((Screen.height - Screen.width) * 0.5f);
					bottom = (int)((Screen.height - Screen.width) * 0.5f);
				}
				else if (Screen.height == Screen.width)
				{
					left = (int)(Screen.width - (Screen.width * 0.9f));
					right = (int)(Screen.width - (Screen.width * 0.9f));
					top = (int)(Screen.height - (Screen.height * 0.9f));
					bottom = (int)(Screen.height - (Screen.height * 0.9f));
				}
				else
				{
					left = (int)(Screen.width - (Screen.height * 0.5f));
					right = (int)(Screen.width - (Screen.height * 0.5f));
					top = 0;
					bottom = 0;
				}
			}
			else
			{
				// 커스텀 타입
				left = (int)(Screen.width - (Screen.width * (1f - item.left)));
				top = (int)(Screen.height - (Screen.height * (1f - item.top)));
				right = (int)(Screen.width - (Screen.width * (1f - item.right)));
				bottom = (int)(Screen.height - (Screen.height * (1f - item.bottom)));
			}

			Debug.Log($"===== left: {left} / top: {top} / right: {right} / bottom: {bottom}");
			noticeWebView.SetScrollbarsVisibility(false);
			noticeWebView.SetMargins(left, top, right, bottom);
			noticeWebView.LoadURL(item.url);
			noticeWebView.SetVisibility(true);
			
		}
		catch(Exception e)
		{
			Debug.LogException(e);
		}
	}

	public void ShowUserInfoView()
	{
		//string url = string.Format("{0}/myPageForm/{1}/{2}?lang={3}&RuntimePlatform={4}&appVersion={5}", AppServerController.Instance.WebUri, Static_APP_Config._PackageName, FireBaseController.GetUserId(), Main.language == SystemLanguage.Korean ? "ko" : "en", Application.platform.ToString(), Application.version);
		//Debug.Log($"===== webView url: {url}");

		ClearRankingWebView();

		rankingWebView = (new GameObject("RankingWebViewObject")).AddComponent<WebViewObject>();
		rankingWebView.gameObject.name = "RankingWebView";

		rankingWebView.Init((callbackMessage) =>
		{
			Debug.Log($"=====ShowUserInfoView callback message: {callbackMessage}");
			CallbackMessage = callbackMessage;
		});

		rankingWebView.SetMargins(0, 0, 0, 0);
		//rankingWebView.LoadURL(url);
		rankingWebView.SetVisibility(true);
	}

	public void ShowRankingView()
	{
		//string url = string.Format("{0}/rankingList/{1}/{2}?lang={3}&RuntimePlatform={4}&appVersion={5}", AppServerController.Instance.WebUri, Static_APP_Config._PackageName, FireBaseController.GetUserId(), Main.language == SystemLanguage.Korean ? "ko" : "en", Application.platform.ToString(), Application.version);
		//Debug.Log($"===== webView url: {url}");

		ClearRankingWebView();

		rankingWebView = (new GameObject("RankingWebViewObject")).AddComponent<WebViewObject>();
		rankingWebView.gameObject.name = "RankingWebView";
		
		rankingWebView.Init((callbackMessage) =>
		{
			Debug.Log($"=====ShowRankingView callback message: {callbackMessage}");
			CallbackMessage = callbackMessage;
		});

		rankingWebView.SetMargins(0, 0, 0, 0);
		//rankingWebView.LoadURL(url);
		rankingWebView.SetVisibility(true);
	}

	public void SetWebView(string strUrl)
	{
#if UNITY_ANDROID
		FirebaseLogController.LogEvent("BannerRequest", "TDI");
#endif
		ClearWebView();
		string Url = string.Format("{0}", strUrl);

		Debug.Log(Url);
		webView = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
		webView.gameObject.name = "WebView";

		webView.Init((callbackMessage) =>
		{
			Debug.Log($"=====webView callback message: {callbackMessage}");
		}, null, null, (newurl) =>
		{
			if (newurl != Url)
			{
				Application.OpenURL(newurl);
			}
		});

		int height = (int)(Screen.height - (Screen.height * 0.08f));

		if (Screen.orientation == ScreenOrientation.Portrait)
		{
			webView.SetMargins(0, height, 0, 0);
			webView.LoadURL(Url);
			webView.SetVisibility(true);
		}
	}

	public void ClearWebView()
	{
		if (webView != null)
		{
			Destroy(webView.gameObject);
			webView = null;
		}
	}

	private void ClearRankingWebView()
	{
		if (rankingWebView)
		{
			rankingWebView.ClearCache(true);
			Destroy(rankingWebView.gameObject);
			rankingWebView = null;
		}
	}

	private void ClearNoticeWebView()
	{
		if (noticeWebView)
		{
			noticeWebView.ClearCache(true);
			Destroy(noticeWebView.gameObject);
			noticeWebView = null;
		}
	}
}
