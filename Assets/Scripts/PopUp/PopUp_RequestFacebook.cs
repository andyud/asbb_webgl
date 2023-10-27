using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopUp_RequestFacebook : PopUp
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UIBasicSprite _texture_Button_Yes;
	public UIBasicSprite _texture_Button_Later;
	public UIBasicSprite _texture_Button_No;
	
	public UILabel _label_Title;
	public UILabel _label_Comment;
	public UILabel _label_ButtonName_Yes;
	public UILabel _label_ButtonName_Later;
	public UILabel _label_ButtonName_No;
	
	public UIButton _button_Yes;
	public UIButton _button_Later;
	public UIButton _button_No;
	
	protected override void Initialize_PopUp ()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;

		_texture_Button_Yes.color = Static_ColorConfigs._Color_TextOrange;
		_texture_Button_Later.color = Static_ColorConfigs._Color_TextOrange;
		_texture_Button_No.color = Static_ColorConfigs._Color_TextOrange;
		
		_label_Title.color = Static_ColorConfigs._Color_FacebookBlue;
		_label_Comment.color = Static_ColorConfigs._Color_TextGray;
		_label_ButtonName_Yes.color = Static_ColorConfigs._Color_TextWhite;
		_label_ButtonName_Later.color = Static_ColorConfigs._Color_TextWhite;
		_label_ButtonName_No.color = Static_ColorConfigs._Color_TextWhite;
		
		_label_Title.text = Static_TextConfigs._RequestFacebook_Title;
		_label_Comment.text = Static_TextConfigs._RequestFacebook_Comment;
		_label_ButtonName_Yes.text = Static_TextConfigs._RequestFacebook_Yes;
		_label_ButtonName_Later.text = Static_TextConfigs._RequestFacebook_Later;
		_label_ButtonName_No.text = Static_TextConfigs._RequestFacebook_No;
		
		_button_Yes.onClick.Clear ();
		_button_Yes.onClick.Add (new EventDelegate (ButtonResponse_Yes));

		_button_Later.onClick.Clear ();
		_button_Later.onClick.Add (new EventDelegate (ButtonResponse_Later));

		_button_No.onClick.Clear ();
		_button_No.onClick.Add (new EventDelegate (ButtonResponse_No));
	}
	
	void ButtonResponse_Yes()
	{
		//Application.OpenURL(FN._FacebookPage_URL);
		UserData._IsFacebookVisited = true;
		Close ();
	}
	
	void ButtonResponse_Later()
	{
		UserData._ExpiredTime_RequestFacebookVisit = System.DateTime.Now.AddDays (1);
		Close ();
	}
	
	void ButtonResponse_No()
	{
		UserData._IsFacebookVisited = true;
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
