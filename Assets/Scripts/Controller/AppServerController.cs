//#define TEST
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using System;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using System.Linq;

public class AppServerController : MonoBehaviour
{
	private const string Dev = "https://ddobbokki.com";
	private const string Release = "https://ddobbokki.com";

	private const string Web_Dev = "https://ddobbokki.com";
	private const string Web_Release = "https://ddobbokki.com";

	public static AppServerController Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<AppServerController>();
				if (instance.isDev)
				{
					instance.ServerUri = Dev;
					instance.WebUri = Web_Dev;
				}
				else
				{
					instance.ServerUri = Release;
					instance.WebUri = Web_Release;
				}
			}

			return instance;
		}
	}

	private static AppServerController instance;

	public bool isDev;
	[HideInInspector]
	public string ServerUri;

	[HideInInspector]
	public string WebUri;

	[HideInInspector]
	public bool IsNewDBInit;

	public void IAP(string receipt, Action<string> callbackComplete, bool isReset = true)
	{
		Debug.Log($"AppServiceController.IAP()");
#if UNITY_ANDROID
		if(FireBaseController.isLogin == false || string.IsNullOrEmpty(receipt))
		{
			Debug.LogError("error => AppServiceController.IAP()");
			return;
		}
#endif
		var receiptData = ExtractReceipt(receipt);

		// 중복제거
		receiptData = receiptData.Distinct().ToList();

		if (receiptData != null && receiptData.Count > 0)
		{
			ReceiptVerification(1, receiptData, response =>
			{
				if (response.result)
				{
					if(callbackComplete != null)
					{
						SetIAPItem(response.data, isReset);
						callbackComplete.Invoke(receipt);
					}
				}
				else
				{
					Debug.LogError($"===== error => AppServiceController.GetBeforeIAP()");
				}
			});
		}
	}

	public void AppStart(Action callback)
	{
#if UNITY_ANDROID
		var userId = FireBaseController.GetUserId();
		if (String.IsNullOrEmpty(userId))
		{
			Debug.LogError($"error => AppserviceController.AppStart()");
			callback.Invoke();
			return;
		}

		if (String.IsNullOrEmpty(FireBaseController.GetUserEmail()))
		{
			Debug.LogError($"error => AppserviceController.AppStart()");
			callback.Invoke();
			return;
		}
#else
		var userId = "111";
		callback.Invoke();
#endif
		GetUserMigrate(()=> { 
			// todo: 여기서 신규 유저인지 먼저 체크한다.
			CheckNewDBUser(response =>
			{
				// 신규 유저
				if(response.result && response.data)
				{
					Debug.Log("신규유저?");
					HasIAP(() =>
					{
						callback.Invoke();
					});
				}
				else // 기존유저
				{
					StartCoroutine(GetBeforeData(userId));
					callback.Invoke();
				}
			});
		});
	}

    public void GetBeforeDataTest(string userId)
    {
        StartCoroutine(GetBeforeData(userId));
    }

    public void GetBeforeIAPDataTest(string userId)
    {
        StartCoroutine(GetIAPDataTest(userId));
    }

    IEnumerator GetIAPDataTest(string userId)
    {
        var uri = string.Format("{0}?token={1}", string.Format("{0}/api/v1/inapp/user/receipt/get", ServerUri), userId);

        Debug.Log("------------ValidationUserReceipt_Request url : " + uri);

        using (var uwr = UnityWebRequest.Get(uri))
        {
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError("error => WebRequest.ValidationUserReceipt_Request()");
            }
            else
            {
                Debug.Log($"success => WebRequest.ValidationUserReceipt_Request() : " + uwr.downloadHandler.text);

                var result = JsonMapper.ToObject<User_IAP_Response>(uwr.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// AppStart Connection Method
    /// </summary>
    IEnumerator GetBeforeData(string userId)
	{
		var uri = string.Format("{0}/api/v1/firebase/auth/{1}/user/data", ServerUri, userId);

		Debug.Log("------------GetBeforeData url : " + uri);

		using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
		{
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

#if UNITY_EDITOR
			if(uwr.isHttpError)
#else
			if(uwr.isNetworkError || uwr.isHttpError)
#endif
			{
				Debug.LogErrorFormat("error => AppServiceController.GetBeforeData() / errormessage: {0}", uwr.error);
			}
			else
			{
				try
				{
					Debug.Log("------------success GetBeforeData : " + uwr.downloadHandler.text);

					//string text = "{\"result\":\"success\",\"code\":0,\"message\":\"Success\",\"data\":{\"token\":\"SG5zXypGT3M4h7pboaQDSne6fEf2\",\"user_data\":{\"BestScore\":43028,\"LastBackupDate\":\"00\\/00\\/0000 00:00:00\"}}}";

                    var responseData = JsonMapper.ToObject<ResponseAppServiceBackUpData>(uwr.downloadHandler.text);
					var backupData = new NewBackUpData();

					// AppService 서버에 데이터가 존재.
					if (responseData != null && responseData.result == "success" && responseData.data != null && responseData.data.user_data != null)
					{
						backupData.BestScore = responseData.data.user_data.BestScore;
						backupData.LastBackupDate = responseData.data.user_data.LastBackupDate;
					}
					else // AppService 서버에 데이터가 존재하지 않음.
					{
						backupData.BestScore = 0;
						backupData.LastBackupDate = DateTime.MinValue;
					}

					if (UserData._Score_Best <= backupData.BestScore)
						UserData._Score_Best = backupData.BestScore;
					else
						backupData.BestScore = UserData._Score_Best;

#if !TEST
					SetUserBackUpData(backupData, response =>
					{
						if(response.result)
						{
							GetIAP();
						}
					});
#endif
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
	}

	private List<ReceiptData> ExtractReceipt(string receipt)
	{
		if (string.IsNullOrEmpty(receipt))
			return null;

		var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

		try
		{
			var resultArray = validator.Validate(receipt);
			List<ReceiptData> receiptData = new List<ReceiptData>();

			if(resultArray != null && resultArray.Length > 0)
			{
				foreach (var result in resultArray)
				{
					var data = new ReceiptData();
					if (Application.platform == RuntimePlatform.Android)
					{
						var ins = result as GooglePlayReceipt;
						var google = new GooglePlayReceipt(ins.productID, ins.orderID, ins.packageName, ins.purchaseToken, ins.purchaseDate, ins.purchaseState);
						var jsonParse = JsonMapper.ToJson(google);
						data.productId = google.productID;
						data.purchaseTokenOrPayload = google.purchaseToken;
					}

					if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						var apple = result as AppleInAppPurchaseReceipt;
						var jsonParse = JsonMapper.ToJson(apple);
						var jsonData = JsonMapper.ToObject(receipt);

						data.productId = apple.productID;
						data.purchaseTokenOrPayload = jsonData["Payload"].ToString();
					}

					receiptData.Add(data);
				}

				Debug.Log($"return receiptData: {receiptData}");
				return receiptData;
			}
		}
		catch(Exception e)
		{
			Debug.Log($"AppServiceController.ExtractReceipt() / exception: {e.Message}");
		}

		return null;
	}

	//신규 유저 플랫폼 iap 내역 크로스 체크
	public void GetIAP(Action callback)
	{
		if (IAPController.storeController == null || IAPController.storeController.products == null)
		{
			callback.Invoke();
			return;
		}

		var products = IAPController.storeController.products.all;

		List<ReceiptData> receiptData = new List<ReceiptData>();
		for (int i = 0; i < products.Length; i++)
		{
			Debug.Log($"33333receipt: {products[i].receipt}");
			var returnValue = ExtractReceipt(products[i].receipt);
			if (returnValue != null)
				receiptData.AddRange(returnValue);
		}

		receiptData = receiptData.FindAll(v => v != null);
		receiptData = receiptData.Distinct().ToList();

		if (receiptData != null && receiptData.Count > 0)
		{
			ReceiptVerification(1, receiptData, response =>
			{
				if (response.result)
					callback.Invoke();
				else
					callback.Invoke();
			});
		}
		else
			callback.Invoke();
	}

	public void GetIAP(Action<bool> callback = null, Action errorCallback = null)
	{
		if (IAPController.storeController == null || IAPController.storeController.products == null)
		{
			if(callback != null)
				callback.Invoke(false);
			return;
		}

		var products = IAPController.storeController.products.all;
		if (products == null)
		{
			if(callback != null)
				callback.Invoke(false);

			return;
		}

		List<ReceiptData> receiptData = new List<ReceiptData>();
		for (int i = 0; i < products.Length; i++)
		{
			var returnValue = ExtractReceipt(products[i].receipt);
			if(returnValue != null)
				receiptData.AddRange(returnValue);
		}

		// null 제거
		receiptData = receiptData.FindAll(v => v != null);
		// 중복 제거
		receiptData = receiptData.Distinct().ToList();

		if (receiptData != null && receiptData.Count > 0)
		{
			ReceiptVerification(1, receiptData, response =>
			{
				if (response.result)
				{
					SetIAPItem(response.data);

					if (callback != null)
					{
						callback.Invoke(response.data.Any(v => v.status == 2));
					}
				}
				else
				{
					Debug.LogError($"error => AppServiceController.GetBeforeIAP()");
					if (errorCallback != null)
						errorCallback.Invoke();
				}
			});
		}
		else
		{
			if(callback != null)
				callback.Invoke(false);
		}
	}

	public void SetIAPItem(UserReceiptData[] receiptData, bool isReset = true)
	{
		if(isReset)
		{
			// 일괄 결제 취소 처리 후 진행.
			PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 0);
			PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 0);
			PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 0);
#if UNITY_ANDROID
			AdController.AdPurchase = false;
#endif
		}

		foreach (var item in receiptData)
		{
			Debug.Log($"ValidationUserReceipt / response.data.productId: {item.productId} / response.data.status: {item.status}");

			switch (item.productId)
			{
				case IAPController.pdSpeed:
					if(item.status == 0 || item.status == 1) // 결제 상태
					{
						Debug.Log("*** SetIAPItem 아이템 샀나요?");
						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 1);
						InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
						InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
					}
					break;
				case IAPController.pdSkipTurn:
					if (item.status == 0 || item.status == 1) // 결제 상태
					{
						Debug.Log("*** SetIAPItem 아이템 샀나요?");
						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 1);
					}
					break;
				case IAPController.pdAdRemoveSpeed:
					if (item.status == 0 || item.status == 1) // 결제 상태
					{
						Debug.Log("*** SetIAPItem 아이템 샀나요?");
						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 1);
#if UNITY_ANDROID
						AdController.Hide_BannerView();
						AdController.AdPurchase = true;
#endif
						InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
						InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
					}
					break;
			}
		}
	}

	public void HasIAP(Action callback)
	{
		GetIAP(() =>
		{
			ValidationUserReceipt((response) =>
			{
				if (response.data != null)
				{
					SetIAPItem(response.data);
				}
				callback.Invoke();
			});
		});
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// API Call Method Start
	/// </summary>
	/// 
	public void GetValidationUserReceiptTest()
	{
		ValidationUserReceipt((response) =>
		{
			if (response.data != null)
			{
			}
		});
	}

	Coroutine ValidationUserReceipt_Coroutine;
	public void ValidationUserReceipt(Action<User_IAP_Response> callback, bool isTest = false)
	{
#if TEST
		var userid = "e4sa9lp52ZOyxeqFN5TT7WnVcLQ2";// isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : FireBaseController.GetUserId();
#else
#if UNITY_ANDROID
		var userid = isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : FireBaseController.GetUserId();
#else
#endif
		var userid = isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : "WZOFOl7GMQPUQskhIl2aJDJPcfK2";
#endif

		if (string.IsNullOrEmpty(userid))
		{
			Debug.LogError($"error => WebRequest.ValidationUserReceipt()");
			return;
		}

		var request = new User_IAP_Request();
		request.token = userid;

		if (ValidationUserReceipt_Coroutine != null)
			StopCoroutine(ValidationUserReceipt_Coroutine);

		ValidationUserReceipt_Coroutine = StartCoroutine(ValidationUserReceipt_Request(request, callback, isTest));
	}

	IEnumerator ValidationUserReceipt_Request(User_IAP_Request request, Action<User_IAP_Response> response, bool isTest)
	{
		var uri = string.Format("{0}?token={1}", string.Format("{0}/api/v1/inapp/user/receipt/get", ServerUri), request.token);

		Debug.Log("------------ValidationUserReceipt_Request url : " + uri);

		using (var uwr = UnityWebRequest.Get(uri))
		{
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (isTest ? uwr.isHttpError : uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => WebRequest.ValidationUserReceipt_Request()");
			}
			else
			{
				Debug.Log($"success => WebRequest.ValidationUserReceipt_Request() : " + uwr.downloadHandler.text);

				var result = JsonMapper.ToObject<User_IAP_Response>(uwr.downloadHandler.text);
				response.Invoke(result);
			}
		}

		yield return 0;
	}

	Coroutine ReceiptVerification_Coroutine;
	public void ReceiptVerification(int itemType, List<ReceiptData> receiptData, Action<IAP_Response> callback, bool isTest = false)
	{
		var iapRequest = new IAP_Request();
#if UNITY_ANDROID
		iapRequest.token = isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : FireBaseController.GetUserId();
#endif
		iapRequest.package = Application.identifier;
		if (isTest)
			iapRequest.osType = 0;
		else
			iapRequest.osType = Application.platform == RuntimePlatform.IPhonePlayer ? 1 : 0;

		iapRequest.itemType = itemType;
		iapRequest.receiptData = receiptData;

		if (ReceiptVerification_Coroutine != null)
			StopCoroutine(ReceiptVerification_Coroutine);

		ReceiptVerification_Coroutine = StartCoroutine(ReceiptVerification_Request(iapRequest, callback, isTest));

	}

	IEnumerator ReceiptVerification_Request(IAP_Request request, Action<IAP_Response> response, bool isTest)
	{
		Debug.Log("------------ReceiptVerification_Request url : " + string.Format("{0}/api/v1/inapp/get", ServerUri));

		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/inapp/get", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);

			Debug.Log("------------ReceiptVerification_Request jsonData : " + jsonData);

			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);

			Debug.Log("------------ReceiptVerification_Request bytes : " + bytes);

			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (isTest ? uwr.isHttpError : uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => WebRequest.ReceiptVerification_Request()");
			}
			else
			{
				Debug.Log($"success => WebRequest.ReceiptVerification_Request() : " + uwr.downloadHandler.text);
				var result = JsonMapper.ToObject<IAP_Response>(uwr.downloadHandler.text);
				response.Invoke(result);
			}
		}

		yield return 0;
	}

	Coroutine CheckNewDBUser_Coroutine;
	public void CheckNewDBUser(Action<CheckNewDBUser_Response> callback, bool isTest = false)
	{
		var request = new CheckNewDBUser_Request();
		request.package = Application.identifier;
#if UNITY_ANDROID
		request.token = isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : FireBaseController.GetUserId();
#endif
		//request.email = isTest ? "" : FireBaseController.GetUserEmail();

		if (CheckNewDBUser_Coroutine != null)
			StopCoroutine(CheckNewDBUser_Coroutine);

		CheckNewDBUser_Coroutine = StartCoroutine(CheckNewDBUser_Request(request, callback, isTest));
	}

	IEnumerator CheckNewDBUser_Request(CheckNewDBUser_Request request, Action<CheckNewDBUser_Response> response, bool isTest)
	{
		Debug.Log("------------CheckNewDBUser_Request url : " + string.Format("{0}/api/v1/inapp/user/check", ServerUri));
		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/inapp/user/check", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (isTest ? uwr.isHttpError : uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => WebRequest.CheckNewDBUser_Request()");
			}
			else
			{
				Debug.Log($"success => WebRequest.CheckNewDBUser_Request()");
				var result = JsonMapper.ToObject<CheckNewDBUser_Response>(uwr.downloadHandler.text);
				response.Invoke(result);
			}
		}

		yield return 0;
	}

	Coroutine GetUserBackUpData_Coroutine;
	public void GetUserBackUpData(Action<GetUserBackUpData_Response> callback, bool isTest = false)
	{
		var request = new GetUserBackUpData_Request();
		request.package = Application.identifier;
#if UNITY_ANDROID
		request.token = isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : FireBaseController.GetUserId();
#endif
		if (GetUserBackUpData_Coroutine != null)
			StopCoroutine(GetUserBackUpData_Coroutine);

		GetUserBackUpData_Coroutine = StartCoroutine(GetUserBackUpData_Request(request, callback, isTest));
	}

	IEnumerator GetUserBackUpData_Request(GetUserBackUpData_Request request, Action<GetUserBackUpData_Response> response, bool isTest)
	{
		Debug.Log("------------GetUserBackUpData_Request url : " + string.Format("{0}/api/v1/inapp/user/info/get", ServerUri));

		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/inapp/user/info/get", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (isTest ? uwr.isHttpError : uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => WebRequest.GetUserBackUpData_Request()");
			}
			else
			{
				Debug.Log($"success => WebRequest.GetUserBackUpData_Request() : " + uwr.downloadHandler.text);
				var result = JsonMapper.ToObject<GetUserBackUpData_Response>(uwr.downloadHandler.text);
				response.Invoke(result);
			}
		}

		yield return 0;
	}

	Coroutine SetUserBackUpData_Coroutine;
	public void SetUserBackUpData(NewBackUpData data, Action<SetUserBackUpData_Response> callback, bool isTest = false)
	{
		var request = new SetUserBackUpData_Request();
		request.package = Application.identifier;
#if UNITY_ANDROID
		request.token = isTest ? "WZOFOl7GMQPUQskhIl2aJDJPcfK2" : FireBaseController.GetUserId();
#endif
		//request.email = isTest ? "" : FireBaseController.GetUserEmail();
		//request.token = isTest ? "YXhdsfyaldUVP7p9BcjuzrMs4PS2" : FireBaseController.GetUserId();

		request.user_data = data;

		if (SetUserBackUpData_Coroutine != null)
			StopCoroutine(SetUserBackUpData_Coroutine);

		SetUserBackUpData_Coroutine = StartCoroutine(SetUserBackUpData_Request(request, callback, isTest));
	}

	IEnumerator SetUserBackUpData_Request(SetUserBackUpData_Request request, Action<SetUserBackUpData_Response> response, bool isTest)
	{
		Debug.Log("------------SetUserBackUpData_Request url : " + string.Format("{0}/api/v1/inapp/user/info/set", ServerUri));

		using (var uwr = new UnityWebRequest(String.Format("{0}/api/v1/inapp/user/info/set", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (isTest ? uwr.isHttpError : uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => WebRequest.SetUserBackUpData_Request()");
			}
			else
			{
				Debug.Log($"success => AppServiceController.SetUserBackUpData_Request() : " + uwr.downloadHandler.text);
				JsonData result = JsonMapper.ToObject(uwr.downloadHandler.text);
				var responseCallback = new SetUserBackUpData_Response();
				responseCallback.result = bool.Parse(result["result"].ToString());
				//var result = JsonMapper.ToObject<SetUserBackUpData_Response>(uwr.downloadHandler.text);
				response.Invoke(responseCallback);
			}
		}

		yield return 0;
	}

	Coroutine RankingScore_Coroutine;
	public void RankingScore(Action<RankingScore_Response> callback, int score, bool isStart)
	{
		var request = new RankingScore_Request();
		request.package = Application.identifier;
#if UNITY_ANDROID
		request.token = FireBaseController.GetUserId();
#endif
		request.score = score;
		request.RunTimePlatform = Application.platform.ToString();
		request.appVersion = Application.version;

		if (isStart)
			Debug.Log($"start Score: {request.score}");
		else
			Debug.Log($"end Score: {request.score}");

		if (RankingScore_Coroutine != null)
			StopCoroutine(RankingScore_Coroutine);

		RankingScore_Coroutine = StartCoroutine(RankingScore_Request(request, callback, isStart));
	}

	IEnumerator RankingScore_Request(RankingScore_Request request, Action<RankingScore_Response> response, bool isStart)
	{
		if (isStart)
			PlayerPrefs.SetInt(PlayerPrefs_Config.StartRankingScore_Success, 0);
		else
		{
			if (PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore_Success) == 0)
			{
				PopUpController.AddAlert(AlertPopUpType.Failed_RankingScore);
				yield break;
			}
		}

		Debug.Log("------------RankingScore_Request url : " + string.Format("{0}/api/v1/ranking/user/{1}", ServerUri, isStart ? "start" : "end"));

		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/ranking/user/{1}", ServerUri, isStart ? "start" : "end"), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.Log($"error => AppServiceController.RankingScore_Request() / message: {uwr.error}");
				PopUpController.AddAlert(AlertPopUpType.Failed_RankingScore);
			}
			else
			{
				var result = JsonMapper.ToObject<RankingScore_Response>(uwr.downloadHandler.text);

				if (result != null && !string.IsNullOrEmpty(result.reason))
				{
					switch (result.reason)
					{
						case nameof(ReasonType.success):
							if(isStart)
								PlayerPrefs.SetInt(PlayerPrefs_Config.StartRankingScore_Success, 1);
							Debug.Log("success => AppServiceController.RankingScore_Request()");
							break;

						case nameof(ReasonType.expired):
							PopUpController.AddAlert(AlertPopUpType.Expired_Ranking);
							break;

						case nameof(ReasonType.failed):
						default:
							Debug.Log($"error => AppServiceController.RankingScore_Request() / reason is {result.reason}");
							PopUpController.AddAlert(AlertPopUpType.Failed_RankingScore);
							break;
					}
				}
				else
				{
					Debug.Log($"error => AppServiceController.RankingScore_Request() / result is null or result.reason is null");
					PopUpController.AddAlert(AlertPopUpType.Failed_RankingScore);
				}

				response.Invoke(result);
			}
		}

		yield return 0;
	}


	Coroutine FCMtoken_Coroutine;
	public void FCMtoken(Action<UserFCMToken_Response> callback, string fcmToken)
	{
		var request = new UserFCMToken_Request();
		request.package = Application.identifier;
#if UNITY_ANDROID
		request.token = FireBaseController.GetUserId();
#endif
		request.push_token = fcmToken;

		if (FCMtoken_Coroutine != null)
			StopCoroutine(FCMtoken_Coroutine);

		FCMtoken_Coroutine = StartCoroutine(FCMtoken_Request(request, callback));
	}

	IEnumerator FCMtoken_Request(UserFCMToken_Request request, Action<UserFCMToken_Response> response)
	{
		Debug.Log("------------FCMtoken_Request url : " + string.Format("{0}/api/v1/ranking/user/setPush", ServerUri));

		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/ranking/user/setPush", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => AppServiceController.FCMtoken_Request()");
			}
			else
			{
				Debug.Log($"success => AppServiceController.FCMtoken_Request()");
				var result = JsonMapper.ToObject<UserFCMToken_Response>(uwr.downloadHandler.text);
				response.Invoke(result);
			}
		}

		yield return 0;
	}

	Coroutine NoticeWeb_Coroutine;
	public void NoticeWeb(Action<Notice_Response> callback)
	{
		var request = new Notice_Request();
		request.package = Application.identifier;
#if UNITY_ANDROID
		request.token = FireBaseController.GetUserId();
#endif
		request.RuntimePlatform = Application.platform.ToString();
		request.appVersion = Application.version;
		request.lang = Main.language == SystemLanguage.Korean ? "ko" : "en";
		request.adid = Main.ADID;
		request.device = SystemInfo.deviceModel;
		request.ip = Main.IP;
		request.osVersion = SystemInfo.operatingSystem;

		Debug.Log($"=====package: {request.package}");
		Debug.Log($"=====token: {request.token}");
		Debug.Log($"=====runtimeplatform: {request.RuntimePlatform}");
		Debug.Log($"=====appversion: {request.appVersion}");
		Debug.Log($"=====lang: {request.lang}");
		Debug.Log($"=====adid: {request.adid}");
		Debug.Log($"=====device: {request.device}");
		Debug.Log($"=====ip: {request.ip}");
		Debug.Log($"=====osversion: {request.osVersion}");

		if (NoticeWeb_Coroutine != null)
			StopCoroutine(NoticeWeb_Coroutine);

		NoticeWeb_Coroutine = StartCoroutine(NoticeWeb_Request(request, callback));
	}

	IEnumerator NoticeWeb_Request(Notice_Request request, Action<Notice_Response> response)
	{
#if UNITY_ANDROID
		Debug.Log("------------NoticeWeb_Request url : " + string.Format("{0}/api/v1/ranking/user/main", ServerUri));

		using (var uwr = new UnityWebRequest(string.Format("{0}/api/v1/ranking/user/main", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => AppServiceController.NoticeWeb_Request()");
			}
			else
			{
				Debug.Log($"success => AppServiceController.NoticeWeb_Request()");
				var result = JsonMapper.ToObject<Notice_Response>(uwr.downloadHandler.text);

                Debug.Log($"===== result reason: {result.reason} / force: {result.force}");
				foreach (var v in result.popup)
				{
					Debug.Log($"===== result url: {v.url} / popuptype: {v.popupType} / left: {v.left} / right: {v.right} / top: {v.top} / bottom: {v.bottom}");
				}

				response.Invoke(result);
			}
		}
#endif
		yield return 0;
	}

	Coroutine GetUserMigrate_Coroutine;
	public void GetUserMigrate(Action callback)
	{		
		if (GetUserMigrate_Coroutine != null)
			StopCoroutine(GetUserMigrate_Coroutine);

		GetUserMigrate_Coroutine = StartCoroutine(UserMigrate_Request(callback));
	}

	IEnumerator UserMigrate_Request( Action callback)
	{
		PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 0);
		PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 0);
		PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0);

		Migrate_Request request = new Migrate_Request();
		request.target_package = "com.Brickgames.RealSwipeBrickBreaker";
		//request.target_package = "com.BigPunchGames.OriginalSwipeBrickBreaker";
		request.prev_package = "com.Monthly23.SwipeBrickBreaker";

