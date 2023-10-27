﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton_SmartShop : MenuButton
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UIBasicSprite _texture_Icon;

	public UIBasicSprite BG;

	protected override void Initialize_Button()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;
		_texture_Icon.color = BG.color;

		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
	}

	public override void Refresh()
	{
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
		_texture_Icon.color = BG.color;
	}
}
