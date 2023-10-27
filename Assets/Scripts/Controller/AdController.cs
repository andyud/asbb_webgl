using UnityEngine;
using System;
using System.Collections;
using GoogleMobileAds.Api;

public static class AdController
{
	static AdRequest _request;
	
	static BannerView _bannerView; // 배너
	
	static InterstitialAd _interstitial; // 전면
	
	static RewardedAd _rewarded_Speed; // 리워드 2배
	static RewardedAd _rewarded_SkipGame; // 리워드 100단위 스킵 턴
	static RewardedAd reward_PowerMode; // powerMode
	static RewardedAd reward_Revive; // Revive

	public static bool AdPurchase = false; // 광고 제거 인앱 구입
	public static bool AdReward = false; // 2배 광고 리워드
	static bool _isReStart = false; // 메인화면에서 시작인지, 게임 오버 후 재시작인지
	static bool _adSkipComplete = false; // 100단위 스킵 턴 체크

	static bool speedModeOpeningCheck;
	static bool powerModeOpeningCheck;
	static bool continueModeOpeningCheck;
	static bool reviveOpeningCheck;

	public static bool isReviveClose;
	public static bool isReviveShow;

	public static bool IsInit;

	//static AppId _curAppID = AppId.Lobby;
	static AdSize _bannerSize;

	static bool _isBannerLoad = false;

	public static void Initialize()
	{
		Debug.Log("Ad Initialize ----!!!");

		//AdPurchase = PlayerPrefs.GetInt("AdSpeedPurchase", 0) == 0 ? false : true;
		AdPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 0 ? false : true;

		MobileAds.Initialize(completeCallback =>
		{
			Debug.Log($"AdController.Initialize() / completeCallback");
		});

		_request = new AdRequest.Builder().Build();

		_bannerView = new BannerView(Static_APP_Config._AD_BannerID, calculateAdSize(), AdPosition.Bottom);
		_bannerView.OnAdLoaded += delegate { _isBannerLoad = true; };
		_bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		_bannerView.OnAdClosed += delegate { LoadAdBanner(); };

		_interstitial = new InterstitialAd(Static_APP_Config._AD_InterstitialID);
#if UNITY_IOS
		_interstitial.OnAdClosed += delegate { OnlyIosInterstitialOnAdClosed(); };
#else
		_interstitial.OnAdClosed += delegate { LoadAd(); };
#endif
		LoadAdBanner();
		LoadAd();

        _rewarded_Speed = new RewardedAd(Static_APP_Config._AD_Reward_SpeedID);
        _rewarded_Speed.OnUserEarnedReward += HandleUserEarnedRewardSpeed;
        _rewarded_Speed.OnAdClosed += HandleRewardedAdSpeedClosed;
        _rewarded_Speed.OnAdOpening += HandleOnAdOpeningSpeedMode;

        _rewarded_Speed.LoadAd(_request);

        _rewarded_SkipGame = new RewardedAd(Static_APP_Config._AD_Reward_SkipGameID);
		_rewarded_SkipGame.OnUserEarnedReward += HandleUserEarnedRewardSkipGame;
		_rewarded_SkipGame.OnAdClosed += HandleRewardedAdSkipGameClosed;
		_rewarded_SkipGame.OnAdOpening += HandleOnAdOpeningContinueMode;
		_rewarded_SkipGame.LoadAd(_request);

		//powermode
		reward_PowerMode = new RewardedAd(Static_APP_Config._AD_Reward_PowerModeID);
		reward_PowerMode.OnUserEarnedReward += HandleUserEarnedRewardPowerMode;
		reward_PowerMode.OnAdClosed += HandleRewardedAdClosedPowerMode;
		reward_PowerMode.OnAdOpening += HandleOnAdOpeningPowerMode;
		reward_PowerMode.LoadAd(_request);
		powerModeOpeningCheck = false;

		//revive
		reward_Revive = new RewardedAd(Static_APP_Config._AD_Reward_ReviveID);
		reward_Revive.OnUserEarnedReward += HandleUserEarnedRewardRevive;
		reward_Revive.OnAdClosed += HandleRewardedAdClosedRevive;
		reward_Revive.OnAdOpening += HandleOnAdOpeningRevive;
		reward_Revive.OnAdFailedToLoad += HandleOnAdFailedToLoadRevive;
		reward_Revive.LoadAd(_request);

		reviveOpeningCheck = false;

		IsInit = true;
	}


