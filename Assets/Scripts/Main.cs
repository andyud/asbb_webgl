using LitJson;
using System;
using UnityEngine;
using System.Collections;
#if UNITY_ANDROID
using Firebase;
#endif
using System.Collections.Generic;
#if UNITY_ANDROID
using System.Net;
using System.Net.Sockets;
#endif
using System.Linq;
using UnityEngine.Networking;


#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class MarketInfo_Request
{
	public MarketInfo_RequestData app;
}

public class MarketInfo_RequestData
{
	public string version;
}

public class MarketInfo_Response
{
	public string result;
	public int code;
	public string message;

	public MarketInfo_ResponseApp data;

}
public class MarketInfo_ResponseApp
{
	public MarketInfo_ResponseData app;
}

public class MarketInfo_ResponseData
{
	public string package;
	public string version;
	public bool update;
	public bool force;
	public string decrypt_key;

}
public class Main : MonoBehaviour
{
	public static Main _main;

	public SceneChanger _sceneChanger;
	public GUIController _gui;
	public InGameController _game;

	public static string ADID;
	public static string IP;

	static System.Random _random = new System.Random();

	public float _interstitialChance_Current = 0.0f;
	public float _interstitialChance_ElapsedTime;
	IEnumerator _thread_AdChance;

	public static bool isAdSkip = false;
	public static SystemLanguage language;

	public string handleUserEarnedRewardSkipGame = string.Empty;

	[SerializeField] private SmartShopWebRequest _smartShopWebRequest;
#if UNITY_ANDROID
	[SerializeField] private WebRequest _webRequest;
#endif
	void Awake()
	{
		Debug.Log(" >>> Awake 1 ");
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			Application.targetFrameRate = 60;

		if (PlayerPrefs.GetInt(PlayerPrefs_Config.SetStateSpeedMode, 0) == 0)
		{
			PlayerPrefs.SetInt(PlayerPrefs_Config.SetStateSpeedMode, 1);
			PlayerPrefs.SetInt(PlayerPrefs_Config.ActiveSpeedMode, 1);
		}
		Debug.Log(" >>> Awake 2 ");
		_main = this;
		language = Application.systemLanguage;

		Static_APP_Config.Initialize();
		Debug.Log(" >>> Awake 3 ");
		UserData.Initialize();
		Debug.Log(" >>> Awake 4 ");
		FontHolder.Initialize(language);
		Debug.Log(" >>> Awake 5 ");
		Static_TextConfigs.Initialize(language);
		Debug.Log(" >>> Awake 6 ");
		Static_ColorConfigs.Initialize();
		Debug.Log(" >>> Awake 7 ");
		CurveHolder.Initialize();
		Debug.Log(" >>> Awake 8 ");
		ObjectPoolHolder.Initialize();
		Debug.Log(" >>> Awake 9 ");
		ObjectPool_GUI.Initialize();
		Debug.Log(" >>> Awake 10 ");
		SoundController.Initialize();
		Debug.Log(" >>> Awake 11 ");
		MaterialHolder.Initialize();
		Debug.Log(" >>> Awake 12 ");
		//GameCenterController.Initialize();


		_sceneChanger.Initialize();
		Debug.Log(" >>> Awake 13 ");
		if (_gui != null)
		{
			_gui.Initialize();
		}
		Debug.Log(" >>> Awake 14 ");
		_game.Initialize(Callback_ScoreChange);
		Debug.Log(" >>> Awake 15 ");
		_gui.gameObject.SetActive(true);
		Debug.Log(" >>> Awake 16 ");
		PopUpController.SetActivateGameObject(true);
		Debug.Log(" >>> Awake 17 ");
		_game.gameObject.SetActive(true);
		Debug.Log(" >>> Awake 18 ");
		_interstitialChance_ElapsedTime = UserData._AC_ElapsedTime;
		_thread_AdChance = null;
		Debug.Log(" >>> Awake 19 ");
	}


