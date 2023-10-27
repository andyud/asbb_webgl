using AppleAuth;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#if UNITY_ANDROID
using Firebase;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class PopUp_Login : PopUp
{
	private enum AlertType
	{
		None =0,
		NotConnectionNetwork,
		NotSupported,
		FirebaseAuthFailed,
		Success,
		Failed,
		StateIsSignOut,
		LogoutSuccess,
	}

	private const string CHAR_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
	private const string APPLE_PROVIDER_ID = "apple.com";
	private const string GOOGLE_PROVIDER_ID = "google.com";

	[SerializeField] private UIBasicSprite _wallSprite;
	[SerializeField] private UIBasicSprite _floorSprite;
	
	[SerializeField] private GameObject _appleLoginCheckObject;
	[SerializeField] private GameObject _googleLoginCheckObject;

	[SerializeField] private GameObject _mainPanelObject;

	[SerializeField] private UILabel _title;
	[SerializeField] private UILabel _description;
	[SerializeField] private UILabel _appleLoginLabel;
	[SerializeField] private UILabel _googleLoginLabel;
	[SerializeField] private UILabel _LogoutLabel;

	[SerializeField] private UIButton _googleLoginBtn;
	[SerializeField] private UIButton _appleLoginBtn;
	[SerializeField] private UIButton _closeBtn;
	[SerializeField] private UIButton _logoutBtn;

	[SerializeField] private GameObject _alertObject;
	[SerializeField] private UILabel _alertTitle;
	[SerializeField] private UILabel _alertComment;
	[SerializeField] private UILabel _alertOkBtnLabel;
	[SerializeField] private UIButton _alertOkBtn;
	[SerializeField] private UISprite _alertBG;

	protected override void Initialize_PopUp()
	{
		_floorSprite.color = Static_ColorConfigs._Color_PopupBackGround;
		_title.text = Static_TextConfigs.Login_Title;
		_description.text = Static_TextConfigs.Login_description;
		_alertTitle.trueTypeFont = FontHolder.GetFont();
		_alertComment.trueTypeFont = FontHolder.GetFont();
		_alertOkBtnLabel.trueTypeFont = FontHolder.GetFont();
		
		if (_googleLoginBtn)
		{
			_googleLoginBtn.onClick.Clear();
			_googleLoginBtn.onClick.Add(new EventDelegate(() =>
			{
				GoogleLogin();
			}));
		}
		
		if(_appleLoginBtn)
		{
			_appleLoginBtn.onClick.Clear();
			_appleLoginBtn.onClick.Add(new EventDelegate(() =>
			{
				AppleLogin();
			}));
		}
		
		if(_closeBtn)
		{
			_closeBtn.onClick.Clear();
			_closeBtn.onClick.Add(new EventDelegate(Close));
		}
		Debug.Log(">>> PopUp_Login: Initialize_PopUp 5");
#if UNITY_ANDROID
		if (_alertOkBtn)
		{
			_alertOkBtn.onClick.Clear();
			_alertOkBtn.onClick.Add(new EventDelegate(() =>
			{
				StopCoroutine("Open");
				StartCoroutine(Open(_mainPanelObject));
				StopCoroutine("Close");
				StartCoroutine(Close(_alertObject));
			}));
		}
#endif
		Debug.Log(">>> PopUp_Login: Initialize_PopUp 6");
#if UNITY_ANDROID
		if (_logoutBtn)
		{
			_logoutBtn.onClick.Clear();
			_logoutBtn.onClick.Add(new EventDelegate(() =>
			{
				if(FireBaseController.isLogin)
				{
					FireBaseController.AuthSignOut();
					OpenAlert(AlertType.LogoutSuccess);
				}
				else
				{
					OpenAlert(AlertType.StateIsSignOut);
				}

				SetState();
			}));
		}
#endif
		Debug.Log(">>> PopUp_Login: Initialize_PopUp 7");
		//if (_alertOkBtnLabel)
		//	_alertOkBtnLabel.text = Static_TextConfigs.Btn_OK;
#if UNITY_ANDROID
		if (_appleLoginLabel)
			_appleLoginLabel.text = Static_TextConfigs.Btn_AppleLogin;
#endif
		//if (_googleLoginLabel)
		//	_googleLoginLabel.text = Static_TextConfigs.Btn_GoogleLogin;
		//if (_LogoutLabel)
		//	_LogoutLabel.text = Static_TextConfigs.Btn_Logout;
		Debug.Log(">>> PopUp_Login: Initialize_PopUp 11");
	}

	public override void Refresh()
	{
		_floorSprite.color = Static_ColorConfigs._Color_PopupBackGround;
		SetState();
	}

	public override void SetUI() { }

	private void SetState()
	{
		//_mainPanelObject.SetActive(true);
		//_alertObject.SetActive(false);

		_appleLoginCheckObject.SetActive(false);
		_googleLoginCheckObject.SetActive(false);
		_appleLoginBtn.isEnabled = true;
		_googleLoginBtn.isEnabled = true;
		_logoutBtn.gameObject.SetActive(false);
#if UNITY_ANDROID
		if (FireBaseController.isLogin)
		{
			var providerId = FireBaseController.GetProviderId();
			if (string.IsNullOrEmpty(providerId))
			{
				Debug.LogError("error => Popup_Login.SetState()");
				return;
			}

			if (providerId.Contains(GOOGLE_PROVIDER_ID + APPLE_PROVIDER_ID) ||
			providerId.Contains(APPLE_PROVIDER_ID + GOOGLE_PROVIDER_ID))
			{
				_appleLoginCheckObject.SetActive(true);
				_appleLoginBtn.isEnabled = false;
				_googleLoginCheckObject.SetActive(true);
				_googleLoginBtn.isEnabled = false;
			}
			else if (providerId.Contains(GOOGLE_PROVIDER_ID))
			{
				_googleLoginCheckObject.SetActive(true);
				_googleLoginBtn.isEnabled = false;
			}
			else if (providerId.Contains(APPLE_PROVIDER_ID))
			{
				_appleLoginCheckObject.SetActive(true);
				_appleLoginBtn.isEnabled = false;
			}

			_logoutBtn.gameObject.SetActive(true);
		}
#endif
	}

	private void OpenAlert(AlertType type)
	{
		_alertBG.color = Static_ColorConfigs._Color_Background;

		var title = string.Empty;
		var comment = string.Empty;

		StopCoroutine("Open");
		StartCoroutine(Open(_alertObject));
		StopCoroutine("Close");
		StartCoroutine(Close(_mainPanelObject));

		switch(type)
		{
			case AlertType.Success:
				title = Static_TextConfigs.Login_Success_Title;
				comment = Static_TextConfigs.Login_Success_Comment;
				break;
			case AlertType.NotConnectionNetwork:
				title = Static_TextConfigs.NotConnectionNetwork_Title;
				comment = Static_TextConfigs.NotConnectionNetwork_Comment;
				break;
			case AlertType.Failed:
				title = Static_TextConfigs.Login_Failed_Title;
				comment = Static_TextConfigs.Login_Failed_Comment;
				break;
			case AlertType.NotSupported:
				title = Static_TextConfigs.NotSupported_Title;
				comment = Static_TextConfigs.NotSupported_Comment;
				break;
			case AlertType.FirebaseAuthFailed:
				title = Static_TextConfigs.Login_FirebaseAuthFailed_Title;
				comment = Static_TextConfigs.Login_FirebaseAuthFailed_Comment;
				break;
			case AlertType.StateIsSignOut:
				title = Static_TextConfigs.StateIsSignOut_Title;
				comment = Static_TextConfigs.StateIsSignOut_Comment;
				break;
			case AlertType.LogoutSuccess:
				title = Static_TextConfigs.LogOutSuccess_Title;
				comment = Static_TextConfigs.LogOutSuccess_Comment;
				break;
		}

		_alertTitle.text = title;
		_alertComment.text = comment;
	}

	private void AppleLogin()
	{
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.AppleLoginBtnClick);

		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			OpenAlert(AlertType.NotConnectionNetwork);
			return;
		}

		if (FireBaseController.dependencyStatus != DependencyStatus.Available)
		{
			OpenAlert(AlertType.FirebaseAuthFailed);
			return;
		}

		if (FireBaseController.isLogin)
		{
			FireBaseController.AuthSignOut();
			SetState();
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FirebaseLogController.LogEvent(FirebaseLogType.AppleLoginForIos);
			AppleLoginDeviceForIos();
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			FirebaseLogController.LogEvent(FirebaseLogType.AppleLoginForAos);
			AppleLoginDeviceForAos();
		}
		else
		{
			OpenAlert(AlertType.NotSupported);
			return;
		}
