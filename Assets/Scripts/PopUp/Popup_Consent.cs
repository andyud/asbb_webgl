using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Popup_Consent : PopUp
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;

	public UILabel _label_Title;
	public UILabel _label_Desc;

	public UILabel _label_Yes;
	public UILabel _label_No;
	public UIButton _button_Yes;
	public UIButton _button_No;

	protected override void Initialize_PopUp()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
		_label_Title.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_Desc.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_Yes.color = Static_ColorConfigs._Color_ButtonFrame;
		
		_label_Title.text = Static_TextConfigs._ConsentTitle;
		_label_Desc.text = Static_TextConfigs._ConsentDesc;
		_label_Yes.text = Static_TextConfigs._ConsentYes;
		_label_No.text = Static_TextConfigs._ConsentNo;

		_button_Yes.onClick.Clear();
		_button_Yes.onClick.Add(new EventDelegate(ButtonResponse_Yes));

		_button_No.onClick.Clear();
		_button_No.onClick.Add(new EventDelegate(ButtonResponse_No));
	}
	public override void SetUI()
	{
	}
	void ButtonResponse_Yes()
	{
		SetConsent(true);		
	}

	public void SetConsent(bool b)
	{
	}

	void ButtonResponse_No()
	{
		SetConsent(false);
	}

	public override void Refresh()
	{

	}
}