	void NoticeWebInit()
	{
		Debug.Log(" >>> NoticeWebInit 1 ");
		var todayCloseTimeString = PlayerPrefs.GetString(PlayerPrefs_Config.TodayCloseTime, string.Empty);
		if (!string.IsNullOrEmpty(todayCloseTimeString))
		{
			var todayCloseTime = DateTime.Parse(todayCloseTimeString);
			var gapTime = DateTime.UtcNow - todayCloseTime;
			if (gapTime.TotalDays >= 1)
				PlayerPrefs.SetString(PlayerPrefs_Config.TodayClose, string.Empty);

			Debug.Log($"=====gapTime: {gapTime} / totalDays: {gapTime.TotalDays}");

		}
		Debug.Log(" >>> NoticeWebInit 2 ");
#if UNITY_ANDROID
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach(var ip in host.AddressList)
		{
			if(ip.AddressFamily == AddressFamily.InterNetwork)
			{
				Debug.Log($"===== ip: {ip}");
				IP = ip.ToString();
			}
		}
#endif
		Debug.Log(" >>> NoticeWebInit 3 ");
		bool isSupport = Application.RequestAdvertisingIdentifierAsync((string advertisingId, bool trackingEnabled, string errorMsg) =>
		{
			ADID = advertisingId;
			Debug.Log($"=====ADID: {ADID}");

			NoticeWeb();
		});
		Debug.Log(" >>> NoticeWebInit 4 ");
		if (isSupport == false)
		{
			Debug.Log("isSupport is false");

			NoticeWeb();
		}
		Debug.Log(" >>> NoticeWebInit 5 ");
	}

	IEnumerator GetMarketVersionRequest()
	{
		//AppServiceController.Instance.GetUserMigrate(null);
		//AppServiceController.Instance.GetBeforeDataTest("8FxmbDp2ggSJNzoQaUzg7oaDft13");
		//AppServerController.Instance.GetBeforeIAPDataTest("8FxmbDp2ggSJNzoQaUzg7oaDft13"); 
		//AppServiceController.Instance.GetValidationUserReceiptTest();
		//AppServiceController.Instance.ValidationUserReceipt(null);
		//AppServiceController.Instance.AppStart(null); 

		// �Ʒ��κ��� ����� �����ϸ� �̺κ��� ���������..
		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/version", AppServerController.Instance.ServerUri), "POST"))
		{
			MarketInfo_Request appData = new MarketInfo_Request();
			appData.app = new MarketInfo_RequestData();
			appData.app.version = Application.version;

			string jsonData = JsonMapper.ToJson(appData);
			Debug.Log("======= GetMarketVersionRequest : " + jsonData);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log(" ======== Error While Sending: " + uwr.error);
			}
			else
			{
				Debug.Log("======= Received: " + uwr.downloadHandler.text);

				var responseData = JsonMapper.ToObject<MarketInfo_Response>(uwr.downloadHandler.text);

				Debug.Log("======= MarketUpdate force : " + responseData.data.app.force);
				if (responseData.data.app.update)
				{
					if (responseData.data.app.force)
						PopUpController.AddAlert(AlertPopUpType.Force_Update);
					else
					{
						//WebViewController.instance.noticeList = CheckTodayCloseList(callback.popup);
						PopUpController.AddAlert(AlertPopUpType.Recommended_Update);
					}
					yield break;
				}
			}
		}

		AppServerController.Instance.NoticeWeb(callback =>
		{
			if (callback == null)
				return;

			switch (callback.reason)
			{
				//case nameof(ReasonType.update):
				//                if (callback.force)
				//                {
				//                    Debug.Log("===== callback force update");
				//                    PopUpController.AddAlert(AlertPopUpType.Force_Update);
				//                }
				//                else
				//                {
				//                    Debug.Log("===== callback recommend update");

				//                    WebViewController.instance.noticeList = CheckTodayCloseList(callback.popup);
				//                    PopUpController.AddAlert(AlertPopUpType.Recommended_Update);
				//                }
				//                break;
				case nameof(ReasonType.popup):
					WebViewController.instance.noticeList = CheckTodayCloseList(callback.popup);
					WebViewController.instance.ShowNoticeWebView();
					break;
				case nameof(ReasonType.success):
					WebViewController.instance.noticeList = CheckTodayCloseList(callback.popup);
					WebViewController.instance.ShowNoticeWebView();

					if (callback.force)
					{
						Debug.Log("===== callback force update");
						PopUpController.AddAlert(AlertPopUpType.Force_Update);
					}
					else
					{
						Debug.Log("===== callback recommend update");
						PopUpController.AddAlert(AlertPopUpType.Recommended_Update);
					}
					break;
			}
		});

