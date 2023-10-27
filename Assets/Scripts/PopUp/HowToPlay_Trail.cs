using UnityEngine;
using System.Collections;

public class HowToPlay_Trail : MonoBehaviour
{
	public UIBasicSprite _texture_Trail;
	public UIBasicSprite _texture_Shadow;

	public void Initialize()
	{
		_texture_Trail.color = Static_ColorConfigs._Color_Ball;
		_texture_Trail.alpha = 0.5f;

		_texture_Shadow.color = Static_ColorConfigs._Color_Ball_Shadow;
		_texture_Shadow.alpha = 0.5f;
	}
}
