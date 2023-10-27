using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
	public UIBasicSprite _blackScreen;
	public float _directDuration = 0.3f;

	System.Action _callback_FadeOutEnd;
	System.Action _callback_FadeInEnd;
	IEnumerator _thread;

	public bool _OnChanging { get { return _thread != null ? true : false; } }

	public void Initialize()
	{
		_blackScreen.color = Color.black;
		_callback_FadeOutEnd = null;
		_callback_FadeInEnd = null;
		_thread = null;
	}

	public void FadeOut(System.Action endCallback)
	{
		_callback_FadeOutEnd = endCallback;
		_thread = Thread_Fade (0, 1, _directDuration, Callback_FadeOut);
	}

	public void FadeIn(System.Action endCallback)
	{
		_callback_FadeInEnd = endCallback;
		_thread = Thread_Fade (1, 0, _directDuration, Callback_FadeIn);
	}

	void Callback_FadeOut()
	{
		if(_callback_FadeOutEnd != null)
		{
			_callback_FadeOutEnd();
			_callback_FadeOutEnd = null;
		}
	}
	
	void Callback_FadeIn()
	{
		_blackScreen.gameObject.SetActive (false);
		
		if(_callback_FadeInEnd != null)
		{
			_callback_FadeInEnd();
			_callback_FadeInEnd = null;
		}
	}

	public void FadeOutAndIn(System.Action outEndCallback, System.Action inEndCallback)
	{
		_callback_FadeOutEnd = outEndCallback;
		_callback_FadeInEnd = inEndCallback;
		_thread = Thread_Fade (0, 1, _directDuration, Callback_FadeOutAndIn);
	}

	void Callback_FadeOutAndIn()
	{
		if(_callback_FadeOutEnd != null)
			_callback_FadeOutEnd();

		_thread = Thread_Fade (1, 0, _directDuration, Callback_FadeIn);
	}

	IEnumerator Thread_Fade(float startAlpha, float endAlpha, float duration, System.Action endCallback)
	{
		float lerpRatio = 0;
		IEnumerator thread = CoroutineUtil.RealTimeThread(duration, (float returnValue)=>{lerpRatio = returnValue;}, null);
		_blackScreen.gameObject.SetActive (true);
		_blackScreen.alpha = startAlpha;
		while(thread.MoveNext())
		{
			yield return thread;
			_blackScreen.alpha = Mathf.Lerp(startAlpha, endAlpha, lerpRatio);
		}
		_blackScreen.alpha = endAlpha;
		
		_thread = null;
		
		if(endCallback != null)
			endCallback();
	}

	public void MoveNext()
	{
		if(_thread != null)
			_thread.MoveNext();
	}
}