		yield break;
	}

	Coroutine _coNotice;
	void NoticeWeb()
	{
		if (_coNotice != null) StopCoroutine(_coNotice);
		_coNotice = StartCoroutine(GetMarketVersionRequest());
	}

	List<Notice_Response_Item> CheckTodayCloseList(Notice_Response_Item[] itemArray)
	{
		var returnList = itemArray.ToList();

		var todayClose = PlayerPrefs.GetString(PlayerPrefs_Config.TodayClose, string.Empty);
		if (!string.IsNullOrEmpty(todayClose))
		{
			var splitString = todayClose.Split('_');

			returnList = returnList.FindAll(v =>
			{
				for (int i = 0; i < splitString.Length; i++)
				{
					if (v.id == int.Parse(splitString[i]))
						return false;
				}
				return true;
			});
		}

		return returnList;
	}

	void OnApplicationFocus(bool focusStatus)
	{
		UserData._AC_ElapsedTime = _interstitialChance_ElapsedTime;

		if (focusStatus)
		{
			//if (WebViewController.instance.webView != null)
			//{
			//	WebViewController.instance.ClearWebView();
			//	if (AdControllerByTDI.init)
			//	{
			//		AdControllerByTDI.SetADBannerbyTDI();
			//		return;
			//	}
			//}

			if (PopUpController._IsPopUpOpened)
				return;

			//AdController.Hide_BannerView();
			//WebViewController.instance.ClearWebView();
#if UNITY_ANDROID
            AdController.LoadAdBanner();
            AdController.Show_BannerView();
#endif
		}
	}

	void OnApplicationPause(bool pauseState)
	{
		UserData._AC_ElapsedTime = _interstitialChance_ElapsedTime;
	}

	IEnumerator CheckNetwork()
	{
		Debug.Log(" >>> CheckNetwork 1 ");
		var waitForSeconds = new WaitForSeconds(10f);
		bool allInit = false;

		while (allInit == false)
		{
			yield return waitForSeconds;

			if (NetworkReachability.NotReachable != Application.internetReachability)
			{
				if (Application.platform == RuntimePlatform.Android)//&& language == SystemLanguage.Korean)
				{
#if UNITY_ANDROID
					if (FireBaseController.dependencyStatus == Firebase.DependencyStatus.Available && AdController.IsInit && SmartShopWebRequest.IsInit)
					{
						Debug.Log("Main.CheckNetwork() / allInit is true");
						allInit = true;
						yield break;
					}
#endif
				}
				else
				{
#if UNITY_ANDROID
					if (FireBaseController.dependencyStatus == Firebase.DependencyStatus.Available && AdController.IsInit)
					{
						Debug.Log("Main.CheckNetwork() / allInit is true");
						allInit = true;
						yield break;
					}
#endif
				}
#if UNITY_ANDROID
				if (FireBaseController.dependencyStatus == DependencyStatus.UnavailableOther)
				{
					Debug.Log("Main.CheckNetwork() / FireBaseController.Initialize()");
					FireBaseController.Initialize();
				}

				if (AdController.IsInit == false)
				{
					Debug.Log("Main.CheckNetwork() / AdController.Initialize()");
					//AdController.Initialize();
				}
#endif
				if (Application.platform == RuntimePlatform.Android)
				{

					if (SmartShopWebRequest.IsInit == false)
					{
						Debug.Log("Main.CheckNetwork() / _smartShopWebRequest.Initialize()");
						_smartShopWebRequest.Initialize();
					}
				}
			}
		}
		Debug.Log(" >>> CheckNetwork 2 ");
	}
	public void InitNetworkCall()
	{

		Debug.Log(" >>> InitNetworkCall 1 ");
#if UNITY_ANDROID
		StartCoroutine(CheckNetwork());
		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			PopUpController.AddAlert(AlertPopUpType.Network_NotReachable);
			return;
		}

		FireBaseController.Initialize();

		if (Application.platform == RuntimePlatform.Android)
		{
			//AdControllerByTDI.Initialize();

			if (language == SystemLanguage.Korean)
				_smartShopWebRequest.Initialize();
		}

		AdController.Initialize();

		//// ���� ������ aos �ʸ� ���Ӽ��� ���� �Ǿ��־..
		//if(Application.platform == RuntimePlatform.Android)
		//{
		//	GameCenterController.LogIn();
		//}
