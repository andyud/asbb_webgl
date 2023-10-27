using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertInfo
{
	public AlertPopUpType Type;
	public bool IsPause;

	public AlertInfo(AlertPopUpType type, bool ispause)
	{
		Type = type;
		IsPause = ispause;
	}
}

public class PopUpController : MonoBehaviour
{
	static PopUpController _instance;

	static float _directScaleMinRatio = 1f/8f;
	static float _directDuration = 0.3f;

	protected static PopUp _currentPopUp;
	public static bool _IsPopUpOpened { get { return _currentPopUp != null ? true : false; } }
	static IEnumerator _thread_Director;

	public UIBasicSprite _alphaScreen;
	protected static float _endAlpha = 0.7f;

	public UIBasicSprite _webViewDim;

	public PopUp _quit;
	public PopUp _requestRating;
	public PopUp _requestFacebook;
	public PopUp _marketUpdate;
	public PopUp _Consent;
	public PopUp _LoginNotice;

    public PopUp _pause;
    public HowToPlay _howToPlay;
    public PopUp _setting;

    public PopUp _store;
    public AlertPopUp _alert;
    public NoticePopUp _smartShopNotice;

    public PopUp _login;
    public PopUp _revive;
    public PopUp MoreGames;
    public PopUp IosIDFA;
    public PopUp Backup;

    protected static System.Action _callback_Close;
	static System.Action _callback_PromoPopUpClose;

	private static List<AlertInfo> AlertStackList;

	public static PopUp GetCurrentPopup()
	{
		return _currentPopUp;
	}

	public static void ActiveAlphaScreen(bool isActive)
	{
		_instance._webViewDim.gameObject.SetActive(isActive);
		_instance._webViewDim.alpha = isActive ? _endAlpha : 0f;
	}

	public static void SetActivateGameObject(bool b)
	{
        if (_instance!=null && _instance.gameObject != null)
        {
			_instance.gameObject.SetActive(b);
		}
	}

	public static void Initialize(System.Action closeCallback)
	{
		Debug.Log(">>> PopUpController:Initialize 1");
		_instance = FindObjectOfType<PopUpController> ();

		_callback_Close = closeCallback;		
		_instance._alphaScreen.color = Static_ColorConfigs._Color_TextWhite;
		Debug.Log(">>> PopUpController:Initialize 2");
		_instance._quit.Initialize (CloseCurrentPopUp);
		_instance._requestRating.Initialize (CloseCurrentPopUp);
		//_instance._requestFacebook.Initialize (CloseCurrentPopUp);
		Debug.Log(">>> PopUpController:Initialize 3");
		//	_instance._LoginNotice.Initialize(CloseCurrentPopUp);
		Debug.Log(">>> PopUpController:Initialize 4");
		_instance._pause.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 5");
		_instance._setting.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 6");
		_instance._howToPlay.Initialize(Callback_PopUpClose);
        //_instance._store.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 7");
		_instance._alert.Initialize(Callback_PopUpClose);
        _instance._smartShopNotice.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 8");
		if (_instance._login)
            _instance._login.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 9");
		if (_instance._revive)
            _instance._revive.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 10");
		//_instance.MoreGames.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 11");
		//_instance.IosIDFA.Initialize(Callback_PopUpClose);
		Debug.Log(">>> PopUpController:Initialize 12");
		//if (_instance.Backup)
  //          _instance.Backup.Initialize(Callback_PopUpClose);
		Reset ();
		
		_instance.StartCoroutine(_instance.AlertStack());

        _instance._webViewDim.gameObject.SetActive(false);
        Debug.Log(">>> PopUpController:Initialize 13");
	}


