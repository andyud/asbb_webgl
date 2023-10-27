using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PhasePopUp : PopUp
{
	public UIBasicSprite _texture_Frame;
	public UIBasicSprite _texture_Body;

	public UILabel _label_PhaseNumber;		
	public UIButton _button_ScreenTouch;

	public List<GameObject> _phase_Image_List;
	int _currentPhase = 0;

	System.Text.StringBuilder _str_PhaseNumber;

	UICamera _uiCam;

	protected override void Initialize_PopUp ()
	{
		_uiCam = UICamera.mainCamera.GetComponent<UICamera> ();
		_str_PhaseNumber = new System.Text.StringBuilder (5);

		Initialize_PhasePopUp ();

		_texture_Frame.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Body.color = Static_ColorConfigs._Color_ButtonBase;
		_label_PhaseNumber.color = Static_ColorConfigs._Color_TextGray;

		_button_ScreenTouch.onClick.Clear ();
		_button_ScreenTouch.onClick.Add (new EventDelegate (ButtonRespone_TouchScreen));
	}

	protected abstract void Initialize_PhasePopUp ();

	public void Reset()
	{
		_currentPhase = 0;
		MoveNext ();
	}

	void ButtonRespone_TouchScreen()
	{
		MoveNext ();
	}

	public override void Refresh ()
	{
		_uiCam.mouseClickThreshold = 1920;
		_uiCam.touchClickThreshold = 1920;

		StartPhase ();
	}

	protected abstract void StartPhase();

	void MoveNext()
	{
		if(_currentPhase < _phase_Image_List.Count)
		{
			DisplayCurrentPhase();			
			_currentPhase++;

			_str_PhaseNumber.Remove(0, _str_PhaseNumber.Length);
			_str_PhaseNumber.AppendFormat("{0}{1}{2}", _currentPhase, " / ", _phase_Image_List.Count);
			_label_PhaseNumber.text = _str_PhaseNumber.ToString();
		}
		else
		{
			_uiCam.mouseClickThreshold = 128;
			_uiCam.touchClickThreshold = 128;

			EndPhase();
			Close();
		}
	}

	protected abstract void EndPhase();

	void DisplayCurrentPhase()
	{
		for(int i = 0; i < _phase_Image_List.Count; i++)
			_phase_Image_List[i].gameObject.SetActive(false);

		_phase_Image_List [_currentPhase].SetActive (true);
	}
}