#endif
		Debug.Log(" >>> InitNetworkCall 2 ");
	}

	public bool isConsentCheck = false;

	IEnumerator Start()
	{
		if (UserData._GameStatus == null)
		{
			Set_TitlePage();
			InGameController.Direct_Ready();
		}
		else
		{
			Set_Continue();
		}

		_sceneChanger.FadeIn(null);

		NoticeWebInit();

		InitNetworkCall();

#if UNITY_ANDROID
		if (PlayerPrefs.GetInt("RefreshIAP") == 0)
		{
			while (IAPController.storeController == null || IAPController.storeController.products == null)
				yield return null;

			if (String.IsNullOrEmpty(FireBaseController.GetUserId()))
			{
				AppServerController.Instance.GetIAP();
				PlayerPrefs.SetInt("RefreshIAP", 1);
			}
		}
#endif

		yield return new WaitUntil(() => PopUpController._IsPopUpOpened == false);

		//todo: ��Ʈ��ũ ���� Ȯ�� �Ϸ� �� �ڻ� ���� �˾� ���� �õ�
#if UNITY_IOS
		if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
		{
			PopUpController_Imp.Open_IosIDFA();

			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			yield return new WaitUntil(() => PopUpController._IsPopUpOpened == false);
		}
#endif

		//if (UserData._GameStatus == null)
		//{
		//	Set_TitlePage ();
		//	InGameController.Direct_Ready();
		//}
		//else
		//{
		//	Set_Continue();
		//}

		if (UserData._IsTutorialCleared == false)
		{
			_main._gui.OpenHowToPlay();

			// ���� ���� ������ ���ؼ� ���ǵ�, �Ŀ� ī��Ʈ ����
			PlayerPrefs.SetInt(PlayerPrefs_Config.ActiveSpeedMode, 1);
			PlayerPrefs.SetInt(PlayerPrefs_Config.ActivePowerMode, 1);
			PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, SpeedMode_Config.FirstRunFreeRewardCount);
			PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, PowerMode_Config.FirstRunFreeRewardCount);
		}
		//else if (UserData._ShowContactPoint_Selected == false)
		//	GUIController.OpenSetting ();
		else
		{
			if (PlayerPrefs.GetInt(PlayerPrefs_Config.UpdateFirstOpen, 0) == 0)
			{
				if (!StaticMethod.IsPurchase_Speed_Total_SmartShopReward())
				{
					PlayerPrefs.SetInt(PlayerPrefs_Config.ActiveSpeedMode, 1);
					PlayerPrefs.SetInt(PlayerPrefs_Config.ActivePowerMode, 1);
					var currentSpeedTurn = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, 0);
					var currentPowerTurn = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0);
					PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, currentSpeedTurn + SpeedMode_Config.FirstRunFreeRewardCount);
					PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, currentPowerTurn + PowerMode_Config.FirstRunFreeRewardCount);

					PopUpController.AddAlert(AlertPopUpType.Update_Reward_Item, false);
				}
			}
		}

		PlayerPrefs.SetInt(PlayerPrefs_Config.UpdateFirstOpen, 1);
		//_sceneChanger.FadeIn(null);

		yield return new WaitForSeconds(1.0f);
		//AdController.Show_BannerView();

		//if (PlayerPrefs.GetInt("Open_LoginNotice", 0) == 0)
		//{
		//    PlayerPrefs.SetInt("Open_LoginNotice", 1);
		//    PopUpController.Open_LoginNotice();
		//}
#if UNITY_ANDROID
        if (FireBaseController.isLogin)
		{
			if (PlayerPrefs.GetInt("UserMigrate_Request_0526", 0) == 0)	
            {
				AppServerController.Instance.GetUserMigrate(null);
			}
		}
