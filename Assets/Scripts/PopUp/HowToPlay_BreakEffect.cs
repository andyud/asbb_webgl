using UnityEngine;
using System.Collections;

public class HowToPlay_BreakEffect : MonoBehaviour
{
	public UIBasicSprite _texture_Model;
	public UIBasicSprite _texture_Shadow;

	public void Initialize(Color color)
	{
		_texture_Model.color = color;
		_texture_Shadow.color = Static_ColorConfigs._Color_Brick_Shadow;
	}
}