	//public static int GetDPToPixel()
	//{
	//	int height = 0;
	//	if (null != _bannerSize)
	//		height = _bannerSize.Height;

	//	if (_bannerView != null)
	//	{
	//		if (height < 50)
	//			height = 50;
	//	}
	//	else
	//		height = 0;

	//	float result = Mathf.Ceil(DPToPixel(Screen.height, height));
	//	result *= AspectRatioFitter.instance._defaultRatio;

	//	return (int)result;
	//}

	//// fFixedResoulutionHeight 내가 고정한 해상도 높이(720x1280으로 고정했다면 1280)
	//// fdpHeight 바꾸고자 하는 dp(애드몹 배너 320x50일 때 50)
	////public
	//static float DPToPixel(float fFixedResoulutionHeight, float fdpHeight)
	//{
	//	float fNowDpi = (Screen.dpi * fFixedResoulutionHeight) / Screen.height;
	//	float scale = fNowDpi / 160f;
	//	float pixel = fdpHeight * scale;
	//	return pixel;
	//}

	// 적응형 배너 관련.
	private static AdSize calculateAdSize()
	{
		// Full screen width
		_bannerSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
		return _bannerSize;
	}

#if UNITY_IOS
	public static void OnlyIosInterstitialOnAdClosed()
	{
		if(_interstitial != null)
		{
			_interstitial.Destroy();
			_interstitial = new InterstitialAd(FN._AD_InterstitialID);
			_interstitial.OnAdClosed += delegate { OnlyIosInterstitialOnAdClosed(); };
			LoadAd();
		}
	}
#endif

	public static void LoadAdBanner()
	{
		Hide_BannerView();

		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		Debug.Log("LoadAd ----!!!");
		if (_isBannerLoad) return;

		if (_bannerView != null)
			_bannerView.LoadAd(_request);
	}

	public static void LoadAd()
	{
		Hide_BannerView();

		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		Debug.Log("LoadAd ----!!!");

		if (_interstitial != null && _interstitial.IsLoaded() == false)
			_interstitial.LoadAd(_request);
	}

	public static void Show_BannerView()
	{
		Debug.Log("Show_BannerView ----!!!");
		Hide_BannerView();

		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		if (_bannerView != null)
			_bannerView.Show();
	}


	public static void Hide_BannerView()
	{
		Debug.Log("Hide_BannerView ----!!!");

		if (_bannerView != null)
			_bannerView.Hide();
	}

	public static void Show_Interstitial()
	{
		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		if (_interstitial != null && _interstitial.IsLoaded())
		{
			Debug.Log("전면광고 보이기");
#if UNITY_ANDROID
			FirebaseLogController.LogEvent("Interstitial", "Show");
#endif
			_interstitial.Show();
		}
		else
		{
			Debug.Log("전면광고 로드");
			LoadAd();
		}
	}

