using UnityEngine;
using System.Collections;

public class PopUp_Store : PopUp
{
    public UIBasicSprite _texture_Wall;
    public UIBasicSprite _texture_Floor;
    public UILabel _label_Title;

	// 아이템 가격
    public UILabel _label_BallSpeed;
    public UILabel _label_SkipTurn;
    public UILabel _label_AdRemove_BallSpeed_SkipTurn;

    public UILabel _label_Policy;

	// 아이템 이름
    public UILabel _label_Speed;
    public UILabel _label_Skip;
    public UILabel _label_AdSpeedSkip;

	public UILabel _label_Close;

	public UISprite _sprite_BallSpeed;
	public UISprite _sprite_SkipTurn;
	public UISprite _sprite_AdRemove_BallSpeed;
	public UISprite _sprite_BallSpeed_Check;
    public UISprite _sprite_SkipTurn_Check;
    public UISprite _sprite_AdRemove_BallSpeed_Check;

    public UIButton _button_BallSpeed;
    public UIButton _button_SkipTurn;
    public UIButton _button_AdRemove_BallSpeed;
    public UIButton _button_Policy;
    public UIButton _button_Close;

	public UIButton Button_Restore;
	public UILabel Label_Restore;
	public UISprite Sprite_Processing;
	private bool isRestoreEnd;

	private bool isSpeedPurchase = false;
    private bool isSkipTurnPurchase = false;
    private bool isAdSpeedPurchase = false;

