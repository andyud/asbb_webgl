using UnityEngine;
using System.Collections;

public class HowToPlay_Ball : MonoBehaviour
{
	public UIBasicSprite _texture_Model;
	public UIBasicSprite _texture_Shadow;

	public void Initialize()
	{
		_texture_Model.color = Static_ColorConfigs._Color_Ball;
		_texture_Shadow.color = Static_ColorConfigs._Color_Ball_Shadow;
	}
}
