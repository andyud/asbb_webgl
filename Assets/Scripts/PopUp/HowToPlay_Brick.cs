using UnityEngine;
using System.Collections;

public class HowToPlay_Brick : MonoBehaviour
{
	public UIBasicSprite _texture_Model;
	public UIBasicSprite _texture_Shadow;
	public UILabel _label_Number;

	public void Initialize(Color color, int number)
	{
		_label_Number.color = Static_ColorConfigs._Color_TextWhite;
		_label_Number.text = number.ToString ();

		_texture_Model.color = color;
		_texture_Shadow.color = Static_ColorConfigs._Color_Brick_Shadow;
	}
}
