using AppleAuth;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AppleLogin : MonoBehaviour
{
	private const string CHAR_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";

	public UIBasicSprite Texture_Wall;
	public UIBasicSprite Texture_Floor;

	[SerializeField] private UIButton _quickLoginBtn;
	[SerializeField] private UIButton _loginBtn;
	[SerializeField] private UIButton _closeBtn;

	private IAppleAuthManager _appleAuthManager;

	private string _userIdKey;

	public void Init(Action closeBtnClick)
	{
		Debug.Log("AppleLogin.Init()");

		Texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		Texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;

		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			var deserializer = new PayloadDeserializer();
			this._appleAuthManager = new AppleAuthManager(deserializer);

			_appleAuthManager.SetCredentialsRevokedCallback(result =>
			{
				Debug.Log("SetCredentialsRevokedCallback / result: " + result);
			});
		}

		_quickLoginBtn.onClick.Clear();
		_loginBtn.onClick.Clear();
		_closeBtn.onClick.Clear();
		_quickLoginBtn.onClick.Add(new EventDelegate(PerformQuickLoginWithFirebase));
		_loginBtn.onClick.Add(new EventDelegate(PerformLoginWithAppleIdAndFirebase));
		_closeBtn.onClick.Add(new EventDelegate(() => closeBtnClick?.Invoke()));

		this.gameObject.SetActive(false);
	}

	private void PerformQuickLoginWithFirebase()
	{
		Debug.Log("AppleLogin.PerformQuickLoginWithFirebase()");

		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			Debug.Log("AppleLogin.PerformQuickLoginWithFirebase() /  AppleAuthManager.IsCurrentPlatformSupported is true");

			var rawNonce = GenerateRandomString(32);
			var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

			var quickLoginArgs = new AppleAuthQuickLoginArgs(nonce);

			this._appleAuthManager.QuickLogin(quickLoginArgs,
				credential =>
				{
					Debug.Log("AppleLogin.PerformQuickLoginWithFirebase() /  QuickLogin");

					var appleIdCredential = credential as IAppleIDCredential;
					if (appleIdCredential != null)
					{
#if UNITY_ANDROID
						FireBaseController.SignInWithApple(appleIdCredential, rawNonce, null);
#endif
					}
					else
					{
						Debug.Log("AppleLogin.PerformQuickLoginWithFirebase() /  appleIdCredential is null");
					}
				}, Debug.LogError);
		}
		else
		{
			Debug.Log("AppleLogin.PerformQuickLoginWithFirebase() /  AppleAuthManager.IsCurrentPlatformSupported is false");
			AppleLoginForAndroid();
		}
	}

	private void PerformLoginWithAppleIdAndFirebase()
	{
		Debug.Log("AppleLogin.PerformLoginWithAppleIdAndFirebase()");

		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			Debug.Log("AppleLogin.PerformLoginWithAppleIdAndFirebase() /  AppleAuthManager.IsCurrentPlatformSupported is true");

			var rawNonce = GenerateRandomString(32);
			var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

			var loginArgs = new AppleAuthLoginArgs(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName, nonce);

			this._appleAuthManager.LoginWithAppleId(loginArgs,
				credential =>
				{
					Debug.Log("AppleLogin.PerformLoginWithAppleIdAndFirebase() / LoginWithAppleId");

					var appleIdCredential = credential as IAppleIDCredential;
					if (appleIdCredential != null)
					{
#if UNITY_ANDROID
						FireBaseController.SignInWithApple(appleIdCredential, rawNonce, null);
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
			Debug.Log("AppleLogin.PerformLoginWithAppleIdAndFirebase() /  AppleAuthManager.IsCurrentPlatformSupported is false");
			AppleLoginForAndroid();
		}
	}

	private void AppleLoginForAndroid()
	{
#if UNITY_ANDROID
		Debug.Log("AppleLogin.AppleLoginForAndroid()");

		Firebase.Auth.FederatedOAuthProviderData providerData = new Firebase.Auth.FederatedOAuthProviderData();

		providerData.ProviderId = "apple.com";

		providerData.Scopes = new List<string> { "email"};

		//providerData.CustomParameters = new Dictionary<string, string>();
		//providerData.CustomParameters.Add("language", "locale");

		Firebase.Auth.FederatedOAuthProvider provider = new Firebase.Auth.FederatedOAuthProvider();
		provider.SetProviderData(providerData);

		FireBaseController.SignInWithAppleWithAOS(provider);
#endif
	}

	private string GenerateSHA256NonceFromRawNonce(string rawNonce)
	{
		Debug.Log("AppleLogin.GenerateSHA256NonceFromRawNonce(string rawNonce)");

		var sha = new SHA256Managed();
		var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
		var hash = sha.ComputeHash(utf8RawNonce);

		var result = string.Empty;
		for(var i=0; i<hash.Length; i++)
		{
			result += hash[i].ToString("x2");
		}

		Debug.Log("AppleLogin.GenerateSHA256NonceFromRawNonce(string rawNonce) / result: " + result);

		return result;
	}

	private string GenerateRandomString(int length)
	{
		Debug.Log("AppleLogin.GenerateRandomString(int length)");

		if(length <= 0)
		{
			throw new Exception("AppleLogin.GenerateRandomString(int length) / length is <= 0");
		}

		var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
		var result = string.Empty;
		var remainingLength = length;
		var randomNumberHolder = new byte[1];
		
		while(remainingLength > 0)
		{
			var randomNumbers = new List<int>(16);
			for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
			{
				cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
				randomNumbers.Add(randomNumberHolder[0]);
			}

			for(var randomNumberIndex =0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
			{
				if(remainingLength == 0)
				{
					break;
				}

				var randomNumber = randomNumbers[randomNumberIndex];
				if(randomNumber < CHAR_SET.Length)
				{
					result += CHAR_SET[randomNumber];
					remainingLength--;
				}
			}
		}

		Debug.Log("AppleLogin.GenerateRandomString(int length) / result: " + result);

		return result;
	}

	private void CheckCredentialState()
	{
		if (string.IsNullOrEmpty(_userIdKey))
		{
			Debug.Log("AppleLogin.CheckCredentialState() / _userIdKey is nullOrEmpty");
			return;
		}

		this._appleAuthManager.GetCredentialState(_userIdKey,
			state =>
			{
				switch (state)
				{
					case AppleAuth.Enums.CredentialState.Authorized:
						Debug.Log("AppleLogin.CheckCredentialState() / state is Authorized");
						break;
					case AppleAuth.Enums.CredentialState.Revoked:
						Debug.Log("AppleLogin.CheckCredentialState() / state is Revoked");
						break;
					case AppleAuth.Enums.CredentialState.NotFound:
						Debug.Log("AppleLogin.CheckCredentialState() / state is NotFound");
						break;
				}
			}, Debug.LogError);
	}

	void Update()
	{
		if(this.gameObject.activeSelf && this._appleAuthManager != null)
			this._appleAuthManager.Update();
	}
}