    static void Callback_PopUpClose()
    {
		Debug.Log(">>> PopUpController:Callback_PopUpClose 1");
		if (_currentPopUp == _instance._howToPlay)
        {
			Debug.Log(">>> PopUpController:Callback_PopUpClose 2");
			UserData._IsTutorialCleared = true;
			Debug.Log(">>> PopUpController:Callback_PopUpClose 3");
		}
        
        if (_currentPopUp == _instance._setting)
        {
			Debug.Log(">>> PopUpController:Callback_PopUpClose 4");
			UserData._ShowContactPoint_Selected = true;
			// WebViewController.instance.ClearWebView();
			Debug.Log(">>> PopUpController:Callback_PopUpClose 5");
		}
		Debug.Log(">>> PopUpController:Callback_PopUpClose 6");
		CloseCurrentPopUp();
		Debug.Log(">>> PopUpController:Callback_PopUpClose 3");
	}

    public static void AddAlert(AlertPopUpType type, bool isPause = true)
	{
		if (AlertStackList == null)
		{
			AlertStackList = new List<AlertInfo>();
		}

		AlertStackList.Add(new AlertInfo(type, isPause));
	}

	IEnumerator AlertStack()
	{
		var waitUntil = new WaitUntil(() => PopUpController._IsPopUpOpened == false);
		var waitForEndOffFrame = new WaitForEndOfFrame();

		while (true)
		{
			if (AlertStackList != null && AlertStackList.Count > 0)
			{
				yield return waitUntil;

                PopUpController.Open_Alert(AlertStackList[0].Type, AlertStackList[0].IsPause);
				AlertStackList.RemoveAt(0);

				yield return waitForEndOffFrame;
			}
			yield return waitForEndOffFrame;
		}
	}

	static Coroutine _coWait;
	public static void Reset()
	{
		_currentPopUp = null;
		_thread_Director = null;

		_instance._alphaScreen.gameObject.SetActive (false);
		_instance._alphaScreen.alpha = 0;

		_instance._quit.gameObject.SetActive (false);
		_instance._requestRating.gameObject.SetActive (false);
		_instance._requestFacebook.gameObject.SetActive (false);
		
		_instance._LoginNotice.gameObject.SetActive(false); 

		if (_instance._marketUpdate != null)
			_instance._marketUpdate.gameObject.SetActive(false);

		if (_instance._Consent != null)
			_instance._Consent.gameObject.SetActive(false);

        _instance._pause.gameObject.SetActive(false);
        _instance._setting.gameObject.SetActive(false);

        _instance._store.gameObject.SetActive(false);
        _instance._howToPlay.gameObject.SetActive(false);
        _instance._alert.gameObject.SetActive(false);
        _instance._smartShopNotice.gameObject.SetActive(false);
        _instance.MoreGames.gameObject.SetActive(false);
        _instance.IosIDFA.gameObject.SetActive(false);

        if (_instance._login)
            _instance._login.gameObject.SetActive(false);

        if (_instance._revive)
            _instance._revive.gameObject.SetActive(false);

        if (_instance.Backup)
            _instance.Backup.gameObject.SetActive(false);

        _instance._howToPlay.Reset();


        if (_coWait != null) _instance.StopCoroutine(_coWait);
		_coWait = _instance.StartCoroutine(_instance.ResetWait());
	}

	IEnumerator ResetWait()
    {
		while (WebViewController.instance == null)
		yield return null;

		if (WebViewController.instance.noticeWebView == null)
		Main.UnPause();
			}

	public static void Open_Quit()
	{
		OpenPopUp(_instance._quit);
	}

	public static void Open_RequestRating(System.Action closeCallback)
	{
		//AdController.Hide_BannerView ();
#if UNITY_ANDROID
        // WebViewController.instance.ClearWebView();
#endif
        _callback_PromoPopUpClose = closeCallback;
		OpenPopUp (_instance._requestRating);
	}

	public static void Open_RequestFacebook(System.Action closeCallback)
	{
#if UNITY_ANDROID
		AdController.Hide_BannerView ();
#endif
        //WebViewController.instance.ClearWebView();
        _callback_PromoPopUpClose = closeCallback;
		OpenPopUp (_instance._requestFacebook);
	}