#endif
		Debug.Log(" >>> Start 2 ");
	}

	static void Set_TitlePage()
	{
		Debug.Log(" >>> Set_TitlePage 1 ");
		Screen.sleepTimeout = SleepTimeout.SystemSetting;

		UserData._GameStatus = null;

		_main._game.Reset ();
		InGameController.Direct_Ready();
		Debug.Log(" >>> Set_TitlePage 5 ");
		_main._gui.Enter_Title();
		Debug.Log(" >>> Set_TitlePage 6 ");
		_main._thread_AdChance = null;
		Debug.Log(" >>> Set_TitlePage 7 ");
	}

	static void Set_InGame()
	{
		Debug.Log(" >>> Set_InGame 1 ");
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Debug.Log(" >>> Set_InGame 2 ");
		if (Main.isAdSkip)
			InGameController._instance._shooter.ResetSkip();
		Debug.Log(" >>> Set_InGame 3 ");
		_main._gui.Enter_InGame();
		_main._game.StartGame();
		Debug.Log(" >>> Set_InGame 4 ");
		_main._thread_AdChance = _main.Thread_AdChance();
		Debug.Log(" >>> Set_InGame 5 ");
		// mode check
		InGameController._instance._shooter.InitSpeedAndPowerMode();
		Debug.Log(" >>> Set_InGame 6 ");
		// reset revive count
		PlayerPrefs.SetInt(PlayerPrefs_Config.ReviveCount, Revive.ReviveCount);
		Debug.Log(" >>> Set_InGame 7 ");
		StartRankingScore();
		Debug.Log(" >>> Set_InGame 8 ");
	}

	static void Set_Continue()
	{
		Debug.Log(" >>> Set_Continue 1 ");
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		InGameController gameInstance = _main._game as InGameController;

		_main._game.Reset();
		gameInstance.Continue(UserData._GameStatus);
		_main._gui.Enter_InGame();

		bool isNewRecord = UserData._GameStatus._currentScore >= UserData._Score_Best ? true : false;
		_main._gui.UpdateScore(UserData._GameStatus._currentScore, 0, isNewRecord);

		_main._thread_AdChance = _main.Thread_AdChance();

		// mode check
		InGameController._instance._shooter.InitSpeedAndPowerMode();

		StartRankingScore();
	}

	static void StartRankingScore()
	{
		int startScore;
		if (InGameController._instance._status._currentTurn == 1)
			startScore = 0;
		else
			startScore = InGameController._instance._status._currentTurn;

		Debug.Log($"{_main._gui._currentState}");
		PlayerPrefs.SetInt(PlayerPrefs_Config.StartRankingScore, startScore);

		// request ranking score
#if UNITY_ANDROID
		if (FireBaseController.isLogin)
		{
			AppServerController.Instance.RankingScore((response) =>
			{
				if (response != null)
				{
					Debug.Log($"=====call back StartRankingScore / reason: {response.reason}");
				}

			}, startScore, true);
		}
#endif
		Debug.Log(" >>> StartRankingScore 2 ");
	}

	static void Callback_ScoreChange(int score, int delta, bool isNewRecord)
	{
		Debug.Log(" >>> Callback_ScoreChange 1 ");
		_main._gui.UpdateScore(score, delta, isNewRecord);
	}

	public static void StartGame()
	{
		Debug.Log(" >>> StartGame 1 ");
		isAdSkip = false;
		Set_InGame();
	}

	public void DelayStartSkipGame()
	{
		Invoke("StartSkipGame", 0.1f);
	}

	public void StartSkipGame()
	{
		Debug.Log(" >>> StartSkipGame 1 ");
		isAdSkip = true;
		Set_InGame();
	}

	public static void BackToTitle()
	{
		Debug.Log(" >>> BackToTitle 1 ");
		isAdSkip = false;
		UserData._PlayCount++;
		_main._sceneChanger.FadeOutAndIn(Set_BackToTitle, null);
		Debug.Log(" >>> BackToTitle 2 ");
	}

	static void Set_BackToTitle()
	{
		// request ranking score
#if UNITY_ANDROID
		if(FireBaseController.isLogin)
		{
			AppServerController.Instance.RankingScore((response) =>
			{
				if (response != null)
				{
					Debug.Log($"=====call back EndRankingScore / reason: {response.reason}");
				}
			}, InGameController._instance._status._currentTurn, false);
		}
#endif
		Debug.Log(" >>> Set_BackToTitle 1 ");
		ShowInterstitial();
		Set_TitlePage();
	}

	public static void GameOver()
	{
		Debug.Log(" >>> GameOver 1 ");
		_main._thread_AdChance = null;

		Screen.sleepTimeout = SleepTimeout.SystemSetting;
		if (_main._game._Score_Current > UserData._Score_Best)
		{
			UserData._Score_Best = _main._game._Score_Current;
			//GameCenterController.Post_HighScore(_main._game._Score_Current);
		}

		PlayerPrefs.SetInt(PlayerPrefs_Config.EndRankingScore, _main._game._Score_Current);
#if UNITY_ANDROID
		if (FireBaseController.isLogin)
		{
			// request ranking score
			AppServerController.Instance.RankingScore((response) =>
			{
				if (response != null)
				{
					Debug.Log($"=====call back EndRankingScore / reason: {response.reason}");
				}
			}, InGameController._instance._status._currentTurn, false);
		}
#endif
		//		if (IsPromoCondition_Rating(1) || IsPromoCondition_Facebook(1))
		//		{
		//#if UNITY_EDITOR
		//			Debug.LogWarning("[Interstitial] Don't check : Promo PopUp");
		//#endif

		//			_main._gui.Enter_GameOver (_main._game._Score_Current, _main._game._IsNewRecord, null);
		//		}
		//		else
		_main._gui.Enter_GameOver(_main._game._Score_Current, _main._game._IsNewRecord, ShowInterstitial);
	}

	//public static void LogIn_and_PostHighScore(System.Action successCallback)
	//{
	//	GameCenterController.LogIn_and_PostHighScore (_main._game._Score_Current, successCallback);
	//}

	public static void BackToHome()
	{
		isAdSkip = false;
		UserData._PlayCount++;
		_main._sceneChanger.FadeOut(CheckPromo_BackToTitle);
	}

	public static void Restart()
	{
		isAdSkip = false;
		UserData._PlayCount++;
		_main._sceneChanger.FadeOut(CheckPromo_Restart);
	}

	public void DelayRestartSkip()
	{
		Invoke("RestartSkip", 0.1f);
	}

	public void RestartSkip()
	{
		isAdSkip = true;
		UserData._PlayCount++;
		_main._sceneChanger.FadeOut(CheckPromo_Restart);
	}

	static void CheckPromo_BackToTitle()
	{
		//if(IsPromoCondition_Rating(0))
		//	PopUpController.Open_RequestRating(FadeIn_TitlePage);
		//else if(IsPromoCondition_Facebook(0))
		//	PopUpController.Open_RequestFacebook(FadeIn_TitlePage);
		//else
		FadeIn_TitlePage();
	}

	static void CheckPromo_Restart()
	{
		//if(IsPromoCondition_Rating(0))
		//	PopUpController.Open_RequestRating(FadeIn_Restart);
		//else if(IsPromoCondition_Facebook(0))
		//	PopUpController.Open_RequestFacebook(FadeIn_Restart);
		//else
		FadeIn_Restart();
	}

	static void FadeIn_TitlePage()
	{
		Set_TitlePage();
		InGameController.Direct_Ready();
		_main._sceneChanger.FadeIn(null);
	}

	static void FadeIn_Restart()
	{
		UserData._GameStatus = null;
		_main._game.Reset();
		Set_InGame();
		InGameController.Direct_Ready();
		_main._sceneChanger.FadeIn(null);
	}

	static void ShowInterstitial()
	{
		//WebViewController.instance.ClearWebView();
		AdController.Show_Interstitial();
		return;
	}

	static void ResetInterstitialChance()
	{
		_main._interstitialChance_Current = 0.0f;
		_main._interstitialChance_ElapsedTime = 0.0f;
	}

	IEnumerator Thread_AdChance()
	{
		while (true)
		{
			if (_interstitialChance_ElapsedTime < Static_APP_Config._interstitialChance_Cooltime)
			{
				_interstitialChance_Current = 0;
			}
			else
			{
				if (_interstitialChance_Current < Static_APP_Config._interstitialChance_Max)
				{
					float lerpRatio = (_interstitialChance_ElapsedTime - Static_APP_Config._interstitialChance_Cooltime) / (Static_APP_Config._interstitialChange_PickTime - Static_APP_Config._interstitialChance_Cooltime);
					_interstitialChance_Current = Mathf.Lerp(Static_APP_Config._interstitialChance_Min, Static_APP_Config._interstitialChance_Max, lerpRatio);
				}
			}

			yield return 0;

			if (PopUpController._IsPopUpOpened == false)
				_interstitialChance_ElapsedTime += RealTime.deltaTime;
		}
	}

	//static bool IsPromoCondition_Rating(int countModifier)
	//{
	//	if(UserData._IsRated == false && UserData._ExpiredTime_RequestRate.CompareTo(System.DateTime.Now) == -1 && UserData._PlayCount + countModifier >= FN._playCount_Rating)
	//		return true;
	//	else
	//		return false;
	//}

	//static bool IsPromoCondition_Facebook(int countModifier)
	//{
	//	if(UserData._IsFacebookVisited == false && UserData._ExpiredTime_RequestFacebookVisit.CompareTo(System.DateTime.Now) == -1 && UserData._PlayCount + countModifier >= FN._playCount_FacebookVisit)
	//		return true;
	//	else
	//		return false;
	//}

	public static void Pause()
	{
		Time.timeScale = 0;
		TouchInputRecognizer.SetPause(true);
	}

	public static void UnPause()
	{
		Time.timeScale = 1.25f;

		TouchInputRecognizer.SetPause(false);
	}

	public static void SendRankinScore()
	{
		_main.StartCoroutine("SendRanking");
	}

	IEnumerator SendRanking()
	{
		bool isWait = true;
		bool isStart = true;

		if (_main._gui._currentState != GUIController.State.INGAME)
		{
			Debug.Log("1************* �α��� StartRankingScore true");
			AppServerController.Instance.RankingScore(response =>
			{
				if (response != null)
					Debug.Log($"call back startRankingScore / reason: {response.reason}");

				isWait = false;
				isStart = false;
			}, 0, true);
		}
		else
			isWait = false;

		while (isWait)
			yield return null;

		if (!isStart)
			yield return new WaitForSeconds(0.5f);

		int bestscore = UserData._Score_Best;
		if (UserData._Score_Best <= PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore))
			bestscore = PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore);

		Debug.Log("2************* �α��� StartRankingScore " + isStart);

		Debug.Log("2************* �α��� StartRankingScore : " + UserData._Score_Best);

		AppServerController.Instance.RankingScore(response =>
		{
			if (response != null)
				Debug.Log($"call back endRankingScore / reason: {response.reason}");
		}, bestscore, isStart);// PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore), true);

		yield break;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (WebViewController.instance.rankingWebView != null)
				return;

			if (WebViewController.instance.noticeWebView != null)
				return;

			if (PopUpController.GetCurrentPopup() as AlertPopUp)
				return;

			if (_sceneChanger._OnChanging == false)
			{
				if (PopUpController._IsPopUpOpened && UserData._IsTutorialCleared)
					PopUpController.CloseCurrentPopUp();
				else
					_gui.DoEscape();
			}
		}

