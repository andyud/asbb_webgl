using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public static class SocialController
{
	public static void Initialize()
	{
#if UNITY_ANDROID
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = false;
		PlayGamesPlatform.Activate();
#endif
	}

	public static void LogIn(System.Action<bool> successCallback = null)
	{
#if UNITY_ANDROID
		Social.localUser.Authenticate((bool success) =>
		{
			if (successCallback != null)
				successCallback(success);
		});
#endif
	}

	public static void LogOut()
	{
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.SignOut();
#endif
	}

	public static void OpenLeaderboard()
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
			Social.ShowLeaderboardUI();
		else
			LogIn((bool b) => { if (b) { Social.ShowLeaderboardUI(); }; });
#endif
	}

	public static void OpenAchievement()
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
			Social.ShowAchievementsUI();
		else
			LogIn((bool b) => { if (b) { Social.ShowAchievementsUI(); }; });
#endif
	}

	public static void PostHighScore(int score)
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
			ReportScore(score);
#endif
	}

	static void ReportScore(int score, System.Action successCallback = null)
	{
#if UNITY_ANDROID
		Social.ReportScore(score, Static_APP_Config._LeaderboardID, (bool success) =>
		{
			if (success)
			{
				if (successCallback != null)
					successCallback();
			}
		});
#endif
	}

	public static void LogInAndPostHighScore(int score, System.Action successCallback)
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
			ReportScore(score, successCallback);
		else
		{
			LogIn((bool b) =>
			{
				if (b)
				{
					if (successCallback != null)
						successCallback();
					ReportScore(score, OpenLeaderboard);
				}
			});
		}
#endif
	}

	public static void PostHighScore(float score)
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
			ReportScore_Float(score);
#endif
	}

	static void ReportScore_Float(float score, System.Action successCallback = null)
	{
#if UNITY_ANDROID
		Social.ReportScore((long)(score), Static_APP_Config._LeaderboardID, (bool success) =>
		{
			if (success)
			{
				if (successCallback != null)
					successCallback();
			}
		});
#endif
	}

	public static void LogInAndPostHighScore(float score, System.Action successCallback)
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
			ReportScore_Float(score, successCallback);
		else
		{
			LogIn((bool b) =>
			{
				if (b)
				{
					if (successCallback != null)
						successCallback();
					ReportScore_Float(score, OpenLeaderboard);
				}
			});
		}
#endif
	}

	public static void UnlockAchievement(string id, System.Action successCallback = null)
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated)
		{
			Social.ReportProgress(id, 100.0f, (bool success) =>
			{
				if (success)
				{
					if (successCallback != null)
						successCallback();
				}
			});
		}
#endif
	}
}