	public static void Open_MarketUpdate()
	{
		OpenPopUp(_instance._marketUpdate);
	}

	public static void Open_Consent()
	{
		//OpenPopUp(_instance._Consent);
	}

	public static void Open_LoginNotice()
	{
		OpenPopUp(_instance._LoginNotice);
	}


	protected static void OpenAlertPopup(PopUp popUp, bool isPause = true)
	{
		if(isPause)
			Main.Pause();

		_currentPopUp = popUp;
		_thread_Director = Direct_Open(_currentPopUp, _directDuration);
	}

	protected static void OpenPopUp(PopUp popUp, bool isPause = true)
	{
		if(!_IsPopUpOpened)
		{
			if(isPause)
				Main.Pause ();

			_currentPopUp = popUp;
			_thread_Director = Direct_Open (_currentPopUp, _directDuration);
		}
	}

	protected static void OpenPopUp_Immediately(PopUp popUp)
	{
		if(!_IsPopUpOpened)
		{
			Main.Pause ();
			_currentPopUp = popUp;
			_thread_Director = Direct_Open (_currentPopUp, 0);
		}
	}

	public static void CloseCurrentPopUp()
	{
		if(_currentPopUp is PopUp_Setting)
		{
			UserData._ShowContactPoint_Selected = true;
		}

		if(_IsPopUpOpened && _thread_Director == null)
			_thread_Director = Direct_Close (_currentPopUp, _directDuration);
	}