#if UNITY_EDITOR

		// �ξ۰����� ��ź�
		if (Input.GetKeyDown(KeyCode.A))
		{
			var receiptData = new List<ReceiptData>();
			var data = new ReceiptData();
			data.productId = "swipe_ball_speed";
			data.purchaseTokenOrPayload = "cbnigjmnoainngibkfdbpdjn.AO-J1OwBon88cWM-2_iZo4k0QEmCw9B7--EUJqnvj49scuB350KntiMc3Mw-dK4W1bKuUtYhcRecszZk0lbb_SY5PqYvCzfzOhTmPm5Sp77RCcqiyHo2HVSGkzw9pAjNd20y6rNQ7FsHJB1bJVgRnyS2eKtxTmLJpA";
			receiptData.Add(data);

			AppServerController.Instance.ReceiptVerification(1, receiptData, (response) =>
			{
				Debug.Log($"=====ReceiptVerification / response.result: {response.result}");

				if (response.data != null)
				{
					foreach (var item in response.data)
					{
						Debug.Log($"=====ReceiptVerification / response.data.productId: {item.productId} / response.data.status: {item.status}");
					}
				}
			}, true);
		}

		//// �� ����� ������ ��ȸ
		//if(Input.GetKeyDown(KeyCode.S))
		//{
		//	AppServiceController.Instance.ValidationUserReceipt((response) =>
		//	{
		//		Debug.Log($"=====ValidationUserReceipt / response.result: {response.result}");
		//		if(response.data != null)
		//		{
		//			foreach (var item in response.data)
		//			{
		//				Debug.Log($"=====ValidationUserReceipt / response.data.productId: {item.productId} / response.data.status: {item.status}");
		//			}
		//		}
		//		else
		//		{
		//			Debug.Log($"===== message: {response.message}");
		//		}
		//	}, true);
		//}

		// �ű� ���� üũ
		if (Input.GetKeyDown(KeyCode.D))
		{
			AppServerController.Instance.CheckNewDBUser((response) =>
			{
				Debug.Log($"=====CheckNewDBUser / response.result: {response.result} / response.data: {response.data}");
			}, true);
		}

		// ���� ���� ��������
		if (Input.GetKeyDown(KeyCode.F))
		{
			AppServerController.Instance.GetUserBackUpData((response) =>
			{
				Debug.Log($"=====GetUserBackUpData / response.result: {response.result}");
				Debug.Log($"=====GetUserBackUpData / response.data.BestScore: {response.data.BestScore} / response.data.LastBackupDate: {response.data.LastBackupDate}");
			}, true);
		}

		// ���� ���� ����ϱ�
		if (Input.GetKeyDown(KeyCode.G))
		{
			var backupData = new NewBackUpData();
			backupData.BestScore = 101;
			backupData.LastBackupDate = DateTime.Now;
			AppServerController.Instance.SetUserBackUpData(backupData, (response) =>
			{
				Debug.Log($"=====SetUserBackUpData / response.result: {response.result}");
			}, true);
		}