#if !TEST
#if UNITY_ANDROID
		request.email = FireBaseController.GetUserEmail();
#endif
#else
		request.email = "nicesunwoo0113@gmail.com";
#endif
        // original swipe
        string beforeToken = string.Empty;
		string migrated_by = string.Empty;

		using (var uwr = new UnityWebRequest(String.Format("{0}/api/v1/inapp/user/migrate", ServerUri), "POST"))
		{
			string jsonData = JsonMapper.ToJson(request);
			byte[] bytes = new System.Text.UTF8Encoding().GetBytes(jsonData);
			uwr.uploadHandler = new UploadHandlerRaw(bytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");
			uwr.SetRequestHeader("x-tdi-client-secret", Static_APP_Config._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => WebRequest.UserMigrate_Request()");
			}
			else
			{
				Debug.Log($"success => AppServiceController.UserMigrate_Request()" + uwr.downloadHandler.text);

				PlayerPrefs.SetInt("UserMigrate_Request_0526", 1);

				if (uwr.downloadHandler.text == "[]")
                {
#if !TEST
					if (callback != null)
                        callback();
#endif
					yield break;
                }

                JsonData resultdata = JsonMapper.ToObject(uwr.downloadHandler.text);
                if (bool.Parse(resultdata["result"].ToString()) == false)
                {
#if !TEST
                    if (callback != null)
                        callback();
#endif
					yield break;
                }

				var responseData = JsonMapper.ToObject<Migrate_Response>(uwr.downloadHandler.text);
				var backupData = new NewBackUpData();

                // AppService 서버에 데이터가 존재.
                if (responseData != null && responseData.result == true && responseData.data != null)
                {
					beforeToken = responseData.data.token;
					migrated_by = responseData.data.migrated_by;
					//if (!responseData.data.is_migrated)
					{
                        backupData.BestScore = responseData.data.BestScore;
                        backupData.LastBackupDate = DateTime.MinValue;

                        if (UserData._Score_Best <= backupData.BestScore)
                            UserData._Score_Best = backupData.BestScore;
                        else
                            backupData.BestScore = UserData._Score_Best;
                    }


                    if (responseData.data.ReceiptIDs != null)
                    {
                        for (int i = 0; i < responseData.data.ReceiptIDs.Count; i++)
                        {
                            if (responseData.data.ReceiptIDs[i] == null) continue;
                            if (i == 0)
                            {
                                Debug.Log("*** UserMigrate_Request 아이템 샀나요?");
                                PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 1);
                                InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
                                InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
                            }
                            if (i == 1)
                            {
                                Debug.Log("*** UserMigrate_Request 아이템 샀나요?");
                                PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 1);
                            }
                            if (i == 2)
                            {
                                Debug.Log("*** UserMigrate_Request 아이템 샀나요?");
                                PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 1);
#if UNITY_ANDROID
                                AdController.Hide_BannerView();
                                AdController.AdPurchase = true;
#endif
                                InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
                                InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
                            }
                        }
                    }
                }
                else // AppService 서버에 데이터가 존재하지 않음.
                {
                    backupData.BestScore = 0;
                    backupData.LastBackupDate = DateTime.MinValue;
                }