	public static void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("===== 로드 실패 : " + args);
		if (AdPurchase || SmartShopController.reward_Activated)
			return;
	}

    public static void Show_Reward_Speed()
    {
        Debug.Log("Show_Reward_Speed");
        if (AdPurchase || SmartShopController.reward_Activated)
            return;

        if (_rewarded_Speed != null && _rewarded_Speed.IsLoaded())
        {
            Debug.Log("Show_Reward_Speed show");
            _rewarded_Speed.Show();
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardSpeedMode);
#endif
			return;
        }

        // 고의적 네트워크 연결 차단을 통한 유저 보상을 막기 위해 체크함.
        if (NetworkReachability.NotReachable == Application.internetReachability)
        {
            PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
            return;
        }

        if (speedModeOpeningCheck) // 광고 시청중 종료한 유저에 대한 필터링 변수임.
        {
            if (_rewarded_Speed != null)
                _rewarded_Speed.LoadAd(_request);

            PopUpController.AddAlert(AlertPopUpType.RewardAdOpeningAfterForceClose);
        }
        else // 인벤토리가 비면 광고 보상을 지급한다.
        {
			//Debug.Log("Show_Reward_Speed reward");
			
			//if (Application.systemLanguage != SystemLanguage.Unknown)
			//{
			//	PopUpController.AddAlert(AlertPopUpType.Load_Fail_Ads);
			//}
			//else
            {
                InGameController._instance._shooter.AdRewardSpeedModeTurn = SpeedMode_Config.SpeedModeRewardTurn;
                InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
            }

			// 보상형 광고 셋
			_rewarded_Speed = new RewardedAd(Static_APP_Config._AD_Reward_SpeedID);
			_rewarded_Speed.OnUserEarnedReward += HandleUserEarnedRewardSpeed;
			_rewarded_Speed.OnAdClosed += HandleRewardedAdSpeedClosed;
			_rewarded_Speed.OnAdOpening += HandleOnAdOpeningSpeedMode;
			_rewarded_Speed.LoadAd(_request);

			//LoadAd();
			//FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardSpeedMode);
		}
    }

    public static void Show_Reward_PowerMode()
	{
        if (AdPurchase || SmartShopController.reward_Activated)
            return;

        if (reward_PowerMode != null && reward_PowerMode.IsLoaded())
		{
			reward_PowerMode.Show();
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardPowerMode);
#endif
			return;
		}

		// 고의적 네트워크 연결 차단을 통한 유저 보상을 막기 위해 체크함.
		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		if (powerModeOpeningCheck) // 광고 시청중 종료한 유저에 대한 필터링 변수임.
		{
			if (reward_PowerMode != null)
				reward_PowerMode.LoadAd(_request);

			PopUpController.AddAlert(AlertPopUpType.RewardAdOpeningAfterForceClose);
		}
		else // 인벤토리가 비면 광고 보상을 지급한다.
		{
			//if (Application.systemLanguage != SystemLanguage.Unknown)
			//{
			//	PopUpController.AddAlert(AlertPopUpType.Load_Fail_Ads);
			//}
			//else
			{
                InGameController._instance._shooter.AdRewardPowerModeTurn = PowerMode_Config.PowerModeRewardTurn;
                InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
            }

			// 보상형 광고 셋
			reward_PowerMode = new RewardedAd(Static_APP_Config._AD_Reward_PowerModeID);
			reward_PowerMode.OnUserEarnedReward += HandleUserEarnedRewardPowerMode;
			reward_PowerMode.OnAdClosed += HandleRewardedAdClosedPowerMode;
			reward_PowerMode.OnAdOpening += HandleOnAdOpeningPowerMode;
			reward_PowerMode.LoadAd(_request);
			//LoadAd();

			//FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardPowerMode);
		}
	}

	public static void Show_Reward_Revive()
	{
		if (reward_Revive != null && reward_Revive.IsLoaded())
        {
			isReviveShow = true;
			reward_Revive.Show();
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardRevive);
#endif
			return;
        }


		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		if (reviveOpeningCheck) // 광고 시청중 종료한 유저에 대한 필터링 변수임.
		{
			if (reward_Revive != null)
				reward_Revive.LoadAd(_request);

			PopUpController.AddAlert(AlertPopUpType.RewardAdOpeningAfterForceClose);
		}
		else // 인벤토리가 비면 광고 보상을 지급한다.
		{
			//if (Application.systemLanguage != SystemLanguage.Unknown)
			//{
			//	PopUpController.AddAlert(AlertPopUpType.Load_Fail_Ads);
			//}
			//else
			{
				ReviveController.Instance.IsActive = true;
			}

			reward_Revive = new RewardedAd(Static_APP_Config._AD_Reward_ReviveID);
			reward_Revive.OnUserEarnedReward += HandleUserEarnedRewardRevive;
			reward_Revive.OnAdClosed += HandleRewardedAdClosedRevive;
			reward_Revive.OnAdOpening += HandleOnAdOpeningRevive;
			reward_Revive.OnAdFailedToLoad += HandleOnAdFailedToLoadRevive;
			reward_Revive.LoadAd(_request);
			//LoadAd();

			//FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardRevive);
		}
	}

    public static void HandleRewardedAdSpeedClosed(object sender, EventArgs args)
    {
        if (AdPurchase)// || SmartShopController.reward_Activated)
            return;

        _rewarded_Speed = new RewardedAd(Static_APP_Config._AD_Reward_SpeedID);
        _rewarded_Speed.OnUserEarnedReward += HandleUserEarnedRewardSpeed;
        _rewarded_Speed.OnAdClosed += HandleRewardedAdSpeedClosed;
        _rewarded_Speed.OnAdOpening += HandleOnAdOpeningSpeedMode;
        _rewarded_Speed.LoadAd(_request);

        LoadAd();
    }

    public static void Show_Reward_SkipGame(bool isReStart)
	{
		_isReStart = isReStart;
		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		if (_rewarded_SkipGame != null && _rewarded_SkipGame.IsLoaded())
		{
			_rewarded_SkipGame.Show();
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardContinue);
#endif
			return;
		}

		// 고의적 네트워크 연결 차단을 통한 유저 보상을 막기 위해 체크함.
		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		if (continueModeOpeningCheck) // 광고 시청중 종료한 유저에 대한 필터링 변수임.
		{
			if (_rewarded_SkipGame != null)
				_rewarded_SkipGame.LoadAd(_request);

			PopUpController.AddAlert(AlertPopUpType.RewardAdOpeningAfterForceClose);
		}
		else // 인벤토리가 비면 광고 보상을 지급한다.
		{
			//if (Application.systemLanguage != SystemLanguage.Unknown)
			//{
			//	PopUpController.AddAlert(AlertPopUpType.Load_Fail_Ads);
			//}
			//else
            {
                // 보상 지급
                _adSkipComplete = true;
                if (_isReStart)
                    Main._main.DelayRestartSkip();
                else
                    Main._main.DelayStartSkipGame();
            }

			// 광고형 보상 셋
			_rewarded_SkipGame = new RewardedAd(Static_APP_Config._AD_Reward_SkipGameID);
			_rewarded_SkipGame.OnUserEarnedReward += HandleUserEarnedRewardSkipGame;
			_rewarded_SkipGame.OnAdClosed += HandleRewardedAdSkipGameClosed;
			_rewarded_SkipGame.OnAdOpening += HandleOnAdOpeningContinueMode;
			_rewarded_SkipGame.LoadAd(_request);
			//LoadAd();

			//FirebaseLogController.LogEvent(FirebaseLogType.Free, FirebaseLogType.RewardContinue);
		}
	}

	public static void HandleRewardedAdSkipGameClosed(object sender, EventArgs args)
	{
		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		_rewarded_SkipGame = new RewardedAd(Static_APP_Config._AD_Reward_SkipGameID);
		_rewarded_SkipGame.OnUserEarnedReward += HandleUserEarnedRewardSkipGame;
		_rewarded_SkipGame.OnAdClosed += HandleRewardedAdSkipGameClosed;
		_rewarded_SkipGame.OnAdOpening += HandleOnAdOpeningContinueMode;
		_rewarded_SkipGame.LoadAd(_request);

		//LoadAd();
	}

    public static void HandleUserEarnedRewardSpeed(object sender, Reward args)
    {
        if (AdPurchase)// || SmartShopController.reward_Activated)
            return;

        Debug.Log("HandleUserEarnedRewardSpeed reward");

        AdReward = true;
        //InGameController._instance._shooter.SetBallSpeed(2, true);
        InGameController._instance._shooter.AdRewardSpeedModeTurn = SpeedMode_Config.SpeedModeRewardTurn;
        InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;

        speedModeOpeningCheck = false;
#if UNITY_ANDROID
        FirebaseLogController.LogEvent(FirebaseLogType.Success, FirebaseLogType.RewardSpeedMode);
#endif
    }

    public static void HandleUserEarnedRewardSkipGame(object sender, Reward args)
	{
		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		_adSkipComplete = true;

		if (_isReStart)
			Main._main.handleUserEarnedRewardSkipGame = "RestartSkip";
		else
			Main._main.handleUserEarnedRewardSkipGame = "StartSkipGame";

		continueModeOpeningCheck = false;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.Success, FirebaseLogType.RewardContinue);
