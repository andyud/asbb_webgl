using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Static_APP_Config
{
	public static string _Client_Secret { get; private set; }

	public static string _PackageName { get; private set; }
	
	public static string _Market_URL { get; private set; }
	public static string _Policy_URL { get; private set; }
	public static string _Refund_URL { get; private set; }

	public static string _LeaderboardID { get; private set; }

	public static string _SaveDataPath_Internal { get; private set; }
	public static string _SaveDataPath_External { get; private set; }

	public static int _playCount_Interstitial = 2;
	public static int _playCount_Rating = 10;
	public static int _playCount_FacebookVisit = 20;

	public static int _interstitial_MinimumScore = 12;

	public static float _interstitialChance_Min = 0.0f;
	public static float _interstitialChance_Max = 0.5f;
	public static float _interstitialChance_Cooltime = 0.5f * 60f;
	public static float _interstitialChange_PickTime = 1.5f * 60f;

	public static string _AD_AppID { get; private set; }
	public static string _AD_BannerID { get; private set; }
	public static string _AD_InterstitialID { get; private set; }
	public static string _AD_Reward_SpeedID { get; private set; }
	public static string _AD_Reward_SkipGameID { get; private set; }
	public static string _AD_Reward_PowerModeID { get; private set; }
	public static string _AD_Reward_ReviveID { get; private set; }

	public static void Initialize()
    {
        _Client_Secret = "HpPZGmU7EGWHsU9JfMfzAcV2jjSfvQJXUBwDU7pOR29lTRRFTHTIIfHvsHZmYhXKTgMXBiQ95QgeF28nGSwOLptNxc0HN7VybDUJhRFjhH0PW4TxZBFW4s8zI2wmo";

		if (Application.platform == RuntimePlatform.Android)
		{
			_PackageName = "com.trochoi.swipebrick";

			// Google Play Store
			_Market_URL = "https://play.google.com/store/apps/details?id=com.trochoi.swipebrick&pli=1";
			_Policy_URL = "https://ddobbokki.com/policy/com.trochoi.swipebrick";
			_Refund_URL = "https://support.google.com/googleplay/answer/2479637";

			_LeaderboardID = "";

			_SaveDataPath_Internal = "/data/data/" + _PackageName + "/files/";
			_SaveDataPath_External = Application.persistentDataPath + "/";

			// Å×½ºÆ® ±¤°í ID
            _AD_BannerID = "ca-app-pub-3940256099942544/6300978111";
            _AD_InterstitialID = "ca-app-pub-3940256099942544/1033173712";
            _AD_Reward_SpeedID = "ca-app-pub-3940256099942544/5224354917";
            _AD_Reward_PowerModeID = "ca-app-pub-3940256099942544/5224354917";
            _AD_Reward_ReviveID = "ca-app-pub-3940256099942544/5224354917";
            _AD_Reward_SkipGameID = "ca-app-pub-3940256099942544/5224354917";

			// ½Ç±¤°í ID
			_AD_BannerID = "ca-app-pub-9847016069318687/5994428329";// 
			_AD_InterstitialID = "ca-app-pub-9847016069318687/2020396435";// 
			_AD_Reward_SpeedID = "ca-app-pub-9847016069318687/7320824572";// 
			_AD_Reward_PowerModeID = "ca-app-pub-9847016069318687/1497652258";// 
			_AD_Reward_ReviveID = "ca-app-pub-9847016069318687/3025236781";// 
			_AD_Reward_SkipGameID = "ca-app-pub-9847016069318687/1330131293";// 
		}
		else
		{
			_Market_URL = "";

			_LeaderboardID = "";

			_SaveDataPath_Internal = Application.dataPath + "/";
			_SaveDataPath_External = Application.dataPath + "/";
		}
	}
}
