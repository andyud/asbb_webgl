using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp_MarketUpdate : PopUp
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;

	public UILabel _label_Desc;

	public UILabel _label_Yes;
	public UILabel _label_No;
	public UIButton _button_Yes;
	public UIButton _button_No;

	protected override void Initialize_PopUp()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
		_label_Desc.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_Yes.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_No.color = Static_ColorConfigs._Color_ButtonFrame;

		_label_Desc.text = Static_TextConfigs._MarketUpdateDesc;
		_label_Yes.text = Static_TextConfigs._MarketUpdateYes;
		_label_No.text = Static_TextConfigs._Quit;

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
		Application.OpenURL(Static_APP_Config._Market_URL);
		Application.Quit();
	}

	void ButtonResponse_No()
	{
		Application.Quit();
	}

	public override void Refresh()
	{

	}
}
