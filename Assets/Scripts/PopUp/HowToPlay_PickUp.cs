using UnityEngine;
using System.Collections;

public class HowToPlay_PickUp : MonoBehaviour
{
	public UIBasicSprite _texture_Model;
	public UIBasicSprite _texture_Outer;
	public UIBasicSprite _texture_Shadow;
	public UIBasicSprite _texture_Shadow_Outer;

	public void Initialize()
	{
		_texture_Model.color = Static_ColorConfigs._Color_PickUp;
		_texture_Outer.color = Static_ColorConfigs._Color_PickUp;
		_texture_Shadow.color = Static_ColorConfigs._Color_PickUp_Shadow;
		_texture_Shadow_Outer.color = Static_ColorConfigs._Color_PickUp_Shadow;
	}
}
