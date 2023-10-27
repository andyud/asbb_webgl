#if UNITY_ANDROID
using Firebase;
using System.Text;
using UnityEngine;

public class FirebaseLogController
{
	private static StringBuilder stringBuilder = new StringBuilder();

	public static void LogEvent(params string[] parameters)
	{
#if UNITY_ANDROID
		if (FireBaseController.dependencyStatus != DependencyStatus.Available)
		{
			Debug.Log("firebase CheckDependencies do not finish");
			return;
		}
#endif
		stringBuilder.Clear();

		for (int i=0; i<parameters.Length; i++)
		{
			stringBuilder.Append(parameters[i]);

			if (i < parameters.Length - 1)
				stringBuilder.Append('_');
		}

		Firebase.Analytics.FirebaseAnalytics.LogEvent(stringBuilder.ToString());
		//Debug.LogFormat("===== {0}", stringBuilder.ToString());
	}
}

public class FirebaseLogType
{
	public const string Success = "Success";
	public const string Opening = "Opening";
	public const string Free = "Free";
	public const string New = "New";

	// 씬별 버튼류
	public const string IngameSceneMenuBtn = "IngameSceneMenuBtn";
	public const string TitleSceneBtn = "TitleSceneBtn";
	public const string GameOverSceneBtn = "GameOverSceneBtn";

	// 애드몹 리워드 보상 광고
	public const string RewardSpeedMode = "RewardSpeedMode";
	public const string RewardPowerMode = "RewardPowerMode";
	public const string RewardContinue = "RewardContinue";
	public const string RewardRevive = "RewardRevive";

	// 네트워크 미연결
	public const string NotConnectionNetwork = "NotConnectionNetwork";

	// 시스템 팝업
	public const string AlertPopup = "AlertPopup";

	// 베스트 스코어 갱신
	public const string PostHighScore = "PostHighScore";

	// 공지창 관련
	public const string SkipNotice = "SkipNotice";
	public const string OpenNotice = "OpenNotice";
	public const string ExpiredSkipNotice = "ExpiredSkipNotice";

	// 로그인 관련
	public const string BackUpLoginAlert = "BackUpLoginAlert";
	public const string GoogleLoginBtnClick = "GoogleLoginBtnClick";
	public const string AppleLoginBtnClick = "AppleLoginBtnClick";
	public const string AppleLoginForAos = "AppleLoginForAos";
	public const string AppleLoginForIos = "AppleLoginForIos";
	public const string GoogleLoginForAos = "GoogleLoginForAos";
	public const string GoogleLoginForIos = "GoogleLoginForIos";

	// referrer 관련
	public const string ReferrerData = "ReferrerData";
}
#endif