#if !TEST
				SetUserBackUpData(backupData, response =>
                {
                    if (response.result)
                        GetIAP();
                });
#endif
			}
        }

        if (beforeToken == string.Empty)
        {
#if !TEST
			if (callback != null)
                callback();
#endif
			yield break;
        }

		var iap_request = new User_IAP_Request();
		iap_request.token = beforeToken;

		var uri = string.Format("{0}?token={1}", string.Format("{0}/api/v1/inapp/user/receipt/get", ServerUri), iap_request.token);

		Debug.Log("------------UserMigrate_Request inapp url : " + uri);

		using (var uwr = UnityWebRequest.Get(uri))
		{
			uwr.SetRequestHeader("Content-Type", "application/json");

			if (migrated_by == "com.Brickgames.RealSwipeBrickBreaker")
            {
                Debug.Log("=================BigPunchGames");

            	uwr.SetRequestHeader("x-tdi-client-secret", "XtKPQO6O61TaTjzkJvhh9LKCXHOSBXwuo1VaBLjvCPNXwjIpZItU8J8BygprJV4ylxvm2PLzSkbEZmUyMi0hq2XmTD5dKxYOecU4YZcRmYDzMOU2Y3RkS2Rfb3X2r");
         	}
			else
			{
				Debug.Log("=================NineGames");
				uwr.SetRequestHeader("x-tdi-client-secret", "33n4X3iCLNSaZxmuVw3Mslr43NrNBnYv6bIbGlqGrnYPh211nCFLKOOzE7jskG4cOcMnFaQu6vEmjmA3nYMGSKXFnaqHqyhBXj8Atj1haPLZNkmlIRepH3wM3r4VK");
			}

            //uwr.SetRequestHeader("x-tdi-client-secret", FN._Client_Secret);

			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError("error => UserMigrate_Request WebRequest.ValidationUserReceipt_Request()");
			}
			else
			{
				Debug.Log($"success => UserMigrate_Request WebRequest.ValidationUserReceipt_Request() : " + uwr.downloadHandler.text);

				var result = JsonMapper.ToObject<User_IAP_Response>(uwr.downloadHandler.text);

                if (!result.result)
                {
#if !TEST
					if (callback != null)
                        callback();
#endif
					yield break;
				}

				foreach (var item in result.data)
				{
					Debug.Log($"UserMigrate_Request ValidationUserReceipt / response.data.productId: {item.productId} / response.data.status: {item.status}");

					switch (item.productId)
					{
						case IAPController.pdSpeed:
							if (item.status == 0 || item.status == 1) // 결제 상태
							{
								Debug.Log("*** UserMigrate_Request 아이템 샀나요?");
								PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 1);
								InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
								InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
							}
							break;
						case IAPController.pdSkipTurn:
							if (item.status == 0 || item.status == 1) // 결제 상태
							{
								Debug.Log("*** UserMigrate_Request 아이템 샀나요?");
								PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 1);
							}
							break;
						case IAPController.pdAdRemoveSpeed:
							if (item.status == 0 || item.status == 1) // 결제 상태
							{
								Debug.Log("*** UserMigrate_Request 아이템 샀나요?");
								PlayerPrefs.SetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 1);
#if UNITY_ANDROID
								AdController.Hide_BannerView();
								AdController.AdPurchase = true;
#endif
								InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
								InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
							}
							break;
					}
				}
			}
        }
