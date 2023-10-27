using UnityEngine;
using System;
using Unity.VisualScripting;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class GUIController : MonoBehaviour
{
    public enum State
    {
        TITLE = 0,
        TUTORIAL,
        INGAME,
        GAMEOVER,
    }
    public State _currentState;

    public Director_Title _title;
    public GUI_Score _score;
    public Director_GameOver _gameOver;

    public MenuButton _howToPlay;
    public MenuButton _leaderboard;
    public MenuButton _pause;
    public MenuButton _setting;
    public MenuButton _multiple;
    public MenuButton _store;
	public MenuButton _smartShop;
	public MenuButton PowerMode;
	public MenuButton MoreGames;
	public MenuButton Login;

	public MenuButton Flexible;
	public UIPanel UIPanel_Flexible;

    public PopUpController _popUp;

	[SerializeField] private UIAnchor _topAnchor;
#if UNITY_IOS
	[DllImport("__Internal")]
	static extern void _ShowStatusBar(bool isShow);
#endif

	private float ratio;

    public void Initialize()
    {
		Debug.Log(">>> GUIController::Initialize 1");
        UILabel[] labels = transform.parent.GetComponentsInChildren<UILabel>();
        for (int i = 0; i < labels.Length; i++)
        {
			// revive popup 에서 카운트 다운 버튼 폰트임. 개별적 사용을 위해 예외처리함.
			if (labels[i].trueTypeFont.name.Equals("JALNANOTF"))
				continue;

            labels[i].trueTypeFont = FontHolder.GetFont();
            labels[i].fontStyle = FontStyle.Normal;
        }
		Debug.Log(">>> GUIController::Initialize 2");
		
		var ratio = Static_Calculator.GetScreenWidthRatio();
		if (ratio > 0.65f && ratio <= 0.75f)
		{
			var flexiblePos = _store.transform.localPosition;
			flexiblePos.x += 260f;
			Flexible.transform.localPosition = flexiblePos;
			UIPanel_Flexible.transform.localPosition = new Vector3(Flexible.transform.localPosition.x, Flexible.transform.localPosition.y - 80f, Flexible.transform.localPosition.z);

			var pos = new Vector3(flexiblePos.x - 130f, flexiblePos.y, flexiblePos.z);
			_leaderboard.transform.localPosition = pos;
			pos.x -= 130f;
			_store.transform.localPosition = pos;
			pos.x -= 130f;
			MoreGames.transform.localPosition = pos;

			_multiple.transform.localPosition = new Vector3(-50f, 80f, _multiple.transform.localPosition.z);
			PowerMode.transform.localPosition = new Vector3(-50f, -45f, PowerMode.transform.localPosition.z);
		}
		Debug.Log(">>> GUIController::Initialize 3");
		
		_title.Initialize();
        _score.Initialize();
        _gameOver.Initialize();
		Debug.Log(">>> GUIController::Initialize 4");
		_howToPlay.Initialize(Callback_OpenHowToPlay);
		Debug.Log(">>> GUIController::Initialize 5");
		_leaderboard.Initialize(Callback_OpenLeaderboard);
		Debug.Log(">>> GUIController::Initialize 6");
		_store.Initialize(Callback_Store);
		Debug.Log(">>> GUIController::Initialize 7");
		_pause.Initialize(Callback_OpenPause);
		Debug.Log(">>> GUIController::Initialize 8");
		_setting.Initialize(Callback_Setting);
		Debug.Log(">>> GUIController::Initialize 9");
		_multiple.Initialize(Callback_Multiple);
		Debug.Log(">>> GUIController::Initialize 10");
		PowerMode.Initialize(Callback_PowerMode);
		Debug.Log(">>> GUIController::Initialize 11");
		MoreGames.Initialize(Callback_MoreGames);
        Debug.Log(">>> GUIController::Initialize 12");
		Login.Initialize(Callback_Login);
		Debug.Log(">>> GUIController::Initialize 13");
		Flexible.Initialize(null);
		Debug.Log(">>> GUIController::Initialize 14");
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android)
		{
			_smartShop.Initialize(Callback_SmartShop);
			_smartShop.gameObject.SetActive(SmartShopController.isActivated_Btn);
			SmartShopWebRequest.actionActivated_Btn = (isActivated) =>
			{
				_smartShop.gameObject.SetActive(isActivated);
			};
		}
		else
		{
			_smartShop.gameObject.SetActive(false);
		}
#endif
		PopUpController.Initialize(Callback_PopUpClose);
		Debug.Log(">>> GUIController::Initialize 15");
	}

    protected void Refresh_Instance()
    {
		Debug.Log(">>> GUIController::Refresh_Instance 1");
#if UNITY_ANDROID
		AdController.Show_BannerView();
#endif
    }


    void Callback_Login()
	{
		Debug.Log(">>> GUIController::Callback_Login 1");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
#if UNITY_ANDROID
		else if (FireBaseController.isLogin)
        {
            //todo: 계정 웹뷰  표시
            Debug.Log("=====login web view");
            WebViewController.instance.ShowUserInfoView();
        }
#endif
        else
        {
            PopUpController.Open_Login();
		}
	}

	private void Callback_MoreGames()
	{
		Debug.Log(">>> GUIController::Callback_MoreGames 1");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
		else
		{
            PopUpController.Open_MoreGames();
		}
	}

	void Callback_SmartShop()
	{
		Debug.Log(">>> GUIController::Callback_SmartShop 1");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
		else
		{
			SmartShopController.CallActivitySmartShop();
		}
	}

	void Callback_OpenHowToPlay()
    {
		Debug.Log(">>> GUIController::Callback_OpenHowToPlay 1");
		_howToPlay.gameObject.SetActive(false);
        _popUp.Open_HowToPlay();
    }

    void Callback_OpenLeaderboard()
    {
		Debug.Log(">>> GUIController::Callback_OpenLeaderboard 1");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		Debug.Log("=====LeaderBoard web view");
		// WebViewController.instance.ShowRankingView();
    }

    void Callback_OpenPause()
    {
		Debug.Log(">>> GUIController::Callback_OpenPause 1");
		_pause.gameObject.SetActive(false);
        _popUp.Open_Pause();
    }

    void Callback_Setting()
    {
		Debug.Log(">>> GUIController::Callback_Setting 1");
		_setting.gameObject.SetActive(false);
        _popUp.Open_Setting();
	}

	public void OpenSetting()
    {
		Debug.Log(">>> GUIController::OpenSetting 1");
		_setting.gameObject.SetActive(false);
        _popUp.Open_Setting();
    }

	void Callback_Multiple()
    {
		Debug.Log(">>> GUIController::Callback_Multiple 1");
		if (InGameController._instance.GetState() is GameState_Moving)
		{
			PopUpController.AddAlert(AlertPopUpType.SpeedMode_State_Moving);
			return;
		}

		if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 0) == 0)
		{
			if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward() ||
				PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, 0) > 0)
				InGameController._instance._shooter.SpeedMode();
			else
				PopUpController.AddAlert(AlertPopUpType.SpeedMode_Guide);
		}
		else
		{
			CheckSpeedMode();
		}
	}

	public void CheckSpeedMode()
	{
		Debug.Log(">>> GUIController::CheckSpeedMode 1");
		// 구매 유저에 대한 처리
		if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward())
        {
            if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 1) == 0)
                InGameController._instance._shooter.SpeedMode();
            else
                InGameController._instance._shooter.SpeedMode(false);
        }
        else if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 1) == 0)
        {
            if (PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, 0) <= 0)
            {
                Debug.Log("BtnClick is Show admob");
#if UNITY_ANDROID
                AdController.Show_Reward_Speed();
#endif
            }
            else
            {
                Debug.Log("BtnClick Show SpeedMode");
                InGameController._instance._shooter.SpeedMode();
            }
        }
        else
        {
            Debug.Log("BtnClick Hide SpeedMode");
            InGameController._instance._shooter.SpeedMode(false);
        }
    }

	void Callback_PowerMode()
	{
		Debug.Log(">>> GUIController::Callback_PowerMode 1");
#if UNITY_EDITOR
		if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 0)
		{
			InGameController._instance._shooter.AdRewardPowerModeTurn = PowerMode_Config.PowerModeRewardTurn;
			InGameController._instance._shooter.PowerMode();
		}
		else
		{
			InGameController._instance._shooter.PowerMode(false);
		}
		
		return;