#endif
	}

	public static void HandleUserEarnedRewardPowerMode(object sender, Reward args)
	{
		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		InGameController._instance._shooter.AdRewardPowerModeTurn = PowerMode_Config.PowerModeRewardTurn;
		InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
		powerModeOpeningCheck = false;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.Success, FirebaseLogType.RewardPowerMode);
#endif
	}

	public static void HandleUserEarnedRewardRevive(object sender, Reward args)
	{
		reviveOpeningCheck = false;

		ReviveController.Instance.IsActive = true;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.Success, FirebaseLogType.RewardRevive);
#endif
	}

	public static void HandleRewardedAdClosedRevive(object sender, EventArgs args)
	{
		isReviveClose = true;

		reward_Revive = new RewardedAd(Static_APP_Config._AD_Reward_ReviveID);
		reward_Revive.OnUserEarnedReward += HandleUserEarnedRewardRevive;
		reward_Revive.OnAdClosed += HandleRewardedAdClosedRevive;
		reward_Revive.OnAdOpening += HandleOnAdOpeningRevive;
		reward_Revive.OnAdFailedToLoad += HandleOnAdFailedToLoadRevive;
		reward_Revive.LoadAd(_request);

		//LoadAd();
	}

	public static void HandleRewardedAdClosedPowerMode(object sender, EventArgs args)
	{
		if (AdPurchase || SmartShopController.reward_Activated)
			return;

		reward_PowerMode = new RewardedAd(Static_APP_Config._AD_Reward_SkipGameID);
		reward_PowerMode.OnUserEarnedReward += HandleUserEarnedRewardPowerMode;
		reward_PowerMode.OnAdClosed += HandleRewardedAdClosedPowerMode;
		reward_PowerMode.OnAdOpening += HandleOnAdOpeningPowerMode;
		reward_PowerMode.LoadAd(_request);

		//LoadAd();
	}

    ////////////////////////////////////// reward 광고 오프닝 체크.
    public static void HandleOnAdOpeningSpeedMode(object sender, EventArgs args)
    {
        speedModeOpeningCheck = true;
#if UNITY_ANDROID
        FirebaseLogController.LogEvent(FirebaseLogType.Opening, FirebaseLogType.RewardSpeedMode);
#endif
    }

    public static void HandleOnAdOpeningPowerMode(object sender, EventArgs args)
	{
		powerModeOpeningCheck = true;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.Opening, FirebaseLogType.RewardPowerMode);
#endif
	}

	public static void HandleOnAdOpeningContinueMode(object sender, EventArgs args)
	{
		continueModeOpeningCheck = true;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.Opening, FirebaseLogType.RewardContinue);
#endif
	}

	public static void HandleOnAdOpeningRevive(object sender, EventArgs args)
	{
		reviveOpeningCheck = true;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.Opening, FirebaseLogType.RewardRevive);
#endif
	}

	public static void HandleOnAdFailedToLoadRevive(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("===== 로드 실패 : " + args.LoadAdError);
	}
}