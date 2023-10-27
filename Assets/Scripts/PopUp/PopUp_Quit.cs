using UnityEngine;
using System.Collections;

public class PopUp_Quit : PopUp
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UILabel _label_Title;
	public UILabel _label_Yes;
	public UILabel _label_No;
	public UIButton _button_Yes;
	public UIButton _button_No;

	protected override void Initialize_PopUp()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
		_label_Title.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_Yes.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_No.color = Static_ColorConfigs._Color_ButtonFrame;
		
		_label_Title.text = Static_TextConfigs._Quit;
		_label_Yes.text = Static_TextConfigs._Yes;
		_label_No.text = Static_TextConfigs._No;
		
		_button_Yes.onClick.Clear ();
		_button_Yes.onClick.Add (new EventDelegate (ButtonResponse_Yes));
		
		_button_No.onClick.Clear ();
		_button_No.onClick.Add (new EventDelegate (ButtonResponse_No));
	}
	
	void ButtonResponse_Yes()
	{
#if UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.lancekun.quit_helper.AN_QuitHelper"))
		{
			AndroidJavaObject UnityInstance = ajc.CallStatic<AndroidJavaObject>("Instance");
			UnityInstance.Call("AN_Exit");
		}
#else
		Application.Quit();
#endif
		
	}
	
	void ButtonResponse_No()
	{
		Close ();
	}

	public override void Refresh ()
	{
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}

    public override void SetUI()
    {

    }
}
