using UnityEngine;

public class MenuButton_Login : MenuButton
{
	public UIBasicSprite Sprite_Icon;
	public UIBasicSprite BG;

	protected override void Initialize_Button()
	{
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
		Sprite_Icon.color = BG.color;
	}

	public override void Refresh()
	{
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
		Sprite_Icon.color = BG.color;
	}
}
