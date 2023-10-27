using UnityEngine;
using System.Collections;

public class Director_Title : MonoBehaviour
{
	public UIBasicSprite _texture_Rating;
	public UIBasicSprite _texture_Share;
	public UIBasicSprite _texture_Ranking;
    public UIBasicSprite _texture_Play;
    public UIBasicSprite _texture_PlaySkip;

    //public UIBasicSprite _texture_FacebookPage_Icon;
	public UIBasicSprite _texture_Rating_Icon;
	public UIBasicSprite _texture_Share_Icon;
	public UIBasicSprite _texture_Ranking_Icon;
	public UIBasicSprite _texture_AD;

	//public UILabel _label_FacebookPage;
	public UILabel _label_Rating;
	public UILabel _label_Share;
	public UILabel _label_Ranking;
	public UILabel _label_Play;
    public UILabel _label_PlaySkip;

	//public UIButton _button_FacebookPage;
	public UIButton _button_Rating;
	public UIButton _button_Share;
	public UIButton _button_Ranking;
	public UIButton _button_Play;
    public UIButton _button_PlaySkip;

	public UIButton _button_BackUp;
	public UILabel _label_BackUp;
	public UIBasicSprite _icon_BackUp;

	System.Text.StringBuilder _strBuilder = new System.Text.StringBuilder();

    public void Initialize()
	{
		//_texture_FacebookPage_Icon.color = ColorConfigs._Color_TextWhite;
		_texture_Rating_Icon.color = Static_ColorConfigs._Color_TextWhite;
		_texture_Share_Icon.color = Static_ColorConfigs._Color_TextWhite;
		_texture_Ranking_Icon.color = Static_ColorConfigs._Color_TextWhite;

		//_texture_FacebookPage_Icon.color = ColorConfigs._Color_TextWhite;
		//_label_FacebookPage.color = ColorConfigs._Color_FacebookBlue;

		_texture_Rating.color = Static_ColorConfigs._Color_TextStrongBlue;
		_label_Rating.color = Static_ColorConfigs._Color_TextStrongBlue;

		_texture_Share.color = Static_ColorConfigs._Color_TextStrongGreen;
		_label_Share.color = Static_ColorConfigs._Color_TextStrongGreen;

		_texture_Ranking.color = Static_ColorConfigs._Color_TextStrongRed;
		_label_Ranking.color = Static_ColorConfigs._Color_TextStrongRed;

		_texture_Play.color = Static_ColorConfigs._Color_TextOrange;
		_label_Play.color = Static_ColorConfigs._Color_TextWhite;

        _texture_PlaySkip.color = Static_ColorConfigs._Color_TextOrange;
        _label_PlaySkip.color = Static_ColorConfigs._Color_TextWhite;

        //_label_FacebookPage.text = TextConfigs._VisitFacebook;
		_label_Rating.text = Static_TextConfigs._Rating;
		_label_Share.text = Static_TextConfigs._Share;
		_label_Ranking.text = Static_TextConfigs.Btn_Ranking;
		_label_Play.text = Static_TextConfigs._Play;
        _label_PlaySkip.text = string.Format( Static_TextConfigs._PlaySkip, (UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn);

        //_button_FacebookPage.onClick.Clear ();
		//_button_FacebookPage.onClick.Add (new EventDelegate (ButtonResponse_FacebookPage));

		_button_Rating.onClick.Clear ();
		_button_Rating.onClick.Add(new EventDelegate(ButtonResponse_Rating));

		_button_Share.onClick.Clear ();
		_button_Share.onClick.Add(new EventDelegate(ButtonResponse_Share));

		_button_Ranking.onClick.Clear ();
		_button_Ranking.onClick.Add(new EventDelegate(ButtonResponse_Ranking));

		_button_Play.onClick.Clear ();
		_button_Play.onClick.Add(new EventDelegate(ButtonResponse_Play));

		_button_PlaySkip.onClick.Clear();
        _button_PlaySkip.onClick.Add(new EventDelegate(ButtonResponse_PlaySkip));

		_button_BackUp.onClick.Clear();
		_button_BackUp.onClick.Add(new EventDelegate(ButtonResponse_BackUp));
		_label_BackUp.color = Static_ColorConfigs._Color_TextStrongNavy;
		

		_label_BackUp.text = Static_TextConfigs.TitleScene_BackUp_Title;
		_icon_BackUp.color = Static_ColorConfigs._Color_TextWhite;
	}

	void ButtonResponse_BackUp()
	{
#if UNITY_ANDROID
		if (FireBaseController.isLogin)
            PopUpController.Open_Backup();
		else
            PopUpController.Open_Login();

		FirebaseLogController.LogEvent(FirebaseLogType.TitleSceneBtn, "BackUp");
#endif
	}

	//void ButtonResponse_FacebookPage()
	//{
	//	if (Application.internetReachability == NetworkReachability.NotReachable)
	//	{
	//		PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
	//	}
	//	else
	//	{
	//		Application.OpenURL(FN._FacebookPage_URL);
	//		FirebaseLogController.LogEvent(FirebaseLogType.TitleSceneBtn, "FacebookPage");
	//	}
	//}

	void ButtonResponse_Rating()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
		else
		{
			Application.OpenURL(Static_APP_Config._Market_URL);
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.TitleSceneBtn, "Rating");
#endif
		}
	}

	void ButtonResponse_Share()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
		else
		{
			NativeShareController.Instance.ShareText();
			//if (Application.platform == RuntimePlatform.IPhonePlayer)
			//{
			//	Share.ShareMessage(FN._Market_URL);
			//}
			//else
			//{
			//	_strBuilder.Remove(0, _strBuilder.Length);
			//	_strBuilder.AppendFormat("{0}\n{1}", TextConfigs._ShareMessage, FN._Market_URL);
			//	Share.ShareMessage(_strBuilder.ToString());
			//}
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.TitleSceneBtn, "Share");
#endif
		}
	}

	void ButtonResponse_Ranking()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		Debug.Log("=====Ranking Web View");
		// WebViewController.instance.ShowRankingView();
	}

	void ButtonResponse_Play()
	{
		Main.StartGame ();
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.TitleSceneBtn, "Play");
#endif
	}
    
    void ButtonResponse_PlaySkip()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
			if (StaticMethod.IsPurchase_SkipGame_Total_SmartShopReward())
				Main._main.DelayStartSkipGame();
