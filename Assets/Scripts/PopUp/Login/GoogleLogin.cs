using LitJson;
using Proyecto26;
using System;
using UnityEngine;


public class GoogleLogin : MonoBehaviour
{
#if UNITY_ANDORID
	private const string clientId = "230799149078-64k6adk0tg90klp90oro4f7245q0meft.apps.googleusercontent.com";
#elif UNITY_IOS
	private const string clientId = "230799149078-j4p9o88mi0u2nolooo945ev5ig4pa8ke.apps.googleusercontent.com";
#else
	private const string clientId = "230799149078-64k6adk0tg90klp90oro4f7245q0meft.apps.googleusercontent.com";
#endif

	public UIBasicSprite Texture_Wall;
	public UIBasicSprite Texture_Floor;

	[SerializeField] private UILabel _title;
	[SerializeField] private UILabel _message;

	[SerializeField] private UIButton _googleTokenBtn;
	[SerializeField] private UIButton _firebaseAuthTokenBtn;
	[SerializeField] private UIButton _closeBtn;

	[SerializeField] private UIInput _inputField;

	private bool isClickGoogleTokenBtn = false;

	public void Init(Action closeBtnClick)
	{
		Texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		Texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;

		if(_googleTokenBtn)
		{
			_googleTokenBtn.onClick.Clear();
			_googleTokenBtn.onClick.Add(new EventDelegate(() =>
			{
				GetGoogleToken();
			}));
		}
		
		if(_firebaseAuthTokenBtn)
		{
			_firebaseAuthTokenBtn.onClick.Clear();
			_firebaseAuthTokenBtn.onClick.Add(new EventDelegate(() =>
			{
				_inputField.gameObject.SetActive(false);
				_firebaseAuthTokenBtn.gameObject.SetActive(false);
				_message.text = "로그인 중입니다... 잠시만 기다려 주세요...";
				ExchangeGoogleTokenWithFirebaseAuthToken(_inputField.value);
			}));
		}
		
		if(_closeBtn)
		{
			_closeBtn.onClick.Clear();
			_closeBtn.onClick.Add(new EventDelegate(() => closeBtnClick?.Invoke()));
		}

		this.gameObject.SetActive(false);
	}

	private void OnApplicationFocus(bool focus)
	{
		if(isClickGoogleTokenBtn && focus)
		{
			isClickGoogleTokenBtn = false;
			_message.text = "복사한 인증코드를 붙여넣기 후\n확인 버튼을 눌러주세요.";

			_inputField.gameObject.SetActive(true);
			_firebaseAuthTokenBtn.gameObject.SetActive(true);

			_googleTokenBtn.gameObject.SetActive(false);
		}
	}

	private void GetGoogleToken()
	{
		Application.OpenURL($"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&scope=email");

		isClickGoogleTokenBtn = true;
	}

	private void ExchangeGoogleTokenWithFirebaseAuthToken(string code)
	{
		RestClient.Post($"https://oauth2.googleapis.com/token?code={code}&client_id={clientId}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code", null).Then
		(
			response =>
			{
				JsonData json = JsonMapper.ToObject(response.Text);

				Debug.LogFormat("PopUp_GoogleLogin.ExchangeGoogleTokenWithFirebaseAuthToken() / Response after request");
#if UNITY_ANDROID
				FireBaseController.SignInWithGoogle(json["id_token"].ToString(), json["access_token"].ToString(), (isSuccess) =>
				{
					_message.text = isSuccess ? "로그인 되었습니다." : "로그인 실패 하였습니다. 잠시 후 다시 시도해 주세요.";
				});
#endif
			}
		).Catch((e) =>
		{
			Debug.LogException(e);
			_message.text = "로그인 실패 하였습니다. 잠시후 다시 시도해 주세요.";
		});
	}

	void OnEnable()
	{
		_title.text = "로그인";
		_title.gameObject.SetActive(true);
		_message.text = string.Empty;
		_message.gameObject.SetActive(true);

		_googleTokenBtn.gameObject.SetActive(false);
		_firebaseAuthTokenBtn.gameObject.SetActive(false);

		_inputField.value = string.Empty;
		_inputField.gameObject.SetActive(false);

		isClickGoogleTokenBtn = false;
#if UNITY_ANDROID
		if (FireBaseController.isLogin)
		{
			_message.text = "로그인 상태 입니다.";
		}
		else
		{
			_message.text = "구글 로그인 요청하기";

			_googleTokenBtn.gameObject.SetActive(true);
		}
#endif
	}
}
