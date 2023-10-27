using UnityEngine;
using System.Collections;

public class MenuButton_Achievement : MenuButton
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UIBasicSprite _texture_Icon;
	
	protected override void Initialize_Button ()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;
		_texture_Icon.color = Static_ColorConfigs._Color_ButtonFrame;
	}
	
	public override void Refresh ()
	{
		
	}
}