	static IEnumerator Direct_Open(PopUp popUp, float duration)
	{
		_instance._alphaScreen.gameObject.SetActive (true);

		popUp.gameObject.SetActive (true);
		popUp.Refresh();
		popUp.EnableButtons (false);
		
		popUp.transform.position = popUp._pivot_CloseEndTransform.position;
		Vector3 startPosition = popUp.transform.localPosition;
		popUp.transform.position = popUp._pivot_OpenEndTransform.position;
		Vector3 endPosition = popUp.transform.localPosition;
		
		Vector3 startScale = Vector3.one * _directScaleMinRatio;
		Vector3 endScale = Vector3.one;

		float startAlpha = 0;
		float endAlpha = _endAlpha;
		AnimationCurve curve =  CurveHolder._AcceleratedRising;

		float lerpRatio = 0;
		IEnumerator thread = CoroutineUtil.RealTimeThread(duration, (float returnValue)=>{lerpRatio = returnValue;}, null);
		popUp.transform.localScale = startScale;
		popUp.transform.localPosition = startPosition;
		_instance._alphaScreen.alpha = startAlpha;
		while(thread.MoveNext())
		{
			yield return thread;
			popUp.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(lerpRatio));
			popUp.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(lerpRatio));
			_instance._alphaScreen.alpha = Mathf.Lerp(startAlpha, endAlpha, lerpRatio*2);
		}
		popUp.transform.localScale = endScale;
		popUp.transform.localPosition = endPosition;
		_instance._alphaScreen.alpha = endAlpha;
		
		popUp.EnableButtons (true);
	}
	
	static IEnumerator Direct_Close(PopUp popUp, float duration)
	{
		Debug.Log(">>> PopUpController: Direct_Close 1");
		if (popUp == _instance._marketUpdate)
			yield break;

		_instance._alphaScreen.gameObject.SetActive (true);
		popUp.EnableButtons (false);
		
		popUp.transform.position = popUp._pivot_OpenEndTransform.position;
		Vector3 startPosition = popUp.transform.localPosition;
		popUp.transform.position = popUp._pivot_CloseEndTransform.position;
		Vector3 endPosition = popUp.transform.localPosition;
		
		Vector3 startScale = Vector3.one;
		Vector3 endScale = Vector3.one * _directScaleMinRatio;
		
		float startAlpha = _endAlpha;
		float endAlpha = 0;
		Debug.Log(">>> PopUpController: Direct_Close 2");
		AnimationCurve curve = CurveHolder._AcceleratedRising;

		float lerpRatio = 0;
		IEnumerator thread = CoroutineUtil.RealTimeThread(duration, (float returnValue)=>{lerpRatio = returnValue;}, null);
		popUp.transform.localScale = startScale;
		popUp.transform.localPosition = startPosition;
		_instance._alphaScreen.alpha = startAlpha;
		while(thread.MoveNext())
		{
			yield return thread;
			popUp.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(lerpRatio));
			popUp.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(lerpRatio));
			_instance._alphaScreen.alpha = Mathf.Lerp(startAlpha, endAlpha, lerpRatio*2f-1f);
		}
		popUp.transform.localScale = endScale;
		popUp.transform.localPosition = endPosition;
		_instance._alphaScreen.alpha = endAlpha;
		Debug.Log(">>> PopUpController: Direct_Close 3");
		popUp.EnableButtons (false);
		popUp.gameObject.SetActive (false);
		
		Reset ();

		if(popUp == _instance._requestRating || popUp == _instance._requestFacebook)
		{
			if(_callback_PromoPopUpClose != null)
				_callback_PromoPopUpClose();
		}
		else
		{
			if(_callback_Close != null)
				_callback_Close();
		}

		_endAlpha = 0.7f;
		Debug.Log(">>> PopUpController: Direct_Close 4");
	}
	
	public static void MoveNext()
	{
		if(_thread_Director != null)
		{
			if(_thread_Director.MoveNext() == false)
				_thread_Director = null;
		}
	}

    public static void Open_IosIDFA()
    {
        if (_instance.IosIDFA)
        {
#if UNITY_ANDROID
            AdController.Hide_BannerView();
#endif
            OpenPopUp(_instance.IosIDFA);
        }
    }

    public static void Open_MoreGames()
    {
        if (_instance.MoreGames)
        {
#if UNITY_ANDROID
            AdController.Hide_BannerView();
#endif
            TouchInputRecognizer.SetPause(true);
            OpenPopUp(_instance.MoreGames, false);
        }
    }

    public static void Open_Revive()
    {
        if (_instance._revive)
            OpenPopUp(_instance._revive, false);
    }

    public static void Open_Login()
    {
        if (_instance._login)
        {
            TouchInputRecognizer.SetPause(true);
            OpenPopUp(_instance._login, false);
        }
    }

    public static void Open_Backup()
    {
        if (_instance.Backup)
        {
            TouchInputRecognizer.SetPause(true);
            OpenPopUp(_instance.Backup, false);
        }
    }

    public static void Open_SmartShopNotice(string title, string comment)
    {
        CloseCurrentPopUp();
        _instance._smartShopNotice.SetString(title, comment);
        OpenAlertPopup(_instance._smartShopNotice);
    }

    public static void Open_Alert(AlertPopUpType type, bool isPause = true)
    {
        CloseCurrentPopUp();
        _instance._alert.SetString(type);
        OpenAlertPopup(_instance._alert, isPause);
    }

    public static void Close_System()
    {
        _instance._alert.CloseSystemPopup();
    }


    public void Open_Pause()
    {
        OpenPopUp(_pause);
    }

    public void Open_Setting()
    {
        OpenPopUp(_setting);
    }

    public void Open_Gamebook()
    {
        //OpenPopUp(_gamebook);
    }

    public void Open_Store()
    {
        _store.SetUI();
        OpenPopUp(_store);
    }

    public void Open_HowToPlay()
    {
#if UNITY_ANDROID
        AdController.Hide_BannerView();
        WebViewController.instance.ClearWebView();
#endif
        _endAlpha = 1;
        OpenPopUp(_howToPlay);
    }

    public static void Open_HowToPlay_Immediately()
    {
#if UNITY_ANDROID
        AdController.Hide_BannerView();
        WebViewController.instance.ClearWebView();
#endif
        _endAlpha = 1;
        OpenPopUp_Immediately(_instance._howToPlay);
    }
}