#if !TEST
		if (callback != null)
            callback();
#endif
		yield return 0;
	}
}

public class Notice_Request
{
	public string token;
	public string package;
	public string RuntimePlatform;
	public string appVersion;
	public string lang;
	public string adid;
	public string device;
	public string ip;
	public string osVersion;
}

public class Notice_Response
{
	public string reason;
	public bool force;
	public Notice_Response_Item[] popup;
}

public class Notice_Response_Item
{
	public int id;
	public string url;
	public string link;
	public string popupType;
	public double left;
	public double top;
	public double right;
	public double bottom;
}

//Ranking Parsing Class
public class UserFCMToken_Request
{
	public string token;
	public string package;
	public string push_token;
}

public class UserFCMToken_Response
{
	public string reason;
}

//Ranking Parsing Class
public class RankingScore_Request
{
	public string token;
	public string package;
	public int score;
	public string RunTimePlatform;
	public string appVersion;
}

public class RankingScore_Response
{
	public string reason;
	public string message;
}

/// <summary>
/// API Parsing Class
/// </summary>
public class NewBackUpData
{
	public int BestScore;
	public DateTime LastBackupDate;
}

public class SetUserBackUpData_Response
{
	public bool result;
	public string message;
}

public class SetUserBackUpData_Request
{
	public string package;
	public string token;
	//public string email;
	public NewBackUpData user_data;
}

