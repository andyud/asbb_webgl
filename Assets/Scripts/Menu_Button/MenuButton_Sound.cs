using UnityEngine;
using System.Collections;

public class MenuButton_Sound : MenuButton
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UIBasicSprite _texture_Icon;
	public UIBasicSprite _texture_Off;

	protected override void Initialize_Button ()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;
		_texture_Icon.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Off.color = Static_ColorConfigs._Color_WarningRed;

		Refresh ();
	}

	
	public override void Refresh()
	{
		bool b = UserData._OnSound;
		_texture_Off.enabled = !b;
		
		if(b)
			_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		else
			_texture_Wall.color = Static_ColorConfigs._Color_WarningRed;
	}
}
