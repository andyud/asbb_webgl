#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;
using AppleAuth.Interfaces;
using System.Text;
using Proyecto26;
using System.Linq;

[Serializable]
public class SubPushData
{
    public long push_time { get; set; }
    public bool click { get; set; }
}

[Serializable]
public class PushData
{
    public string adid { get; set; }
    public string fcm_token { get; set; }
    public List<SubPushData> data { get; set; }
}

[Serializable]
public class RemoteConfigData_MoreGames
{
	public bool IsActive;
	public string Ko_ImageUrl;
	public string En_ImageUrl;
	public string TextureName;
	public string AosClickUrl;
	public string IosClickUrl;
}

public class RemoteConfigData
{
	public bool IsGamebookIcon { get; set; }
	public bool IsReviveFeature { get; set; }
	public bool IsBackUpFeature { get; set; }

	public RemoteConfigData_MoreGames[] MoreGames { get; set; }
}

public class FireBaseController
{
	public static RemoteConfigData ConfigData; 

	public static DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	public static string FCMToken = string.Empty;
	public static int maxPushCount = 3;
	public static bool isLogin = false;

	private const string ApiKey = "AIzaSyDGR7kM3yuhkRZXB9EovI6gj1SP5xfst5A";
	private static FirebaseAuth auth = null;

