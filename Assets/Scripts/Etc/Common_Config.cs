
using UnityEngine;

public class PlayerPrefs_Config
{
	// PowerMode
	public const string AdRewardPowerModeTurn = "AdRewardPowerModeTurn";
	public const string ActivePowerMode = "ActivePowerMode";

	// speedMode
	public const string AdRewardSpeedModeTurn = "AdSpeedTurn"; // 기존 유저에 연결되어 있는 키값이라 변경이 힘듬.
	public const string ActiveSpeedMode = "AdReward"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬. 230417-> //0: base, 1: *2, 2: *4 (기본값 1)

	// IAP
	public const string Purchase_Speed = "SpeedPurchase"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬.
	public const string Purchase_Continue = "SkipTurnPurchase"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬.
	public const string Purchase_Total = "AdSpeedPurchase"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬.

	// Before App IAP
	public const string BeforeApp_Purchase_Speed = "BeforeAppSpeedPurchase"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬.
	public const string BeforeApp_Purchase_Continue = "BeforeAppSkipTurnPurchase"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬.
	public const string BeforeApp_Purchase_Total = "BeforeAppAdSpeedPurchase"; //기존 유저에 연결되어 있는 키값이라 변경이 힘듬.

	//public const string Purchase_Speed_Receipt = "Purchase_Speed_Receipt";
	//public const string Purchase_Continue_Receipt = "Purchase_Continue_Receipt";
	//public const string Purchase_Total_Receipt = "Purchase_Total_Receipt";

	// SmartShop Notice
	public const string ExpiredSmartShopNoticeSkipData = "ExpiredSmartShopNoticeSkipDate";

	// FCM Item Reward Count
	public const string FCMRewardCount = "FCMRewardCount";

	// 스마트 샵 구매 유저에 대한 기간제 보상 아이템
	public const string SmartShopRewardExpiredDate = "SmartShopRewardExpiredDate";

	// 소생 카운트
	public const string ReviveCount = "ReviveCount";

	// Update Event First Reward
	public const string UpdateFirstOpen = "UpdateFirstOpen";

	public const string BackupUpdateDateTime = "BackupUpdateDateTime";

	//Ranking Start Score
	public const string StartRankingScore = "StartRankingScore";
	public const string EndRankingScore = "EndRankingScore";
	public const string StartRankingScore_Success = "StartRankingScore_Success";

	// web notice popup => do not open today
	public const string TodayClose = "TodayClose";

	// 오늘 하루 다시보지 않기 관련 시간값 저장
	public const string TodayCloseTime = "TodayCloseTime";

	public const string Step_Speed = "Step_Speed";
	public const string SetStateSpeedMode = "SetStateSpeedMode";
}

public class RemoteConfigKey
{
	public const string IsReviveFeature = "IsReviveFeature";
	public const string IsBackupFeature = "IsBackUpFeature";
	public const string MoreGames = "MoreGames";
}

public class Revive
{
	public const int ReviveCount = 1;
	public const int DeleteLineCount = 5;
}

public class PowerMode_Config
{
	public const int PowerModeLimitCount = 0;
	public const int PowerModeRewardTurn = 100;
	public const int PowerModeMinPowerBallCount = 2;
	public const int PowerModeMaxPowerBallCount = 250;

	// 앱 최초 설치 실행시 보상 및 업데이트 보상 (1회성)
	public const int FirstRunFreeRewardCount = 30;
}

public class SpeedMode_Config
{
	public const int SpeedModeRewardTurn = 100;

	// 볼 스피드 변수
	public const int ActiveValue_4 = 4;
	public const int ActiveValue = 2;
	public const int InactiveValue = 1;

	// 볼 발사 간격 프레임.
	public const int ActiveFrameGap_4 = 2;
	public const int ActiveFrameGap = 3;
	public const int InactiveFrameGap = 4;

	// 앱 최초 설치 실행시 보상 및 업데이트 보상 (1회성)
	public const int FirstRunFreeRewardCount = 30;
}

public class Ball_Config
{
	public const int BallDefault = 100;
	public const int BallMaximum = 500;
}

public class StaticMethod
{
	public static bool IsPurchase_Speed_Total_SmartShopReward()
	{
		if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Speed, 0) == 1 ||
			PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 1 ||
			PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 0) == 1 ||
			PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) == 1 ||
			SmartShopController.reward_Activated)
		{
			return true;
		}
		else
			return false;
	}

	public static bool IsPurchase_SkipGame_Total_SmartShopReward()
	{
		if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Continue, 0) == 1 ||
		   PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 1 ||
		   PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 0) == 1 ||
		   PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) == 1 ||
		   SmartShopController.reward_Activated)
		{
			return true;
		}
		else
			return false;
	}
}