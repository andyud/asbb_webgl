using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Static_TextConfigs
{
	public static string _ShareScore_Message_Front;
	public static string _ShareScore_Message_Rear;

	public static string _BallCount;
	public static string _Skip;

	public static string _Setting;
	public static string _Sound;
	public static string _ContactPoint;
	public static string DarkModeTitle;

	public static string PolicyTitle;

	public static string _On;
	public static string _Off;
	public static string _Store;
	public static string _Speed;
	public static string _1x;
	public static string _2x;

	public static string _HowToPlay_1;
	public static string _HowToPlay_2;
	public static string _HowToPlay_3;
	public static string _HowToPlay_4;
	public static string _HowToPlay_5;

	public static string _MarketUpdateDesc { get; private set; }
	public static string _MarketUpdateYes { get; private set; }

	public static string _ConsentTitle { get; private set; }
	public static string _ConsentDesc { get; private set; }

	public static string _ConsentYes { get; private set; }
	public static string _ConsentNo { get; private set; }

	public static string _VisitFacebook { get; private set; }
	public static string _Rating { get; private set; }
	public static string _Share { get; private set; }
	public static string _OtherGames { get; private set; }
	public static string _NewGameArrival { get; private set; }
	public static string _Play { get; private set; }
	public static string _PlaySkip { get; private set; }

	public static string _ShareMessage { get; private set; }

	public static string _Catalogue_Title { get; private set; }
	public static string _Catalogue_Popular { get; private set; }
	public static string _Catalogue_Recent { get; private set; }
	public static string _Catalogue_SlotState_Installed { get; private set; }
	public static string _Catalogue_SlotState_NotInstalled { get; private set; }
	public static string _NewCatalogue { get; private set; }
	public static string _Loading { get; private set; }

	public static string _Record_Title { get; private set; }
	public static string _Score_Title { get; private set; }
	public static string _HowToPlay_Title { get; private set; }

	public static string _GameOver_Title { get; private set; }
	public static string _FinalScore { get; private set; }
	public static string _NewRecord { get; private set; }
	public static string _ShareSnapShot { get; private set; }
	public static string _ShareScore { get; private set; }
	public static string _PostingScore { get; private set; }
	public static string _BackToTitle { get; private set; }

	public static string _RankingScore { get; private set; }

	public static string _ResetWarning { get; private set; }
	public static string _Resume { get; private set; }
	public static string _Quit { get; private set; }
	public static string _Yes { get; private set; }
	public static string _No { get; private set; }
	public static string _Close { get; private set; }
	public static string _Policy { get; private set; }
	public static string _IAP_Speed { get; private set; }
	public static string _IAP_SkipTurn { get; private set; }
	public static string _IAP_AdSpeed { get; private set; }

	public static string _RequestRating_Title { get; private set; }
	public static string _RequestRating_Comment { get; private set; }
	public static string _RequestRating_Yes { get; private set; }
	public static string _RequestRating_Later { get; private set; }
	public static string _RequestRating_No { get; private set; }

	public static string _RequestFacebook_Title { get; private set; }
	public static string _RequestFacebook_Comment { get; private set; }
	public static string _RequestFacebook_Yes { get; private set; }
	public static string _RequestFacebook_Later { get; private set; }
	public static string _RequestFacebook_No { get; private set; }

	public static string RevivePopup_Title { get; private set; }

	public static string LoginNotice_Popup_Comment { get; private set; }

	// Alert popup 창 관련 문구 //
	// SmartShop 네트워크 불안정에 따른 안내문구
	public static string Network_NotReachable_Title { get; private set; }
	public static string Network_NotReachable_Comment { get; private set; }
	// SmartShop 보상 아이템 사용 가능
	public static string Availability_SmartShopPromotionItem_Title { get; private set; }
	public static string Availability_SmartShopPromotionItem_Comment { get; private set; }
	// SmartShop 보상 아이템 만료
	public static string Expiration_SmartShopPromotionItem_Title { get; private set; }
	public static string Expiration_SmartShopPromotionItem_Comment { get; private set; }
	// SmartShop 접속 실패
	public static string WebRequest_Error_Title { get; private set; }
	public static string WebRequest_Error_Comment { get; private set; }
	// ADID 로직부 에러 문구
	public static string RequestAdvertisingIdentifierAsync_Error_Title { get; private set; }
	public static string RequestAdvertisingIdentifierAsync_Error_Comment { get; private set; }

	// ingame powerMode 관련 문구
	public static string PowerMode_LimitCount_Title { get; private set; }
	public static string PowerMode_LimitCount_Comment { get; private set; }
	public static string PowerMode_State_Moving_Title { get; private set; }
	public static string PowerMode_State_Moving_Comment { get; private set; }

	// ingame SpeedMode 관련 문구
	public static string SpeedMode_State_Moving_Title { get; private set; }
	public static string SpeedMode_State_Moving_Comment { get; private set; }

	// 리워드 광고 시청중 강제 닫기후 재 접속시 광고 미로드시 안내 팝업문구.
	public static string RewardAdOpeningAfterForceClose_Title { get; private set; }
	public static string RewardAdOpeningAfterForceClose_Comment { get; private set; }

	// 파워모드 설명 문구
	public static string PowerModeGuide_Title { get; private set; }
	public static string PowerModeGuide_Comment { get; private set; }

	// 스피드모드 설명 문구
	public static string SpeedModeGuide_Title { get; private set; }
	public static string SpeedModeGuide_Comment { get; private set; }

	// Restore 관련 문구
	public static string RestoreSuccess_Title { get; private set; }
	public static string RestoreSuccess_Comment { get; private set; }

	// Restore 관련 문구.
	public static string RestoreFail_Title { get; private set; }
	public static string RestoreFail_Comment { get; private set; }

	// 네트워크 미연결시..
	public static string NotConnectionNetwork_Title { get; private set; }
	public static string NotConnectionNetwork_Comment { get; private set; }

	// FCM 인입 유저 보상 관련
	public static string FCM_RewardItem_Title { get; private set; }
	public static string FCM_RewardItem_Comment { get; private set; }

	// 업데이트 무료 보상 지급 1회성
	public static string Update_RewardItem_Title { get; private set; }
	public static string Update_RewardItem_Comment { get; private set; }

	// 영수증 중복 오류 메시지
	public static string Duplicate_Receipt_Title { get; private set; }
	public static string Duplicate_Receipt_Comment { get; private set; }

	// iap 정보 로딩중일때
	public static string IAP_Loading_Title { get; private set; }
	public static string IAP_Loading_Comment { get; private set; }

	public static string Failed_RankingScore_Title { get; private set; }
	public static string Failed_RankingScore_Commnet { get; private set; }

	// login 관련
	public static string Login_Success_Title { get; private set; }
	public static string Login_Success_Comment { get; private set; }
	public static string Login_Failed_Title { get; private set; }
	public static string Login_Failed_Comment { get; private set; }
	public static string Login_FirebaseAuthFailed_Title { get; private set; }
	public static string Login_FirebaseAuthFailed_Comment { get; private set; }
	public static string Login_Title { get; private set; }
	public static string Login_description { get; private set; }

	// 랭킹 관련
	public static string Expired_Ranking_Title { get; private set; }
	public static string Expired_Ranking_Comment { get; private set; }

	// 강제 업데이트
	public static string Force_Update_Title { get; private set; }
	public static string Force_Update_Comment { get; private set; }

	// 권장 업데이트
	public static string Recommended_Update_Title { get; private set; }
	public static string Recommended_Update_Comment { get; private set; }

	// 광고 로드 실패
	public static string AdsLoad_Fail_Title { get; private set; }
	public static string AdsLoad_Fail_Comment { get; private set; }


	// Alert popup 창 관련 문구 //

	public static string Btn_Retry { get; private set; }
	public static string Btn_OK { get; private set; }
	public static string Btn_Restore { get; private set; }
	public static string Btn_AdView { get; private set; }
	public static string Btn_Revive { get; private set; }

	///// title scene /////
	public static string TitleScene_BackUp_Title { get; private set; }
	// title scene


	///// Popup_Login /////

	// backup 실패시
	public static string BackupFail_Title { get; private set; }
	public static string BackupFail_Comment { get; private set; }
	// backup 성공시
	public static string BackupSuccess_Title { get; private set; }
	public static string BackupSuccess_Comment { get; private set; }
	// logout 성공시
	public static string LogOutSuccess_Title { get; private set; }
	public static string LogOutSuccess_Comment { get; private set; }
	// logout 실패시
	public static string LogOutFail_Title { get; private set; }
	public static string LogOutFail_Comment { get; private set; }
	// 로그인 상태일때
	public static string StateIsSignIn_Title { get; private set; }
	public static string StateIsSignIn_Comment { get; private set; }
	// 로그아웃 상태일때
	public static string StateIsSignOut_Title { get; private set; }
	public static string StateIsSignOut_Comment { get; private set; }

	// apple 로그인 미지원 os 일때
	public static string NotSupported_Title { get; private set; }
	public static string NotSupported_Comment { get; private set; }

	public static string LoginPopup_BestScore_Title { get; private set; }
	public static string LoginPopup_IAP_Title { get; private set; }
	public static string LoginPopup_IAP_Purchased { get; private set; }
	public static string LoginPopup_IAP_NonPurchase { get; private set; }
	public static string LoginPopup_BackUpDate_Title { get; private set; }
	public static string Btn_GoogleLogin { get; private set; }
	public static string Btn_AppleLogin { get; private set; }
	public static string Btn_Logout { get; private set; }
	public static string LoginPopup_InactiveLogin_Description_Message { get; private set; }

	// Popup Login

	// Popup MoreGaems
	public static string MoreGamesPopup_Title { get; private set; }
	public static string MoreGamesPopup_MoreBtnLabel { get; private set; }
	public static string MoreGamesPopup_LoadFailLabel { get; private set; }

	// Popup Ios IDFA
	public static string IosIDFA_Title { get; private set; }
	public static string IosIDFA_Message { get; private set; }
	public static string IosIDFA_Next { get; private set; }

	// 랭킹 버튼
	public static string Btn_Ranking { get; private set; }

	// 점수 공유하기
	public static string Share_Subject { get; private set; }
	public static string Share_Text { get; private set; }

	public static void Initialize(SystemLanguage language)
	{
        if (language == SystemLanguage.Korean)
            Initialize_Text_Korean();
        else
            Initialize_Text_English();
	}
	static void Initialize_Text_English()
	{
		_ShareScore_Message_Front = "I got";
		_ShareScore_Message_Rear = "score on SWIPE BRICK BREAKER. Let's play with me!";

		_BallCount = "Ball x";
		_Skip = "SKIP";

		_Setting = "SETTING";
		_Sound = "SOUND";
		_ContactPoint = "AIMING";
		DarkModeTitle = "DarkMode";

		PolicyTitle = "Policy";

		_On = "ON";
		_Off = "OFF";
		_Store = "Store";
		_Speed = "Speed";
		_1x = "x1";
		_2x = "x2";

		_HowToPlay_1 = "Swipe to shoot balls to break the bricks.";
		_HowToPlay_2 = "When ball hits the brick, durability is reduced.\nWhen durability reduce to 0, brick breaks.";
		_HowToPlay_3 = "Get the green circle to increase the number of balls.";
		_HowToPlay_4 = "When the bricks reach the bottom line, game is over.";
		_HowToPlay_5 = "Challenge\nto make your high score!";

		_MarketUpdateDesc = "Please update to the\nlatest version.";
		_MarketUpdateYes = "Update";

		_ConsentTitle = "Personal Information Collection Consent Information";
		_ConsentDesc = "We collect the following information for service use.\n-Purpose of Use: Service Use\n-Collection Information: ADID (Advertisement Identifier), installed application\nThe collected information is collected through API password and communication.\nYou may refuse to consent, and if you refuse to consent, there may be restrictions on the use of the service.";
		_ConsentYes = "Agreed";
		_ConsentNo = "Reject";

		Btn_Revive = "Revive";
		Btn_AdView = "Ad View";
		Btn_Restore = "Restore";
		Btn_OK = "OK";
		Btn_Retry = "Retry";
		Btn_Logout = "Log Out";
		Btn_AppleLogin = "Apple Login";
		Btn_GoogleLogin = "Google Login";
		Btn_Ranking = "Ranking";

		Force_Update_Title = "Update is Available";
		Force_Update_Comment = "To continue to use\nthe Swipe Brick Breaker app,\nYou must update your app\nto the latest version.";

		Recommended_Update_Title = "New version is Available";
		Recommended_Update_Comment = "Swipe Brick Breaker update\nversion is available. Click\nbelow for download.";

		Expired_Ranking_Title = "Weekly(Monthly) Ranking Unavailable Notification";
		Expired_Ranking_Comment = "If the game start date and end date are recorded in different weeks(monthly), you cannot participate in the weekly(monthly) ranking.";

		Share_Subject = "Original SwipeBrickBreaker";
		Share_Text = "Try it right now!";

		Login_Title = "LogIn";
		Login_description = "When you log in, your game records are saved\nYou can participate in the rankings.";

		Login_FirebaseAuthFailed_Title = "Reboot Guide";
		Login_FirebaseAuthFailed_Comment = "Please restart the app\nafter completely closing it";

		Login_Success_Title = "Login Succeed";
		Login_Success_Comment = "You are logged in";

		Login_Failed_Title = "Login Failed";
		Login_Failed_Comment = "You are login failed";

		IAP_Loading_Title = "Loading payment details..";
		IAP_Loading_Comment = "Please try again in a few minutes.";

		Failed_RankingScore_Title = "Failed to register ranking score";
		Failed_RankingScore_Commnet = "Ranking score not registered\nPlease restart";

		Duplicate_Receipt_Title = "Duplicate Receipt";
		Duplicate_Receipt_Comment = "This is an already registered receipt.";

		Update_RewardItem_Title = "Update Audit Event";
		Update_RewardItem_Comment = "Update users will receive\n{0} speed/power items\nas a gift!";

		LoginPopup_InactiveLogin_Description_Message = "Login is required";
		LoginPopup_BackUpDate_Title = "Backup Date: {0}";
		LoginPopup_IAP_NonPurchase = "There are no purchased items";
		LoginPopup_IAP_Purchased = "Purchase Completed";
		LoginPopup_IAP_Title = "In-app Payment History";
		LoginPopup_BestScore_Title = "Highest Score: {0}";

		TitleScene_BackUp_Title = "Backup and Recovery";

		StateIsSignOut_Title = "Logout Status Notification";
		StateIsSignOut_Comment = "You are currently logged out";

		NotSupported_Title = "Unsupported Information";
		NotSupported_Comment = "The software version is not supported.";

		LogOutFail_Title = "Logout Failure Notification";
		LogOutFail_Comment = "Logout failed\nPlease try again later";

		LogOutSuccess_Title = "Logout Notification";
		LogOutSuccess_Comment = "Logout has been processed normally";

		BackupSuccess_Title = "Backup And Recovery Success Notifications";
		BackupSuccess_Comment = "Successful backup and recovery";

		BackupFail_Title = "Backup And Recovery Failure Notification";
		BackupFail_Comment = "Backup and recovery failed\nPlease try again later";

		RevivePopup_Title = "Only one chance\nto revive!";

		FCM_RewardItem_Title = "Surprise Item Event";
		FCM_RewardItem_Comment = "To send a notification message\nSpeed / power item {0} times\nEven as a gift!";

		NotConnectionNetwork_Title = "Network Connection Failure";
		NotConnectionNetwork_Comment = "Please check your network connection,\nand try again.";

		RestoreSuccess_Title = "Recovery Notification";
		RestoreSuccess_Comment = "Purchase history\nrestored successfully.";

		RestoreFail_Title = "Recovery Failure Notification";
		RestoreFail_Comment = "Recover Purchase History\nFailed.\nPlease try again later.";

		PowerModeGuide_Title = "Power mode item exhaustion notification";
		PowerModeGuide_Comment = "Charge 100 items\nto shorten the game time!";

		SpeedModeGuide_Title = "Speed mode item exhaustion notification";
		SpeedModeGuide_Comment = "Charge 100 items\nthat increase the speed of the ball!";

		RewardAdOpeningAfterForceClose_Title = "Alert!";
		RewardAdOpeningAfterForceClose_Comment = "You just requested an advertisement.\nPlease try again later.";

		SpeedMode_State_Moving_Title = "Speed Mode";
		SpeedMode_State_Moving_Comment = "Can't be set while the ball is moving";

		PowerMode_LimitCount_Title = "Power mode usage restrictions";
		PowerMode_LimitCount_Comment = string.Format("Power mode can be used\nwhen the number of balls is {0} or more.", PowerMode_Config.PowerModeLimitCount);
		PowerMode_State_Moving_Title = "Power Mode";
		PowerMode_State_Moving_Comment = "Can't be set while the ball is moving";

		Network_NotReachable_Title = "Network Connection Failed";
		Network_NotReachable_Comment = "Please check your network\nconnection You may affect the game\nif you are not connected.";

		Availability_SmartShopPromotionItem_Title = "Reward Item Availability Guide";
		Availability_SmartShopPromotionItem_Comment = "Expired Date of Use of\nPeriodic Reward Items\n{0}";

		Expiration_SmartShopPromotionItem_Title = "Reward Item Expiration Notice";
		Expiration_SmartShopPromotionItem_Comment = "Your term reward item has expired.\nThank you.";

		WebRequest_Error_Title = "Network Communication Failure";
		WebRequest_Error_Comment = "Network communication is unstable.\nPlease try again later.";

		RequestAdvertisingIdentifierAsync_Error_Title = "Data Communication Failure";
		RequestAdvertisingIdentifierAsync_Error_Comment = "Data communication is unstable.\nPlease try again later.";

		_VisitFacebook = "Find Us on Facebook";
		_Rating = "Rate and Comment";
		_Share = "Share with Friends";
		_OtherGames = "Enjoy Other Games";
		_NewGameArrival = "New Game Arrival!";
		_Play = "PLAY";
		_PlaySkip = "{0} PLAY";

		_ShareMessage = "Let's play together!";

		_Catalogue_Title = "NEW ARRIVAL";
		_Catalogue_Popular = "POPULAR";
		_Catalogue_Recent = "RECENT";
		_Catalogue_SlotState_Installed = "PLAY\nNOW";
		_Catalogue_SlotState_NotInstalled = "FREE\nGET";
		_NewCatalogue = "NEW!";
		_Loading = "Loading";

		_HowToPlay_Title = "HOW TO PLAY";
		_Record_Title = "RECORD : ";
		_Score_Title = "SCORE : ";

		_GameOver_Title = "GAME OVER";
		_FinalScore = "Final Record";
		_RankingScore = "Earned Score";
		_NewRecord = "New Record!";
		_ShareSnapShot = "Share SnapShot";
		_ShareScore = "share my score";
		_PostingScore = "Post to Leaderboard";
		_BackToTitle = "BACK TO TITLE";

		_ResetWarning = "(Game will be reset.)";
		_Resume = "RESUME";
		_Quit = "QUIT?";
		_Yes = "YES";
		_No = "NO";
		_Close = "CLOSE";

		if (Application.platform == RuntimePlatform.Android)
			_Policy = "[000000]Refund policy is subject to Google policy.[-] [0054FF]here[-]";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			_Policy = "[000000]Refund policy is subject to Apple policy.[-] [0054FF]here[-]";
		else
			_Policy = string.Empty;

		_IAP_Speed = "Power / Speed mode\n\nThe power and speed of the ball increases,\nso you can enjoy the game with speed.";
		_IAP_SkipTurn = "Save Score\n\nSave every 100 points without advertisement";
		_IAP_AdSpeed = "Power + Speed + Save Score\n\nGet rid of all advertisement";

		_RequestRating_Title = "WILL YOU RATE US?";
		_RequestRating_Comment = "We chack\nall of your rate and comment.";
		_RequestRating_Yes = "YES";
		_RequestRating_Later = "Later";
		_RequestRating_No = "No";

		_RequestFacebook_Title = "LIKE US ON FACEBOOK";
		_RequestFacebook_Comment = "We release game every month.";
		_RequestFacebook_Yes = "YES";
		_RequestFacebook_Later = "Later";
		_RequestFacebook_No = "No";

		MoreGamesPopup_Title = "Various recommended games of Nine Games";
		MoreGamesPopup_MoreBtnLabel = "Check out other games now!";
		MoreGamesPopup_LoadFailLabel = "Failed to load data.\nPlease run the app again!";

		IosIDFA_Title = "Please allow tracking in your\n device's privacy settings!";
		IosIDFA_Message = "Your personal data will not\n be collected and only be used\n to provide optimized content.";
		IosIDFA_Next = "Next";

		LoginNotice_Popup_Comment = "You will be able to retrieve your scores and purchase history if you login in the new game. To login, click the login button on the main screen.\n\nIf there is any question, don't hesitate to contact us. (hailey0hailey0@gmail.com)";


		AdsLoad_Fail_Title = "Ads failed to load";
		AdsLoad_Fail_Comment = "Your ad is not ready. Please try again in a few seconds.";
	}

	static void Initialize_Text_Korean()
	{
		_ShareScore_Message_Front = "스와이프 벽돌깨기에서";
		_ShareScore_Message_Rear = "점을 달성했어요. 함께 플레이해요~";

		_BallCount = "공 x";
		_Skip = "턴 넘기기";

		_Setting = "환경설정";
		_Sound = "사운드";
		_ContactPoint = "조준점";
		DarkModeTitle = "다크모드";

		PolicyTitle = "개인정보 처리방침";

		_On = "ON";
		_Off = "OFF";
		_Store = "상점";
		_Speed = "속도";
		_1x = "x1";
		_2x = "x2";

		_HowToPlay_1 = "스와이프로 공을 날려 벽돌을 파괴하세요.";
		_HowToPlay_2 = "공이 부딪히면 벽돌의 내구도가 감소합니다.\n내구도가 0이 되면 벽돌이 파괴됩니다.";
		_HowToPlay_3 = "녹색 원을 획득하면 공의 수가 늘어납니다.";
		_HowToPlay_4 = "하단 라인까지 벽돌이 내려오면 게임오버 됩니다.";
		_HowToPlay_5 = "최고 점수에 도전해보세요!";

		_MarketUpdateDesc = "최신 버전으로\n업데이트 해주세요.";
		_MarketUpdateYes = "업데이트";

		_ConsentTitle = "개인정보 수집 동의 안내";
		_ConsentDesc = "서비스 이용을 위해 아래와 같은 정보를 수집합니다.\n-이용목적 : 서비스이용\n-수집정보 : ADID(광고식별자), 설치된 애플리케이션\n수집된 정보는 API 암호와 통신으로 수집됩니다.\n동의를 거부하실 수 있으며, 동의 거부 시 서비스 이용이\n제한이 있을 수 있습니다.";
		_ConsentYes = "동의";
		_ConsentNo = "거부";

		Btn_Revive = "부활";
		Btn_AdView = "광고 보기";
		Btn_Restore = "복구";
		Btn_OK = "확인";
		Btn_Retry = "재시도";
		Btn_Logout = "로그아웃";
		Btn_AppleLogin = "애플 로그인";
		Btn_GoogleLogin = "구글 로그인";
		Btn_Ranking = "순위 보기";

		Force_Update_Title = "필수 업데이트 알림";
		Force_Update_Comment = "원활한 스와이프 벽돌깨기 앱 이용을\n위해서는 최신 버전으로 업데이트가\n필요합니다.";

		Recommended_Update_Title = "최신버전 업데이트";
		Recommended_Update_Comment = "앱의 최신버전이 등록되었습니다.\n최신버전으로\n업데이트 하시겠습니까?";

		Expired_Ranking_Title = "주간(월간)랭킹 참여불가 알림";
		Expired_Ranking_Comment = "게임 시작일자와 종료일자가 다른 주간(월간)에 기록되면 주간(월간) 랭킹에 참여할 수 없습니다.";

		Share_Subject = "스와이프 벽돌깨기";
		Share_Text = "지금 바로 도전하세요!";

		Login_Title = "로그인";
		Login_description = "로그인 시, 게임 기록이 저장되며\n랭킹에 참여할 수 있습니다.";

		Login_FirebaseAuthFailed_Title = "재부팅 안내";
		Login_FirebaseAuthFailed_Comment = "앱 완전 종료후 재 실행 해주세요";

		Login_Success_Title = "로그인 성공";
		Login_Success_Comment = "로그인 되었습니다.";

		Login_Failed_Title = "로그인 실패";
		Login_Failed_Comment = "로그인에 실패 하였습니다.";

		IAP_Loading_Title = "결제 정보 로드중..";
		IAP_Loading_Comment = "잠시 후 다시 시도해 주세요.";

		Failed_RankingScore_Title = "랭킹 점수 등록 실패";
		Failed_RankingScore_Commnet = "랭킹 점수가 등록되지 않았습니다\n재시작 해주세요";

		Duplicate_Receipt_Title = "중복 영수증";
		Duplicate_Receipt_Comment = "이미 등록된 영수증 입니다.";

		Update_RewardItem_Title = "업데이트 감사 이벤트";
		Update_RewardItem_Comment = "업데이트 유저분께 \n스피드/파워 아이템 {0}회를\n선물로 드립니다!";

		LoginPopup_InactiveLogin_Description_Message = "로그인이 필요합니다";
		LoginPopup_BackUpDate_Title = "백업 일자: {0}";
		LoginPopup_IAP_NonPurchase = "구매한 아이템이 없습니다.";
		LoginPopup_IAP_Purchased = "구매 완료";
		LoginPopup_IAP_Title = "인앱결제 내역";
		LoginPopup_BestScore_Title = "최고 점수: {0}";

		TitleScene_BackUp_Title = "백업 및 복구 하기";

		StateIsSignOut_Title = "로그아웃 상태 알림";
		StateIsSignOut_Comment = "현재 로그아웃 상태입니다.";

		NotSupported_Title = "미지원 안내";
		NotSupported_Comment = "지원하지 않는 소프트웨어 버전 입니다.";

		StateIsSignIn_Title = "로그인 상태중 알림";
		StateIsSignIn_Comment = "현재 로그인 상태 입니다.";

		LogOutFail_Title = "로그아웃 실패 알림";
		LogOutFail_Comment = "로그아웃 실패 하였습니다.\n잠시 후 다시 시도해 주세요.";

		LogOutSuccess_Title = "로그아웃 알림";
		LogOutSuccess_Comment = "로그아웃이 정상 처리 되었습니다.";

		BackupSuccess_Title = "백업 및 복구 성공 알림";
		BackupSuccess_Comment = "백업 및 복구 성공 하였습니다.";

		BackupFail_Title = "백업 및 복구 실패 알림";
		BackupFail_Comment = "백업 및 복구 실패 하였습니다.\n잠시 후 다시 시도해 주세요.";

		RevivePopup_Title = "부활할 수 있는\n단 한 번의 기회!";

		FCM_RewardItem_Title = "깜짝 아이템 이벤트";
		FCM_RewardItem_Comment = "알림 메시지를 통해 접속하신 유저분께\n스피드/파워 아이템 {0}회를\n선물로 드립니다!";

		NotConnectionNetwork_Title = "네트워크 연결 실패";
		NotConnectionNetwork_Comment = "네트워크 연결 상태를 확인하고,\n다시 시도해 주세요.";

		RestoreSuccess_Title = "복구 알림";
		RestoreSuccess_Comment = "구매 내역이\n정상 복구되었습니다.";

		RestoreFail_Title = "복구 실패 알림";
		RestoreFail_Comment = "구매 내역 복구\n실패하였습니다.\n잠시 후 다시 시도해 주세요.";

		PowerModeGuide_Title = "파워모드 아이템 소진 알림";
		PowerModeGuide_Comment = "게임 시간을 단축시킬 수 있는\n아이템 100개 충전!";

		SpeedModeGuide_Title = "스피드 모드 아이템 소진 알림";
		SpeedModeGuide_Comment = "공의 스피드를 올려주는\n아이템 100개 충전!";

		RewardAdOpeningAfterForceClose_Title = "잠깐!";
		RewardAdOpeningAfterForceClose_Comment = "현재 불러올 수 있는 광고가 없습니다.\n잠시 후 다시 시도해 주세요.";

		SpeedMode_State_Moving_Title = "스피드 모드";
		SpeedMode_State_Moving_Comment = "공이 움직이는 중에는 설정할 수 없습니다";

		PowerMode_LimitCount_Title = "파워 모드 사용 제한";
		PowerMode_LimitCount_Comment = string.Format("파워 모드는 공 개수 {0}개\n 이상일 때 사용 가능합니다.", PowerMode_Config.PowerModeLimitCount);
		PowerMode_State_Moving_Title = "파워 모드";
		PowerMode_State_Moving_Comment = "공이 움직이는 중에는 설정할 수 없습니다";

		Network_NotReachable_Title = "네트워크 연결 실패";
		Network_NotReachable_Comment = "네트워크 연결 상태를 확인하세요\n네트워크 미 연결 시\n게임에 영향을 줄 수 있습니다.";

		Availability_SmartShopPromotionItem_Title = "보상 아이템 사용 가능 안내";
		Availability_SmartShopPromotionItem_Comment = "기간제 보상 아이템 사용 만료 일자\n{0}";

		Expiration_SmartShopPromotionItem_Title = "보상 아이템 만료 안내";
		Expiration_SmartShopPromotionItem_Comment = "기간제 보상 아이템 사용 기한이\n만료되었습니다\n감사합니다.";

		WebRequest_Error_Title = "네트워크 통신 장애";
		WebRequest_Error_Comment = "네트워크 통신 상태가 불안정합니다.\n잠시 후 다시 시도해 주세요.";

		RequestAdvertisingIdentifierAsync_Error_Title = "데이터 통신 장애";
		RequestAdvertisingIdentifierAsync_Error_Comment = "데이터 통신 상태가 불안정합니다.\n잠시 후 다시 시도해 주세요.";

		_VisitFacebook = "페이스북 방문하기";
		_Rating = "평점과 리뷰 남기기";
		_Share = "친구에게 소개하기";
		_OtherGames = "다른 게임 확인하기";
		_NewGameArrival = "새 게임이 도착했어요!";
		_Play = "시작";
		_PlaySkip = "{0} 부터 시작";

		_ShareMessage = "함께 플레이해요~";

		_Catalogue_Title = "신작 출시";
		_Catalogue_Popular = "인기순";
		_Catalogue_Recent = "최신순";
		_Catalogue_SlotState_Installed = "실행";
		_Catalogue_SlotState_NotInstalled = "받기";
		_NewCatalogue = "NEW!";
		_Loading = "로딩중";

		_HowToPlay_Title = "게임 방법";
		_Record_Title = "최고기록 : ";
		_Score_Title = "현재점수 : ";

		_GameOver_Title = "게임오버";
		_FinalScore = "최종 기록";
		_RankingScore = "획득 점수";
		_NewRecord = "신기록 달성!";
		_ShareSnapShot = "스냅샷 공유";
		_ShareScore = "내 점수 공유하기";
		_PostingScore = "리더보드에 등록";
		_BackToTitle = "메인화면으로";

		_ResetWarning = "(게임이 처음부터 시작됩니다.)";
		_Resume = "계속하기";
		_Quit = "나가기";
		_Yes = "네";
		_No = "아니오";
		_Close = "닫기";

		if (Application.platform == RuntimePlatform.Android)
			_Policy = "[000000]환불 정책은 구글 정책에 따릅니다.[-] [0054FF]여기[-]";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			_Policy = "[000000]환불 정책은 애플 정책에 따릅니다.[-] [0054FF]여기[-]";
		else
			_Policy = string.Empty;

		_IAP_Speed = "파워/스피드 모드\n\n공의 파워와 스피드업으로\n속도감 있는\n게임을 즐길 수 있어요";

		_IAP_SkipTurn = "이어하기\n\n100점 단위로 저장되는\n이어하기 기능을\n광고 없이 즐기세요";
		_IAP_AdSpeed = "파워+스피드+이어하기\n\n모든 기능을\n광고 없이 즐기세요";

		_RequestRating_Title = "평점을 남겨주세요.";
		_RequestRating_Comment = "우리는 모든 평점과 리뷰를 확인합니다.";
		_RequestRating_Yes = "네";
		_RequestRating_Later = "나중에";
		_RequestRating_No = "아니오";

		_RequestFacebook_Title = "페이스북을 방문해주세요.";
		_RequestFacebook_Comment = "우리는 매월 게임을 출시하고 있습니다.";
		_RequestFacebook_Yes = "네";
		_RequestFacebook_Later = "나중에";
		_RequestFacebook_No = "아니오";

		MoreGamesPopup_Title = "Nine Games의 다양한 추천 게임";
		MoreGamesPopup_MoreBtnLabel = "다른 게임들도 지금 확인해보세요!";
		MoreGamesPopup_LoadFailLabel = "데이터를 불러오는데 실패했습니다.\n앱을 다시 실행해주세요!";

		IosIDFA_Title = "기기의 설정 > 개인 정보 보호에서\n 추적을 허용해주세요.";
		IosIDFA_Message = "개인정보는 수집되지 않으며\n 앱 내 최적의 컨텐츠 추천에 활용됩니다.";
		IosIDFA_Next = "다음";

		LoginNotice_Popup_Comment = "새 게임에 로그인하면 점수와 구매 내역을 검색할 수 있습니다. 로그인하려면 메인 화면에서 로그인 버튼을 클릭하세요.\n\n궁금한 점이 있으면 언제든지 문의해 주세요. (hailey0hailey0@gmail.com)";

		AdsLoad_Fail_Title = "광고 로드 실패";
		AdsLoad_Fail_Comment = "광고가 준비되지 않았습니다. 잠시 후 다시 시도해주세요.";
	}
}
