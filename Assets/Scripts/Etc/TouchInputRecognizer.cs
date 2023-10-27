using UnityEngine;
using System.Collections;

public class Swipe
{
	public enum State
	{
		START = 0,
		STAY,
		END,
	}

	public State _State;
	public Vector3 _StartPosition;
	public Vector3 _CurrentPosition;
	public Vector3 _EndPosition;

	public Swipe()
	{
		_State = State.START;
	}
}

public static class TouchInputRecognizer
{
	static Swipe _currentSwipe;

	public delegate void Announcer(Swipe currentSwipe);
	public static Announcer _AnnounceTouch_Start;
	public static Announcer _AnnounceTouch_Stay;
	public static Announcer _AnnounceTouch_End;
	public static Announcer _AnnounceTouch_Cancel;

	static bool _pause = false;
	public static bool _IsPaused { get { return _pause; } }
	static int _touchIndex = -1;

	public static void SetPause(bool b)
	{
		_pause = b;
		_currentSwipe = null;
	}

	public static void Recognize()
	{
		if(!_pause)
		{
			if(Input.touchCount > 0)
				RecognizeTouchInput();
			else
			{
				RecognizeMouseInput();
			}
/*			else if(Input.touchCount > 1 && _currentSwipe != null)
			{
				_currentSwipe = null;
				if(_AnnounceTouch_Cancel != null)
					_AnnounceTouch_Cancel(null);
			}
*/			
			//#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_WIN
			//RecognizeMouseInput();
			//#endif
		}
	}

	static void RecognizeTouchInput()
	{
		Touch touch = Input.touches[0];
		if (_touchIndex == -1)
			_touchIndex = touch.fingerId;
		else if(Input.touchCount > 1)
		{
			for(int i = 0; i < Input.touchCount; i++)
			{
				if(Input.touches [i].fingerId == _touchIndex)
					touch = Input.touches[i];
			}
		}

        if (touch.phase == TouchPhase.Began)
        {
            SetInputStart(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary )
        {
            if (_currentSwipe != null)
                SetInputStay(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            if (_currentSwipe != null)
                SetInputEnd(touch.position);
        }
	}
	
	static void RecognizeMouseInput()
	{
		if(Input.GetKeyDown(KeyCode.Mouse0))
			SetInputStart(Input.mousePosition);
		else if(Input.GetKey(KeyCode.Mouse0))
		{
			if(_currentSwipe != null)
				SetInputStay(Input.mousePosition);
		}
		else if(Input.GetKeyUp(KeyCode.Mouse0))
		{
			if(_currentSwipe != null)
				SetInputEnd(Input.mousePosition);
		}
	}
	
	static void SetInputStart(Vector3 position)
	{
		_currentSwipe = new Swipe ();
		_currentSwipe._StartPosition = position;

		if(_AnnounceTouch_Start != null)
			_AnnounceTouch_Start(_currentSwipe);
	}

	static void SetInputStay(Vector3 position)
	{
		if(_currentSwipe != null)
		{
			_currentSwipe._State = Swipe.State.STAY;
			_currentSwipe._CurrentPosition = position;

			if(_AnnounceTouch_Stay != null)
				_AnnounceTouch_Stay(_currentSwipe);
		}
	}
	
	static void SetInputEnd(Vector3 position)
	{
		if(_currentSwipe != null)
		{
			_currentSwipe._State = Swipe.State.END;
			_currentSwipe._EndPosition = position;
		
			if(_AnnounceTouch_End != null)
				_AnnounceTouch_End(_currentSwipe);

			_currentSwipe = null;
			_touchIndex = -1;
		}
	}
}