	// firebase init
	public static void Initialize()
	{
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				Debug.LogError("FireBaseController.Initialize() 1");
			}
			else if (task.IsFaulted)
			{
				Debug.LogError("FireBaseController.Initialize() 2");
			}
			else if (task.IsCompleted)
			{
				dependencyStatus = task.Result;
				if (dependencyStatus == Firebase.DependencyStatus.Available)
				{
					Debug.Log("FireBaseController.Initialize is success");
					Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
					Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
					InitializeFirebaseAuth();
					InitRemoteConfig();
				}
				else
				{
					Debug.LogError("FireBaseController.Initialize() 3");
				}
			}
			else
			{
				Debug.LogError("FireBaseController.Initialize() 4");
			}
		});
	}

	// auth 초기화
	private static void InitializeFirebaseAuth()
	{
		Debug.Log("FireBaseController.InitializeFirebaseAuth()");
		auth = FirebaseAuth.DefaultInstance;

		if(auth != null)
		{
			auth.StateChanged += AuthStateChanged;
		}			
	}

	static void InitRemoteConfig()
	{
		Debug.Log("FireBaseController.InitRemoteConfig()");

		//Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(RemoteConfigFetchComplete);
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(12)).ContinueWithOnMainThread(RemoteConfigFetchComplete);
	}

	static void RemoteConfigFetchComplete(Task task)
	{
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

		string configValue;

		if (task.IsCanceled)
		{
			Debug.LogException(task.Exception);
		}
		else if (task.IsFaulted)
		{
			Debug.LogException(task.Exception);
		}
		else if (task.IsCompleted)
		{
			Debug.Log("FireBaseController.InitRemoteConfig() / task.IsCompleted");

			foreach(KeyValuePair<string,Firebase.RemoteConfig.ConfigValue> info in Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.AllValues)
            {
				Debug.Log("파베 리모트 value : " + info.Key);
            }

			configValue = string.Empty;
			configValue = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKey.IsReviveFeature).StringValue;
			if (!string.IsNullOrEmpty(configValue))
			{
				Debug.LogFormat("FireBaseController.InitRemoteConfig() / key: {0} value: {1}", RemoteConfigKey.IsReviveFeature, configValue);
				if(ConfigData == null)
					ConfigData = new RemoteConfigData();

				ConfigData.IsReviveFeature = bool.Parse(configValue);
			}
			else
			{
				Debug.Log("error => FireBaseController.InitRemoteConfig() / configValue(IsReviveFeature) is Null or Empty");
			}

			configValue = string.Empty;
			configValue = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKey.IsBackupFeature).StringValue;
			if(!string.IsNullOrEmpty(configValue))
			{
				Debug.LogFormat("FireBaseController.InitRemoteConfig() / key: {0} value: {1}", RemoteConfigKey.IsBackupFeature, configValue);
				if (ConfigData == null)
					ConfigData = new RemoteConfigData();

				ConfigData.IsBackUpFeature = bool.Parse(configValue);
			}
			else
			{
				Debug.Log("error => FireBaseController.InitRemoteConfig() / configValue(IsBackupFeature) is Null or Empty");
			}

			configValue = string.Empty;
			configValue = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKey.MoreGames).StringValue;
			if(!string.IsNullOrEmpty(configValue))
			{
				Debug.LogFormat("FireBaseController.InitRemoteConfig() / key: {0} value: {1}", RemoteConfigKey.MoreGames, configValue);
				if (ConfigData == null)
					ConfigData = new RemoteConfigData();

				ConfigData.MoreGames = JsonHelper.ArrayFromJson<RemoteConfigData_MoreGames>(configValue);
			}

			//configValue = string.Empty;
			//configValue = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKey.IsGamebookIcon).StringValue;
			//if (!string.IsNullOrEmpty(configValue))
			//{
			//	Debug.LogFormat("FireBaseController.InitRemoteConfig() / key: {0} value: {1}", RemoteConfigKey.IsGamebookIcon, configValue);
			//	if (ConfigData == null)
			//		ConfigData = new RemoteConfigData();

			//	ConfigData.IsGamebookIcon = bool.Parse(configValue);

			//	Main._main._gui.UpdateGamebookUI();
			//}
			//else
			//{
			//	Debug.Log("error => FireBaseController.InitRemoteConfig() / configValue(IsGamebookIcon) is Null or Empty");
			//	// 필요없는 부분.. 다음패치에 지우자!..
			//	configValue = string.Empty;
			//	configValue = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKey.IsBottomGameList).StringValue;
			//	if (!string.IsNullOrEmpty(configValue))
			//	{
			//		Debug.LogFormat("FireBaseController.InitRemoteConfig() / key: {0} value: {1}", RemoteConfigKey.IsBottomGameList, configValue);
			//		if (ConfigData == null)
			//			ConfigData = new RemoteConfigData();

			//		ConfigData.IsGamebookIcon = !bool.Parse(configValue);

			//		Main._main._gui.UpdateGamebookUI();
			//	}
			//	else
			//		Debug.Log("error => FireBaseController.InitRemoteConfig() / configValue(IsBottomGameList) is Null or Empty");
			//}
		}
		else
		{
			Debug.LogException(task.Exception);
		}
	}

	// 로그인 정보 변경 체크
	private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
	{
		Debug.Log("=====AuthStateChanged");
		// 미 로그인 유저 처리
		if (auth == null || auth.CurrentUser == null)
		{
			Debug.Log("FireBaseController.AuthStateChanged() / User not logged in");
			isLogin = false;
			IAPController.instance.SetIAP();
			AppServerController.Instance.IsNewDBInit = true;
		}
		else
		{
			isLogin = true;
			Debug.Log("FireBaseController.AuthStateChanged() / Existing login user");
			Debug.Log("FireBaseController.AuthStateChanged() / userId: " + auth.CurrentUser.UserId);

			AppServerController.Instance.AppStart(() =>
			{
				AppServerController.Instance.IsNewDBInit = true;

				// 유저 토근값 전송.
				AppServerController.Instance.FCMtoken(callback => { } , FCMToken);
				
				Main.SendRankinScore();

				////if(GUIController_Imp._instance._currentState == GUIController.State.INGAME)
				//{
				//	int bestscore = UserData._Score_Best;
				//	if (UserData._Score_Best <= PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore))
				//		bestscore = PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore);

				//	AppServiceController.Instance.RankingScore(response =>
				//	{
				//		if (response != null)
				//			Debug.Log($"call back startRankingScore / reason: {response.reason}");
				//	}, bestscore, false);// PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore), true);
				//}
			});
		}
	}

	public static string GetProviderId()
	{
		string providerId = string.Empty;
		try
		{
			if (auth != null && auth.CurrentUser != null && auth.CurrentUser.ProviderData != null)
			{
				foreach(var data in auth.CurrentUser.ProviderData)
				{
					providerId += data.ProviderId;
				}
				//providerId = auth.CurrentUser.ProviderData.First().ProviderId;
			}
		}
		catch(Exception e)
		{
			Debug.LogException(e);
		}

		return providerId;
	}

	public static string GetUserId()
	{
		if (auth != null && auth.CurrentUser != null && !string.IsNullOrEmpty(auth.CurrentUser.UserId))
		{
			Debug.Log($"=====FirebaseController.GetUserId() / userId: {auth.CurrentUser.UserId}");
			return auth.CurrentUser.UserId; //"XNAIr1DK6RhXff2aV5hS5nPy9Un2";// 
		}

		return string.Empty;
	}

	public static string GetUserEmail()
	{
		if (auth != null && auth.CurrentUser != null && !string.IsNullOrEmpty(auth.CurrentUser.Email))
		{
			Debug.Log($"=====FirebaseController.GetUserEmail() / userEmail: {auth.CurrentUser.Email}");
			return auth.CurrentUser.Email;
		}


		return string.Empty;
	}

	// 로그아웃
	public static void AuthSignOut()
	{
		if(auth != null)
			auth.SignOut();

		isLogin = false;
	}

	// 탈퇴
	public static void DeleteUser()
	{
		auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task =>
		{
			if(task.IsCanceled)
			{
				Debug.LogException(task.Exception);
			}
			else if(task.IsFaulted)
			{
				Debug.LogException(task.Exception);
			}
			else
			{
				Debug.Log("===== FirebaseController.DeleteUser() / is success");
			}
		});
	}
	
	public static void SignInWithAppleWithAOS(FederatedOAuthProvider provider)
	{
		Debug.Log("=====FirebaseController.SignInWithAppleWithAOS()");

		auth.SignInWithProviderAsync(provider).ContinueWithOnMainThread(
			task =>
			{
				if (task.IsCanceled)
				{
					Debug.Log("=====FirebaseController.SignInWithAppleWithAOS() / task is canceled");
				}
				else if (task.IsFaulted)
				{
					Debug.Log("=====FirebaseController.SignInWithAppleWithAOS() / task is Faulted");
				}
				else
				{
					SignInResult signInResult = task.Result;
					FirebaseUser user = signInResult.User;
					Debug.LogFormat("=====FirebaseController.SignInWithAppleWithAOS() / completed / displayName: {0}, userId: {1}", user.DisplayName, user.UserId);
				}
			});
	}

	

	public static void SignInWithGoogle(FederatedOAuthProvider provider, Action<bool, string, string> callback)
	{
		Debug.Log("FireBaseController.SignInWithGoogle()");

		if(auth.CurrentUser != null)
		{
			Debug.Log("FireBaseController.SignInWithGoogle() / auth.CurrentUser is true");
			callback.Invoke(true, string.Empty, auth.CurrentUser.UserId);
			return;
		}

		auth.SignInWithProviderAsync(provider).ContinueWithOnMainThread(
			task =>
			{
				if (task.IsCanceled)
				{
					callback.Invoke(false, task.Exception.ToString(), string.Empty);
				}
				else if (task.IsFaulted)
				{
					callback.Invoke(false, task.Exception.ToString(), string.Empty);
				}
				else
				{
					try
					{
						callback.Invoke(true, string.Empty, task.Result.User.UserId);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
				}
			});
	}

	// aos 전용
	public static void SignInWithApple(FederatedOAuthProvider provider, Action<bool, string, string> callback)
	{
		Debug.Log("FireBaseController.SignInWithApple()");

		if (auth.CurrentUser != null)
		{
			Debug.Log("FireBaseController.SignInWithApple() / auth.CurrentUser is true");
			callback.Invoke(true, string.Empty, auth.CurrentUser.UserId);
			return;
		}

		auth.SignInWithProviderAsync(provider).ContinueWithOnMainThread(
			task =>
			{
				if (task.IsCanceled)
					callback.Invoke(false, task.Exception.ToString(), string.Empty);
				else if (task.IsFaulted)
					callback.Invoke(false, task.Exception.ToString(), string.Empty);
				else
				{
					try
					{
						callback.Invoke(true, string.Empty, task.Result.User.UserId);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
				}
			});
	}

	// ios 전용
	public static void SignInWithApple(IAppleIDCredential appleIdCredential, string rawNonce, Action<bool, string, string> callback)
	{
		Debug.Log("FirebaseController.SignInWithApple()");

		var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
		var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
		var firebaseCredential = OAuthProvider.GetCredential("apple.com", identityToken, rawNonce, authorizationCode);

		auth.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(
		task =>
		{
			if (task.IsCanceled)
				callback.Invoke(false, task.Exception.ToString(), string.Empty);
			if (task.IsFaulted)
				callback.Invoke(false, task.Exception.ToString(), string.Empty);
			else
			{
				try
				{
					callback.Invoke(true, string.Empty, task.Result.UserId);
				}
				catch(Exception e)
				{
					Debug.LogException(e);
				}
			}
		});
	}

	public static void SignInWithGoogle(string idToken, string accessToken, Action<bool> callback)
	{
		if (auth == null)
			return;

		var googleCredential = GoogleAuthProvider.GetCredential(idToken, accessToken);
		var isSuccess = false;

		auth.SignInWithCredentialAsync(googleCredential).ContinueWithOnMainThread(task =>
		{
			if(task.IsCanceled)
			{
				Debug.LogException(task.Exception);
			}
			else if(task.IsFaulted)
			{
				Debug.LogException(task.Exception);
			}
			else if(task.IsCompleted)
			{
				Debug.Log("FireBaseController.SignInWithGoogle() / SignInWithCredentialAsync is Completed");
				isSuccess = true;
			}
			else
			{
				Debug.LogException(task.Exception);
			}

			if (callback != null)
				callback.Invoke(isSuccess);
		});

		// firebase auth에서 지원하지 않는 플랫폼일때 사용하자.
		//var payLoad =
		//	$"{{\"postBody\":\"id_token={token}&providerId={providerId}\",\"requestUri\":\"http://localhost\",\"returnIdpCredential\":true,\"returnSecureToken\":true}}";
		//RestClient.Post($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={ApiKey}", payLoad).Then(
		//	response =>
		//	{
		//		Debug.Log(response.Text);
		//	}).Catch(Debug.Log);
	}

	// 이메일 인증 관련.
	public static void SendEmailVerification(Action<string, string> callback)
	{
		if(auth == null || auth.CurrentUser == null || auth.CurrentUser.IsAnonymous)
		{
			Debug.Log("FireBaseController.SendEmailVerification() / auth == null or auth.CurrentUser == null or auth.CurrentUser.IsAnonymous == true");

			if (callback != null)
				callback.Invoke("인증 실패 알림", "로그인 정보가 없습니다.\n확인 바랍니다.");
		}
		else
		{
			auth.CurrentUser.SendEmailVerificationAsync().ContinueWith(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogException(task.Exception);

					if (callback != null)
						callback.Invoke("인증 취소 알림", "인증이 취소 되었습니다.");
				}
				else if (task.IsFaulted)
				{
					Debug.LogException(task.Exception);

					if (callback != null)
						callback.Invoke("인증 실패 알림", "인증이 실패 하였습니다.");
				}
				else if (task.IsCompleted)
				{
					Debug.Log("FireBaseController.SendEmailVerification() / task Is Completed");

					if (callback != null)
						callback.Invoke("인증 이메일 전송 알림", "인증 이메일 전송이 완료 되었습니다.\n이메일 인증을 완료하면 보상을 드립니다.");
				}
				else
				{
					Debug.LogException(task.Exception);
				}
			});
		}
	}

	// 이메일, 패스워드 계정 생성.
	public static void CreateUserWithEmailAndPassword(string email, string password, Action<string, string> callback)
	{
		// 기존 익명 로그인 유저 email 로그인 연동.
		if (auth != null && auth.CurrentUser != null)
		{
			// 기존 게스트 유저
			if(auth.CurrentUser.IsAnonymous)
			{
				Credential credential = null;
				try
				{
					credential = EmailAuthProvider.GetCredential(email, password);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					return;
				}

				auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
				{
					if (task.IsCanceled)
					{
						Debug.LogException(task.Exception);

						if (callback != null)
							callback.Invoke("계정 연동 가입 취소 알림", "계정 연동을 취소 하였습니다.");
					}
					else if (task.IsFaulted)
					{
						Debug.LogException(task.Exception);

						if (callback != null)
							callback.Invoke("계정 연동 가입 실패 알림", "계정 연동 가입에 실패 하였습니다.");
					}
					else if (task.IsCompleted)
					{
						Debug.Log("FireBaseController.CreateUserWithEmailAndPassword() / LinkWithCredentialAsync task is Completed");

						if (callback != null)
							callback.Invoke("계정 연동 가입 성공 알림", "계정이 성공적으로 연동 되었습니다.");
					}
					else
					{
						Debug.LogException(task.Exception);
					}
				});
			}
			else // 이미 계정이 존재함.
			{
				if (callback != null)
					callback.Invoke("신규 가입 불가 알림", "기존 유저 정보가 존재 합니다.");
			}
		}
		else // 회원 가입
		{
			auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogException(task.Exception);

					if (callback != null)
						callback.Invoke("계정 가입 취소 알림", "계정 가입이 취소 되었습니다.");
				}
				else if (task.IsFaulted)
				{
					Debug.LogException(task.Exception);

					if (callback != null)
						callback.Invoke("계정 가입 실패 알림", "계정 가입이 실패 하였습니다.");
				}
				else if(task.IsCompleted)
				{
					Debug.Log("FireBaseController.CreateUserWithEmailAndPassword() / CreateUserWithEmailAndPasswordAsync task is Completed");

					if (callback != null)
						callback.Invoke("계정 가입 성공 알림", "계정 가입에 성공 하였습니다.");
				}
				else
				{
					Debug.LogException(task.Exception);
				}
			});
		}
	}

	// 익명 로그인
	public static void GuestLogin(Action<string, string> callback)
	{
		// 로그인 정보 존재 하다면 처리..
		if (auth.CurrentUser != null)
		{
			// 게스트 로그인 상태
			if(auth.CurrentUser.IsAnonymous)
			{
				if (callback != null)
					callback.Invoke("게스트 로그인 상태 알림", "이미 게스트 로그인 상태입니다.");
			}
			else if(!string.IsNullOrEmpty(auth.CurrentUser.Email))
			{
				if (callback != null)
					callback.Invoke("로그인 상태 알림", "이미 로그인 상태입니다.");
			}
			else
			{
				Debug.LogError("FireBaseController.GuestLogin() / Unknown error");
			}
		}
		else
		{
			auth.SignInAnonymouslyAsync().ContinueWith(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogException(task.Exception);

					if (callback != null)
						callback.Invoke("게스트 로그인 취소 알림", "게스트 로그인 취소 되었습니다.");
				}
				else if (task.IsFaulted)
				{
					Debug.LogException(task.Exception);

					if (callback != null)
						callback.Invoke("게스트 로그인 실패 알림", "게스트 로그인 실패 하였습니다.");
				}
				else if (task.IsCompleted)
				{
					Debug.Log("FireBaseController.GuestLogin() / SignInAnonymouslyAsync task is Completed");

					if (callback != null)
						callback.Invoke("게스트 로그인 성공 알림", "게스트 로그인 성공 하였습니다.");
				}
				else
				{
					Debug.LogException(task.Exception);
				}
			});
		}
	}

	// 기존 계정 로그인
	public static void SignInWithEmailAndPassword(string email, string password, Action<string, string> callback)
	{
		auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
		{
			if(task.IsCanceled)
			{
				Debug.LogException(task.Exception);

				if (callback != null)
					callback.Invoke("로그인 취소 알림", "로그인 취소 되었습니다");
			}
			else if (task.IsFaulted)
			{
				Debug.LogException(task.Exception);

				if (callback != null)
					callback.Invoke("로그인 실패 알림", "로그인 실패 하였습니다.");
			}
			else if(task.IsCompleted)
			{
				Debug.Log("FireBaseController.SignInWithEmailAndPassword() / SignInWithEmailAndPasswordAsync task is Completed");

				if (callback != null)
					callback.Invoke("로그인 성공 알림", "로그인 성공 하였습니다.");
			}
			else
			{
				Debug.LogException(task.Exception);
			}
		});
	}