#endif

		_sceneChanger.MoveNext();
		_game.MoveNext();
		_gui.MoveNext();

		if (_thread_AdChance != null)
			_thread_AdChance.MoveNext();


		if (!string.IsNullOrEmpty(handleUserEarnedRewardSkipGame))
		{
			Invoke(handleUserEarnedRewardSkipGame, 0.1f);
			handleUserEarnedRewardSkipGame = string.Empty;
		}
	}

	public static string GetLocalCode()
	{
		switch (Application.systemLanguage)
		{
			case SystemLanguage.Afrikaans: return "af";
			case SystemLanguage.Arabic: return "ar";
			case SystemLanguage.Basque: return "eu";
			case SystemLanguage.Belarusian: return "be";
			case SystemLanguage.Bulgarian: return "bg";
			case SystemLanguage.Catalan: return "ca";
			case SystemLanguage.Chinese: return "zh";
			case SystemLanguage.Czech: return "cs";
			case SystemLanguage.Danish: return "da";
			case SystemLanguage.Dutch: return "nl";
			case SystemLanguage.English: return "en";
			case SystemLanguage.Estonian: return "et";
			case SystemLanguage.Faroese: return "fo";
			case SystemLanguage.Finnish: return "fi";
			case SystemLanguage.French: return "fr";
			case SystemLanguage.German: return "de";
			case SystemLanguage.Greek: return "el";
			case SystemLanguage.Hebrew: return "he";
			case SystemLanguage.Hungarian: return "hu";
			case SystemLanguage.Icelandic: return "is";
			case SystemLanguage.Indonesian: return "id";
			case SystemLanguage.Italian: return "it";
			case SystemLanguage.Japanese: return "ja";
			case SystemLanguage.Korean: return "ko";
			case SystemLanguage.Latvian: return "lv";
			case SystemLanguage.Lithuanian: return "lt";
			case SystemLanguage.Norwegian: return "no";
			case SystemLanguage.Polish: return "pl";
			case SystemLanguage.Portuguese: return "pt";
			case SystemLanguage.Romanian: return "ro";
			case SystemLanguage.Russian: return "ru";
			case SystemLanguage.SerboCroatian: return "sr";
			case SystemLanguage.Slovak: return "sk";
			case SystemLanguage.Slovenian: return "sl";
			case SystemLanguage.Spanish: return "es";
			case SystemLanguage.Swedish: return "sv";
			case SystemLanguage.Thai: return "th";
			case SystemLanguage.Turkish: return "tr";
			case SystemLanguage.Ukrainian: return "uk";
			case SystemLanguage.Vietnamese: return "vi";
			default: return "en";
		}
	}
}
