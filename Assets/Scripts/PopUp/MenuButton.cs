using UnityEngine;
using System.Collections;

public abstract class MenuButton : MonoBehaviour
{
	public UIButton _button;
	System.Action _callback_Touch;

	public void Initialize(System.Action touchCallback)
	{
		_callback_Touch = touchCallback;
		Initialize_Button ();

		if(touchCallback != null)
		{
			_button.onClick.Clear();
			_button.onClick.Add(new EventDelegate(ButtonResponse_Touch));
		}
	}

	protected abstract void Initialize_Button ();

	void ButtonResponse_Touch()
	{
		if(_callback_Touch != null)
		{
			_callback_Touch();
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.IngameSceneMenuBtn, _callback_Touch.Method.Name);
#endif
		}
	}

	public abstract void Refresh();
}
