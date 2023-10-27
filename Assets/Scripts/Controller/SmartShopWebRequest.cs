using LitJson;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class SmartShopWebRequest : MonoBehaviour
{
	private static SmartShopWebRequest _instance;

	public static Action<bool> actionActivated_Btn;

	private static string ADID;

	public static bool IsInit;

	public void Initialize()
	{
		Debug.Log(">>> SmartShopWebRequest: Initialize");
		_instance = this;

		RequestAdvertisingIdentifierAsync();
		SmartShopController.CallbackActivitySmartShop();
	}

	public static void RequestAdvertisingIdentifierAsync()
	{
		Application.RequestAdvertisingIdentifierAsync((string advertisingId, bool trackingEnabled, string errorMsg) =>
		{
			ADID = advertisingId;
			if (!string.IsNullOrEmpty(errorMsg))
			{
				PopUpController.AddAlert(AlertPopUpType.SmartShop_RequestAdvertisingIdentifierAsync_Error);
				Debug.LogErrorFormat("error => SmartShopWebRequest.Awake() / errormessage: {0}", errorMsg);
			}
			else
			{
				IsInit = true;
				_instance.CoroutineRequest();
			}
		});
	}

	public static void RetryCoroutineRequest()
	{
		_instance.CoroutineRequest();
	}

	private void CoroutineRequest()
	{
		StartCoroutine(_instance.Request());
	}

	private IEnumerator Request()
	{
		using (UnityWebRequest uwr = UnityWebRequest.Get(string.Format("{0}/api/v1/shop?adid={1}", AppServerController.Instance.ServerUri, ADID)))
		{
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogErrorFormat("error => SmartShopWebRequest.Request() / errormessage: {0}", uwr.error);

				PopUpController.AddAlert(AlertPopUpType.SmartShop_WebRequest_Error);
				yield break;
			}
			else
			{
				try
				{
					Debug.LogFormat("uwr.downloadHandler: {0}", uwr.downloadHandler.text);
					JsonData json = JsonMapper.ToObject(uwr.downloadHandler.text);
					// smartshop btn 노출 관련
					SmartShopController.isActivated_Btn = int.Parse(json["data"]["shop"]["activated"].ToString()) > 0;
					if (actionActivated_Btn != null)
						actionActivated_Btn.Invoke(SmartShopController.isActivated_Btn);

					// 스마트 샵 구입 관련 보상 리워드
					SmartShopController.reward_Activated = int.Parse(json["data"]["reward"]["activated"].ToString()) > 0;
					var reward_Expired_Date = json["data"]["reward"]["expired_date"].ToString();
					var reward_Item_Type = json["data"]["reward"]["item"].ToString();

					if (SmartShopController.reward_Activated)
					{
						PlayerPrefs.SetString(PlayerPrefs_Config.SmartShopRewardExpiredDate, reward_Expired_Date);
					}

					// 스마트샵 보상 아이템 유효기간 체크
					SmartShopController.CheckRewardItemExpireDateTime();


					bool notice_Activated = false;
					bool notice_New = false;
					string notice_Title = string.Empty;
					string notice_Message = string.Empty;
					// 공지 관련.
					if (json["data"]["reward"]["notice"].Count > 0)
					{
						notice_Activated = true;
						notice_New = int.Parse(json["data"]["reward"]["notice"]["urgency"].ToString()) > 0;
						notice_Title = json["data"]["reward"]["notice"]["title"].ToString();
						notice_Message = json["data"]["reward"]["notice"]["msg"].ToString();
					}
					else
					{
						notice_Activated = false;
						notice_New = false;
						notice_Title = notice_Message = string.Empty;
					}

					StopCoroutine("SmartShopNoticePopup");
					StartCoroutine(SmartShopNoticePopup(notice_New, notice_Title, notice_Message, notice_Activated));
				}
				catch (Exception e)
				{
					Debug.LogErrorFormat("error => SmartShopController.Request() / errormessage: {0}", e.Message);

					PopUpController.AddAlert(AlertPopUpType.SmartShop_WebRequest_Error);
					yield break;
				}
			}
		}
	}

	IEnumerator SmartShopNoticePopup(bool noticeNew, string title, string message, bool activeNotice)
	{
		Debug.Log("SmartShopNoticePopup()");

		yield return new WaitUntil(() => PopUpController._IsPopUpOpened == false);

		if (noticeNew)
		{
			Debug.Log("SmartShopController.notice_New");
            PopUpController.Open_SmartShopNotice(title, message);
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.OpenNotice, FirebaseLogType.New);
#endif
		}
		else if (activeNotice)
		{
			var dateString = PlayerPrefs.GetString(PlayerPrefs_Config.ExpiredSmartShopNoticeSkipData, string.Empty);
			if (!string.IsNullOrEmpty(dateString))
			{
				var expiredNoticeSkipDate = DateTime.Parse(dateString);

				// skip notice 만료
				if (DateTime.Today > expiredNoticeSkipDate)
				{
					Debug.Log("Expired skip notice ");
                    PopUpController.Open_SmartShopNotice(title, message);
#if UNITY_ANDROID
					FirebaseLogController.LogEvent(FirebaseLogType.OpenNotice, FirebaseLogType.ExpiredSkipNotice);
#endif
				}
			}
			else
			{
				Debug.Log("Activated Notice");
                PopUpController.Open_SmartShopNotice(title, message);
#if UNITY_ANDROID
				FirebaseLogController.LogEvent(FirebaseLogType.OpenNotice);
#endif
			}
		}
	}
}