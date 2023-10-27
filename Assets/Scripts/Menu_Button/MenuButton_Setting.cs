﻿using UnityEngine;
using System.Collections;

public class MenuButton_Setting : MenuButton
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UIBasicSprite _texture_Icon;

	public UIBasicSprite BG;

	protected override void Initialize_Button ()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;

		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;

		Refresh ();
	}

	public override void Refresh ()
	{
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
	}
}