#endif
	}

	private void AppleLoginDeviceForIos()
	{
		var deserializer = new PayloadDeserializer();
		var appleAuthManager = new AppleAuthManager(deserializer);
		StopCoroutine("UpdateAppleAuthManager");
		StartCoroutine("UpdateAppleAuthManager", appleAuthManager);

		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			var rawNonce = GenerateRandomString(32);
			var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

			var loginArgs = new AppleAuthLoginArgs(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName, nonce);

			appleAuthManager.LoginWithAppleId(loginArgs,
				credential =>
				{
					StopCoroutine("UpdateAppleAuthManager");

					var appleIdCredential = credential as IAppleIDCredential;
					if (appleIdCredential != null)
					{
#if UNITY_ANDROID
						FireBaseController.SignInWithApple(appleIdCredential, rawNonce, AppleLoginCallback);
#endif
					}
					else
					{
						Debug.Log("AppleLogin.PerformLoginWithAppleIdAndFirebase() / appleIdCredential is null");
					}
				}, Debug.LogError);
		}
		else
		{
			StopCoroutine("UpdateAppleAuthManager");
			Debug.LogError("error => Popup_Login.AppleLoginDeviceForIos() / AppleAuthManager.IsCurrentPlatformSupported is false");
			OpenAlert(AlertType.NotSupported);
		}
	}

	IEnumerator UpdateAppleAuthManager(AppleAuthManager appleAuthManager)
	{
		while(true)
		{
			if (this.gameObject.activeSelf && appleAuthManager != null)
				appleAuthManager.Update();

			yield return 0;
		}
	}

	private string GenerateSHA256NonceFromRawNonce(string rawNonce)
	{
		Debug.Log("Popup_Login.GenerateSHA256NonceFromRawNonce(string rawNonce)");

		var sha = new SHA256Managed();
		var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
		var hash = sha.ComputeHash(utf8RawNonce);

		var result = string.Empty;
		for (var i = 0; i < hash.Length; i++)
		{
			result += hash[i].ToString("x2");
		}

		Debug.LogFormat("Popup_Login.GenerateSHA256NonceFromRawNonce() / return value: {0}", result);

		return result;
	}

	private string GenerateRandomString(int length)
	{
		Debug.Log("Popup_Login.GenerateRandomString(int length)");

		if (length <= 0)
		{
			return null;
		}

		var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
		var result = string.Empty;
		var remainingLength = length;
		var randomNumberHolder = new byte[1];

		while (remainingLength > 0)
		{
			var randomNumbers = new List<int>(16);
			for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
			{
				cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
				randomNumbers.Add(randomNumberHolder[0]);
			}

			for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
			{
				if (remainingLength == 0)
				{
					break;
				}

				var randomNumber = randomNumbers[randomNumberIndex];
				if (randomNumber < CHAR_SET.Length)
				{
					result += CHAR_SET[randomNumber];
					remainingLength--;
				}
			}
		}

		Debug.LogFormat("Popup_Login.GenerateRandomString(int length) / return value: {0}", result);

		return result;
	}

	private void AppleLoginDeviceForAos()
	{
#if UNITY_ANDROID
		Debug.Log("Popup_Login.AppleLoginDeviceForAos()");

		Firebase.Auth.FederatedOAuthProviderData providerData = new Firebase.Auth.FederatedOAuthProviderData();

		providerData.ProviderId = APPLE_PROVIDER_ID;

		//providerData.Scopes = new List<string> { "email" };

		//providerData.CustomParameters = new Dictionary<string, string>();
		//providerData.CustomParameters.Add("language", "locale");

		Firebase.Auth.FederatedOAuthProvider provider = new Firebase.Auth.FederatedOAuthProvider();
		provider.SetProviderData(providerData);

		FireBaseController.SignInWithApple(provider, AppleLoginCallback);
#endif
	}

	// 애플로 로그인 성공, 실패에 대한 처리
	private void AppleLoginCallback(bool isSuccess, string exception, string userId)
	{
		Debug.Log($"Popup_Login.AppleLoginCallback() / [isSuccess: {isSuccess} exception: {exception} userId: {userId}]");

		SetState();
		OpenAlert(isSuccess ? AlertType.Success : AlertType.Failed);
	}

	private void GoogleLogin()
	{
#if UNITY_ANDROID
		Debug.Log("Popup_Login.GoogleLogin()");
		FirebaseLogController.LogEvent(FirebaseLogType.GoogleLoginBtnClick);

		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			OpenAlert(AlertType.NotConnectionNetwork);
			return;
		}

		if (FireBaseController.dependencyStatus != DependencyStatus.Available)
		{
			OpenAlert(AlertType.FirebaseAuthFailed);
			return;
		}

		if(FireBaseController.isLogin)
		{
			FireBaseController.AuthSignOut();
			SetState();
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			FirebaseLogController.LogEvent(FirebaseLogType.GoogleLoginForIos);
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			FirebaseLogController.LogEvent(FirebaseLogType.GoogleLoginForAos);
		}
		else
		{
			OpenAlert(AlertType.NotSupported);
			return;
		}

		Firebase.Auth.FederatedOAuthProviderData providerData = new Firebase.Auth.FederatedOAuthProviderData();

		providerData.ProviderId = GOOGLE_PROVIDER_ID;

		Firebase.Auth.FederatedOAuthProvider provider = new Firebase.Auth.FederatedOAuthProvider();
		provider.SetProviderData(providerData);

		FireBaseController.SignInWithGoogle(provider, GoogleLoginCallback);