public class GetUserBackUpData_Response
{
	public bool result;
	public NewBackUpData data;
	public string message;
}

public class GetUserBackUpData_Request
{
	public string package;
	public string token;
}

public class CheckNewDBUser_Response
{
	public bool result;
	public bool data;
	public string message;
}

public class CheckNewDBUser_Request
{
	public string package;
	public string token;

	//public string email;
}

public class ReceiptData
{
	public string productId;
	public string purchaseTokenOrPayload;
}

public class UserReceiptData
{
	public string productId;
	public int status;
}

public class User_IAP_Request
{
	public string token;
}

public class User_IAP_Response
{
	public bool result;
	public UserReceiptData[] data;
	public string message;
}

public class IAP_Request
{
	public string token;
	public string package;
	public int osType;
	public int itemType;
	public List<ReceiptData> receiptData;
}

public class IAP_Response
{
	public bool result;
	public UserReceiptData[] data;
	public string message;
}

public class Migrate_Request
{
	public string prev_package;
	public string target_package;
	public string email;
}

public class Migrate_Response
{
	public bool result;
	public MigrateData data;
	public string message;
}

//////////////////////////////////////
// AppService server Get
public class ResponseAppServiceBackUpData
{
	public string result;
	public int code;
	public string message;
	public BackUpData data;
}

// AppService server Post
public class BackUpData
{
	public string token;
	public BackupUserData user_data;
}

public class BackupUserData
{
	public int BestScore;
	public string ProviderId;
	public DateTime LastBackupDate;
	public List<string> ReceiptIDs;
}

public class MigrateData
{
	public int BestScore;
	public string ProviderId;
	public string LastBackupDate;
	public List<string> ReceiptIDs;
	public string token;

	public bool is_migrated;
	public string migrated_by;
	public string migrated_at;
}