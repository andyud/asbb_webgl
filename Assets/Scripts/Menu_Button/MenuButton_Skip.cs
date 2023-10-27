using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton_Skip : MenuButton
{
    public UIBasicSprite _texture_Icon_Triangle;
    public UIBasicSprite _texture_Icon_Quad;

    protected override void Initialize_Button()
    {
        _texture_Icon_Triangle.color = Static_ColorConfigs._Color_ButtonFrame;
        _texture_Icon_Quad.color = Static_ColorConfigs._Color_ButtonFrame;

        Refresh();
    }

    public override void Refresh()
    {

    }
}