#if UNITY_ANDROID
			else
			{
				AdController.Show_Reward_SkipGame(false);
			}
#endif   
        }
        else
            Main._main.DelayStartSkipGame();
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.TitleSceneBtn, "PlaySkip");
#endif
	}

    public void Reset(){}

	public void Refresh()
	{
        _label_PlaySkip.text = string.Format(Static_TextConfigs._PlaySkip, (UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn);

		Direct();
	}
	
	public void Direct ()
	{
		// 이어하기 버튼 표시 체크
		if ((UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn > 0)
			_button_PlaySkip.gameObject.SetActive(true);
		else
			_button_PlaySkip.gameObject.SetActive(false);

		// 이어하기 아이콘 표시 체크
		//if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Continue, 0) == 1 || PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 1)
		//if(StaticMethod.IsPurchase_SkipGame_Total_SmartShopReward())
		//	_texture_AD.gameObject.SetActive(false);
		//else
		//	_texture_AD.gameObject.SetActive(true);
	}

	private void Update()
	{
#if UNITY_ANDROID
		if(FireBaseController.ConfigData != null && FireBaseController.ConfigData.IsBackUpFeature && AppServerController.Instance.IsNewDBInit)
		{
			_button_BackUp.gameObject.SetActive(true);
		}
		else
#endif
		{
			_button_BackUp.gameObject.SetActive(false);
		}

		// 이어하기 아이콘 표시 체크
		if (StaticMethod.IsPurchase_SkipGame_Total_SmartShopReward())
			_texture_AD.gameObject.SetActive(false);
		else
			_texture_AD.gameObject.SetActive(true);
	}
}