#endif
	}

	// 구글로 로그인 성공, 실패에 대한 처리
	private void GoogleLoginCallback(bool isSuccess, string exception, string userId)
	{
		Debug.Log($"Popup_Login.GoogleLoginCallback() / isSuccess: {isSuccess} exception: {exception} userId: {userId}");

		SetState();
		OpenAlert(isSuccess ? AlertType.Success : AlertType.Failed);
	}

	IEnumerator Open(GameObject obj)
	{
		obj.SetActive(true);

		obj.transform.position = _pivot_CloseEndTransform.position;
		Vector3 startPosition = obj.transform.localPosition;
		obj.transform.position = _pivot_OpenEndTransform.position;
		Vector3 endPosition = obj.transform.localPosition;

		Vector3 startScale = Vector3.one * 1f / 8f;
		Vector3 endScale = Vector3.one;

		AnimationCurve curve = CurveHolder._AcceleratedRising;

		float lerpRatio = 0;
		float durationTime = 0.3f;
		float elapsedTime = 0f;

		obj.transform.localScale = startScale;
		obj.transform.localPosition = startPosition;
		
		while (elapsedTime < durationTime)
		{
			lerpRatio = elapsedTime / durationTime;
			obj.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(lerpRatio));
			obj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(lerpRatio));
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		obj.transform.localScale = endScale;
		obj.transform.localPosition = endPosition;
	}

	IEnumerator Close(GameObject obj)
	{
		obj.transform.position = _pivot_OpenEndTransform.position;
		Vector3 startPosition = obj.transform.localPosition;
		obj.transform.position = _pivot_CloseEndTransform.position;
		Vector3 endPosition = obj.transform.localPosition;

		Vector3 startScale = Vector3.one;
		Vector3 endScale = Vector3.one * 1f / 8f;

		AnimationCurve curve = CurveHolder._AcceleratedRising;

		float lerpRatio = 0;
		float durationTime = 0.3f;
		float elapsedTime = 0f;
	
		obj.transform.localScale = startScale;
		obj.transform.localPosition = startPosition;
		
		while (elapsedTime < durationTime)
		{
			lerpRatio = elapsedTime / durationTime;
			obj.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(lerpRatio));
			obj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(lerpRatio));
			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		obj.transform.localScale = endScale;
		obj.transform.localPosition = endPosition;

		obj.SetActive(false);
	}
}

