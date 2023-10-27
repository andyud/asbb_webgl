using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton_PowerMode : MenuButton
{
	const int SIZE_BG = 112;

	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UIBasicSprite _texture_Icon;

	public UIBasicSprite Sprite_Active;
	public UIBasicSprite BG;
	public UILabel Label_TurnCount;

	private bool isIconActionHide = false;

	protected override void Initialize_Button()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;

		_texture_Icon.gameObject.SetActive(false);
		Sprite_Active.gameObject.SetActive(false);

		Label_TurnCount.text = string.Empty;

		SetMode(false);
	}

	IEnumerator IconAction()
	{
		Vector3 startScale = Vector3.one * 1.3f;
		Vector3 endScale = Vector3.one;
		float elapsedTime = 0f;
		float durationTime = 1f;
		float ratioLerp = 0f;

		while (isIconActionHide == false)
		{
			if (elapsedTime > durationTime)
			{
				elapsedTime = 0;
				var temp = startScale;
				startScale = endScale;
				endScale = temp;
			}

			ratioLerp = elapsedTime / durationTime;
			_texture_Icon.transform.localScale = Vector3.Lerp(startScale, endScale, ratioLerp);
			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		_texture_Icon.transform.localScale = Vector3.one;
	}

	public override void Refresh()
	{
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
	}

	public void SetMode(bool isActive)
	{
		if (this.gameObject.activeSelf == false)
			return;

		Debug.LogFormat("MenuButton_PowerMode is Active: {0}", isActive);

		if (isActive)
		{
			Label_TurnCount.text = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0).ToString();
			isIconActionHide = true;
		}
		else
		{
			Label_TurnCount.text = string.Empty;
			isIconActionHide = false;
			StartCoroutine(IconAction());
		}

		_texture_Icon.gameObject.SetActive(!isActive);
		Sprite_Active.gameObject.SetActive(isActive);

		if(StaticMethod.IsPurchase_Speed_Total_SmartShopReward() == false)
		{
			StopCoroutine("BG_Action");
			StartCoroutine(BG_Action(isActive));
		}
		else
		{
			BG.width = SIZE_BG;
			Label_TurnCount.gameObject.SetActive(false);
		}
	}

	IEnumerator BG_Action(bool isActive)
	{
		float durationTime = 0.5f;
		float elapsedTime = 0f;
		float lerpRatio = 0f;

		var startSize = BG.width;
		var endSize = isActive ? SIZE_BG + (int)(Label_TurnCount.width + Label_TurnCount.transform.localPosition.x) : SIZE_BG;

		while (elapsedTime <= durationTime)
		{
			lerpRatio = elapsedTime / durationTime;

			BG.width = (int)Mathf.Lerp(startSize, endSize, lerpRatio);

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		BG.width = endSize;
		Label_TurnCount.gameObject.SetActive(isActive);
	}
}