    protected override void Initialize_PopUp()
    {
        _texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
        _texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
		_label_Title.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_BallSpeed.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_SkipTurn.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_AdRemove_BallSpeed_SkipTurn.color = Static_ColorConfigs._Color_ButtonFrame;
        //_label_Policy.color = ColorConfigs._Color_ButtonFrame;
        _label_Speed.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_Skip.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_AdSpeedSkip.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_Close.color = Static_ColorConfigs._Color_ButtonFrame;
		Label_Restore.color = Static_ColorConfigs._Color_ButtonFrame;

		_label_Title.text = Static_TextConfigs._Store;
        _label_BallSpeed.text = string.Empty;
        _label_SkipTurn.text = string.Empty;
        _label_AdRemove_BallSpeed_SkipTurn.text = string.Empty;
        _label_Policy.text = Static_TextConfigs._Policy;
        _label_Speed.text = Static_TextConfigs._IAP_Speed;
        _label_Skip.text = Static_TextConfigs._IAP_SkipTurn;
        _label_AdSpeedSkip.text = Static_TextConfigs._IAP_AdSpeed;
        _label_Close.text = Static_TextConfigs._Close;
		Label_Restore.text = Static_TextConfigs.Btn_Restore;

		Sprite_Processing.gameObject.SetActive(false);

		//isSpeedPurchase = PlayerPrefs.GetInt("SpeedPurchase", 0) == 0 ? false : true;
		//isSkipTurnPurchase = PlayerPrefs.GetInt("SkipTurnPurchase", 0) == 0 ? false : true;
		//isAdSpeedPurchase = PlayerPrefs.GetInt("AdSpeedPurchase", 0) == 0 ? false : true;
		isSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Speed, 0) == 0 ? false : true;
		isSkipTurnPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Continue, 0) == 0 ? false : true;
		isAdSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 0 ? false : true;
		
		if(!isSpeedPurchase)
			isSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 0) == 0 ? false : true;

		if (!isSkipTurnPurchase)
			isSkipTurnPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 0) == 0 ? false : true;

		if (!isAdSpeedPurchase)
			isAdSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) == 0 ? false : true;

		if (isSpeedPurchase)
        {
            _sprite_BallSpeed_Check.enabled = false;
            _button_BallSpeed.enabled = false;
            //Debug.LogError("IAP isSpeedPurchase : " + isSpeedPurchase);
        }
        if (isSkipTurnPurchase)
        {
            _sprite_SkipTurn_Check.enabled = false;
            _button_SkipTurn.enabled = false;
            //Debug.LogError("IAP isSpeedPurchase : " + isSpeedPurchase);
        }
        if (isAdSpeedPurchase)
        {
            _sprite_AdRemove_BallSpeed_Check.enabled = false;
            _button_AdRemove_BallSpeed.enabled = false;
            //Debug.LogError("IAP isAdSpeedPurchase : " + isAdSpeedPurchase);
        }

        _button_BallSpeed.onClick.Clear();
        _button_BallSpeed.onClick.Add(new EventDelegate(ButtonResponse_BallSpeed));

        _button_SkipTurn.onClick.Clear();
        _button_SkipTurn.onClick.Add(new EventDelegate(ButtonResponse_SkipTurn));

        _button_AdRemove_BallSpeed.onClick.Clear();
        _button_AdRemove_BallSpeed.onClick.Add(new EventDelegate(ButtonResponse_AdRemove_BallSpeed));

        _button_Policy.onClick.Clear();
        _button_Policy.onClick.Add(new EventDelegate(ButtonResponse_Policy));

        _button_Close.onClick.Clear();
        _button_Close.onClick.Add(new EventDelegate(ButtonResponse_Close));

		// 구매 복구 aos 자동, ios 수동.
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Button_Restore.gameObject.SetActive(true);
			Button_Restore.onClick.Clear();
			Button_Restore.onClick.Add(new EventDelegate(ButtonResponse_Restore));

			// 버튼 위치 조정..
			var posClose = _button_Close.transform.localPosition;
			posClose.x = 200f;
			_button_Close.transform.localPosition = posClose;

			var posRestore = Button_Restore.transform.localPosition;
			posRestore.x = -200f;
			Button_Restore.transform.localPosition = posRestore;
		}
		else
		{
			var pos = _button_Close.transform.localPosition;
			pos.x = 0f;
			_button_Close.transform.localPosition = pos;
		
			Button_Restore.gameObject.SetActive(false);
		}
	}

    void ButtonResponse_BallSpeed()
    {
		//Firebase.Analytics.FirebaseAnalytics.LogEvent("IAP_CLICK_BALLSPEED");
		IAPController.instance.Purchase(IAPTYPE.BALLSPEED);
    }

    void ButtonResponse_SkipTurn()
    {
		//Firebase.Analytics.FirebaseAnalytics.LogEvent("IAP_CLICK_BALLSPEED");
		IAPController.instance.Purchase(IAPTYPE.SKIPTURN);
    }

    void ButtonResponse_AdRemove_BallSpeed()
    {
		//Firebase.Analytics.FirebaseAnalytics.LogEvent("IAP_CLICK_AD+SPEED");
		IAPController.instance.Purchase(IAPTYPE.ADREMOVE_BALLSPEED);
    }

    void ButtonResponse_Policy()
    {
        Application.OpenURL(Static_APP_Config._Refund_URL);
    }

    void ButtonResponse_Close()
    {
        Close();
    }

	void ButtonResponse_Restore()
	{
		// 네트워크 체크.
		if (NetworkReachability.NotReachable == Application.internetReachability)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		isRestoreEnd = false;

		StopCoroutine(RestoreInProcessing());
		StartCoroutine(RestoreInProcessing());

		IAPController.instance.Restore(() =>
		{
			isRestoreEnd = true;
		});
	}

	IEnumerator RestoreInProcessing()
	{
		float durationTime = 1f;
		float elapsedTime = 0f;
		float ratioLerp = 0f;
		var start = Vector3.zero;
		var end = new Vector3(0f, 0f, 360f);
		Time.timeScale = 1.25f;

		Sprite_Processing.gameObject.SetActive(true);

		while (isRestoreEnd == false)
		{
			if (elapsedTime > durationTime)
				elapsedTime = 0;

			ratioLerp = elapsedTime / durationTime;

			Sprite_Processing.transform.localEulerAngles = Vector3.Lerp(start, end, ratioLerp);

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		Sprite_Processing.transform.localEulerAngles = start;
		Sprite_Processing.gameObject.SetActive(false);
	}

    public override void Refresh()
    {
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}

    public override void SetUI()
    {
		StopCoroutine(RestoreInProcessing());
		Sprite_Processing.gameObject.SetActive(false);


		_label_BallSpeed.text = IAPController.instance.costSpeed;
        _label_SkipTurn.text = IAPController.instance.costSkipTurn;
        _label_AdRemove_BallSpeed_SkipTurn.text = IAPController.instance.costAdRemoveSpeed;

		//isSpeedPurchase = PlayerPrefs.GetInt("SpeedPurchase", 0) == 0 ? false : true;
		//isSkipTurnPurchase = PlayerPrefs.GetInt("SkipTurnPurchase", 0) == 0 ? false : true;
		//isAdSpeedPurchase = PlayerPrefs.GetInt("AdSpeedPurchase", 0) == 0 ? false : true;

		isSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Speed, 0) == 0 ? false : true;
		isSkipTurnPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Continue, 0) == 0 ? false : true;
		isAdSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 0 ? false : true;

		if (!isSpeedPurchase)
			isSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 0) == 0 ? false : true;

		if (!isSkipTurnPurchase)
			isSkipTurnPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 0) == 0 ? false : true;

		if (!isAdSpeedPurchase)
			isAdSpeedPurchase = PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) == 0 ? false : true;

		// 이전정보 불러온 유저중에 상점에 표시는 잘 되는데 광고가 나오는 경우가 있어서 여기서 다시한번 체크
		if (isAdSpeedPurchase)
		{
#if UNITY_ANDROID
			AdController.Hide_BannerView();
			AdController.AdPurchase = true;
#endif
			InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
			InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
		}

		_sprite_BallSpeed_Check.enabled = false;
        _sprite_SkipTurn_Check.enabled = false;
        _sprite_AdRemove_BallSpeed_Check.enabled = false;

		_button_BallSpeed.enabled = true;
		_button_SkipTurn.enabled = true;
		_button_AdRemove_BallSpeed.enabled = true;

		if (isSpeedPurchase)
        {
            _sprite_BallSpeed_Check.enabled = true;
            _button_BallSpeed.enabled = false;
        }
        if(isSkipTurnPurchase)
        {
            _sprite_SkipTurn_Check.enabled = true;
            _button_SkipTurn.enabled = false;
        }
        if (isAdSpeedPurchase)
        {
            //_sprite_BallSpeed_Check.enabled = true;
            //_button_BallSpeed.enabled = false;
            //_sprite_SkipTurn_Check.enabled = true;
            //_button_SkipTurn.enabled = false;
            _sprite_AdRemove_BallSpeed_Check.enabled = true;
            _button_AdRemove_BallSpeed.enabled = false;
        }

		if(UserData._OnDarkMode)
		{
			_sprite_BallSpeed.color = Static_ColorConfigs._Color_PopupBackGround;
			_sprite_SkipTurn.color = Static_ColorConfigs._Color_PopupBackGround;
			_sprite_AdRemove_BallSpeed.color = Static_ColorConfigs._Color_PopupBackGround;
		}
		else
		{
			_sprite_BallSpeed.color = Color.white;
			_sprite_SkipTurn.color = Color.white;
			_sprite_AdRemove_BallSpeed.color = Color.white;
		}
    }
}