/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

	private static void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
	{
		UnityEngine.Debug.Log("FCM Token: " + token.Token);
		FCMToken = token.Token;
	}

	private static void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
	{
		if (e != null && e.Message != null && e.Message.Data != null && e.Message.NotificationOpened)
		{
			foreach (var data in e.Message.Data)
			{
				switch (data.Key)
				{
#if UNITY_ANDROID && !UNITY_EDITOR
					case "SmartShop":
						switch(data.Value)
						{
							case "Open":
								SmartShopController.CallActivitySmartShop();
								break;
							default:
								SmartShopController.CallActivitySmartShop(data.Value);
								break;
						}
						break;
#endif
					case "SpeedAndPower":
						var rewardValue = int.Parse(data.Value);
						var currentValue = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, 0);

						PlayerPrefs.SetInt(PlayerPrefs_Config.FCMRewardCount, rewardValue);

						PlayerPrefs.SetInt(PlayerPrefs_Config.ActiveSpeedMode, 1);
						PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, rewardValue + currentValue);

						currentValue = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0);
						PlayerPrefs.SetInt(PlayerPrefs_Config.ActivePowerMode, 1);
						PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, rewardValue + currentValue);
						break;
					case "OpenURL":
						Application.OpenURL(data.Value);
						break;
				}
			}

			if (e.Message.Data.Keys.Contains("SpeedAndPower"))
			{
				PopUpController.AddAlert(AlertPopUpType.FCM_Reward_Item);
			}
		}

		//Debug.Log("FCM message from: " + e.Message.From);

		//int pushCount = PlayerPrefs.GetInt("PushCount", 1);
		//string pushMessage = PlayerPrefs.GetString("PushMessage", "");

		//string push = UnixTimeNow() + "|" + e.Message.NotificationOpened.ToString();
		//if (pushCount < maxPushCount)
		//	push += "|";

		//PlayerPrefs.SetString("PushMessage", pushMessage + push);

		//if (pushCount < maxPushCount)
		//{
		//	PlayerPrefs.SetInt("PushCount", pushCount + 1);
		//}
		//else if (pushCount == maxPushCount)
		//{
		//	WebRequest.instance.HTTPSend(ADTYPE.PUSH);
		//}
		//else
		//{
		//	PlayerPrefs.SetInt("PushCount", 1);
		//	PlayerPrefs.SetString("PushMessage", "");
		//}
	}

	private static long UnixTimeNow()
	{
		var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
		return (long)timeSpan.TotalMilliseconds;
	}
}
#endif