#endif

		if (InGameController._instance.GetState() is GameState_Moving)
		{
			PopUpController.AddAlert(AlertPopUpType.PowerMode_State_Moving);
			return;
		}

		if (InGameController._instance._shooter.OriginBallCount < PowerMode_Config.PowerModeLimitCount)
		{
			PopUpController.AddAlert(AlertPopUpType.PowerMode_LimitCount);
			return;
		}

		if(PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 0)
		{
			if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward() ||
				PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0) > 0)
				InGameController._instance._shooter.PowerMode();
			else
				PopUpController.AddAlert(AlertPopUpType.PowerMode_Guide);
		}
		else
		{
			CheckPowerMode();
		}
	}

	public void CheckPowerMode()
	{
		Debug.Log(">>> GUIController::CheckPowerMode 1");
		if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward())
		{
			if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 0)
			{
				InGameController._instance._shooter.PowerMode();
			}
			else
			{
				InGameController._instance._shooter.PowerMode(false);
			}
		}
		else if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 0)
		{
#if UNITY_ANDROID
			if (PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0) <= 0)
			{
				AdController.Show_Reward_PowerMode();
			}
			else
#endif
			{
				InGameController._instance._shooter.PowerMode();
			}
		}
		else
		{
			InGameController._instance._shooter.PowerMode(false);
		}
	}

    void Callback_Store()
    {
		Debug.Log(">>> GUIController::Callback_Store 1");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
		else if(AppServerController.Instance.IsNewDBInit == false)
		{
			PopUpController.AddAlert(AlertPopUpType.IAP_Loading);
		}
		else
		{
			_store.gameObject.SetActive(false);
			_popUp.Open_Store();
		}
	}

	public void OpenStore()
    {
		Debug.Log(">>> GUIController::OpenStore 1");
		_store.gameObject.SetActive(false);
        _popUp.Open_Store();
    }

    void Callback_PopUpClose()
    {
		Debug.Log(">>> GUIController::Callback_PopUpClose 1");
		Refresh();
    }

    public void Reset()
    {
		Debug.Log(">>> GUIController::Reset 1");
	}


    public void DoEscape()
    {
		Debug.Log(">>> GUIController::DoEscape 1");
		if (_currentState == State.INGAME)
            Callback_OpenPause();
        else
            PopUpController.Open_Quit();
    }

    public void Enter_Title()
    {
		Debug.Log(">>> GUIController::Enter_Title 1");
		_score.Reset();
        ChangeState(State.TITLE);
        _title.Direct();
    }

    public void Enter_Tutorial()
    {
		Debug.Log(">>> GUIController::Enter_Tutorial 1");
		_score.Reset();
        ChangeState(State.TUTORIAL);
    }

    public void Enter_InGame()
    {
		Debug.Log(">>> GUIController::Enter_InGame 1");
		_score.Reset();
        ChangeState(State.INGAME);
    }

    public void Enter_GameOver(int finalScore, bool isNewRecord, System.Action endCallback)
    {
		Debug.Log(">>> GUIController::Enter_GameOver 1");
		ChangeState(State.GAMEOVER);
        _gameOver.Direct(finalScore, isNewRecord, endCallback);
    }

    void ChangeState(State state)
    {
		Debug.Log(">>> GUIController::ChangeState 1");
		PopUpController.Reset();
        _currentState = state;
        Refresh();
    }

    void Refresh()
    {
		Debug.Log(">>> GUIController::Refresh 1");
		if (_currentState == State.TITLE)
        {
			var howToPlayPos = MoreGames.transform.position;
			howToPlayPos.x *= -1f;
			_howToPlay.transform.position = howToPlayPos;
			_howToPlay.gameObject.SetActive(true);
			_pause.gameObject.SetActive(false);
			_setting.transform.position = _store.transform.position;
            _setting.gameObject.SetActive(true);
            _multiple.gameObject.SetActive(false);
            _store.gameObject.SetActive(false);
			_leaderboard.gameObject.SetActive(false);
			_score.gameObject.SetActive(false);
            _title.gameObject.SetActive(true);
            _gameOver.gameObject.SetActive(false);
			PowerMode.gameObject.SetActive(false);
			Flexible.gameObject.SetActive(false);
			//MoreGames.gameObject.SetActive(true);
			Login.gameObject.SetActive(true);

			_title.Reset();
            _title.Refresh();

			UIPanel_Flexible.clipping = UIDrawCall.Clipping.None;

		}
        else if (_currentState == State.INGAME || _currentState == State.TUTORIAL)
        {
            _howToPlay.gameObject.SetActive(true);
            _pause.gameObject.SetActive(true);
            _setting.gameObject.SetActive(true);
            _multiple.gameObject.SetActive(true);
			PowerMode.gameObject.SetActive(true);
			_store.gameObject.SetActive(true);

			//MoreGames.gameObject.SetActive(true);
			_leaderboard.gameObject.SetActive(true);
            _score.gameObject.SetActive(true);
            _title.gameObject.SetActive(false);
            _gameOver.gameObject.SetActive(false);
			Flexible.gameObject.SetActive(true);
			Login.gameObject.SetActive(false);

			if (_currentState == State.INGAME)
			{
				UIPanel_Flexible.clipping = UIDrawCall.Clipping.SoftClip;
				Flexible.Refresh();
			}
		}
        else if (_currentState == State.GAMEOVER)
        {
			_howToPlay.transform.position = _multiple.transform.position;
            _howToPlay.gameObject.SetActive(true);
            _pause.gameObject.SetActive(false);
			_setting.transform.position = _store.transform.position;
            _setting.gameObject.SetActive(true);
            _multiple.gameObject.SetActive(false);
			_store.gameObject.SetActive(false);
			PowerMode.gameObject.SetActive(false);
			MoreGames.gameObject.SetActive(false);
			_leaderboard.gameObject.SetActive(false);
            _score.gameObject.SetActive(false);
            _title.gameObject.SetActive(false);
            _gameOver.gameObject.SetActive(true);
			Flexible.gameObject.SetActive(false);
			Login.gameObject.SetActive(false);

			UIPanel_Flexible.clipping = UIDrawCall.Clipping.None;
		}
	}

    public void MoveNext()
    {
		PopUpController.MoveNext();
        _score.MoveNext();
        _gameOver.MoveNext();
    }

    public void UpdateScore(int score, int delta, bool isNewRecord)
    {
		Debug.Log(">>> GUIController::UpdateScore 1");
		_score.UpdateScore(score, delta, isNewRecord);
    }

	public void SetDarkMode()
	{
		Debug.Log(">>> GUIController::SetDarkMode 1");
		_howToPlay.Refresh();
		_leaderboard.Refresh();
		_pause.Refresh();
		_setting.Refresh();
		_multiple.Refresh();
		_store.Refresh();
		_smartShop.Refresh();
		PowerMode.Refresh();
		Flexible.Refresh();
		MoreGames.Refresh();
		Login.Refresh();
	}


    public void OpenHowToPlay()
    {
		Debug.Log(">>> GUIController::OpenHowToPlay 1");
		_howToPlay.gameObject.SetActive(false);
        PopUpController.Open_HowToPlay_Immediately();
    }
}
