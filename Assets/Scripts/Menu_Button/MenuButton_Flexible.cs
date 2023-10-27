using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton_Flexible : MenuButton
{
	const float MASK_PANEL_SIZE_X = 180f;
	const float BETWEEN_ICON_SIZE = 160f;

	public UIPanel Panel_Flexible;
	public UIBasicSprite BG;

	private bool isFlexible;

	protected override void Initialize_Button()
	{
		Panel_Flexible.clipOffset = new Vector2(0f, 0f);
		Panel_Flexible.SetRect(0f, 0f, MASK_PANEL_SIZE_X, 0f);
		Panel_Flexible.clipSoftness = new Vector2(0f, 0f);
		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
		//BG.height = 0;

		_button.onClick.Add(new EventDelegate(ClickEvent));
	}

	public override void Refresh()
	{
		if (this.isActiveAndEnabled)
		{
			isFlexible = false;
			StopCoroutine("Action");
			StartCoroutine(Action());
		}

		BG.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Color.white;
	}

	public void ClickEvent()
	{
		Debug.Log("ClickEvent()");

		isFlexible = !isFlexible;

		StopCoroutine("Action");
		StartCoroutine(Action());
	}

	IEnumerator Action()
	{
		float durationTime = 0.5f;
		float elapsedTime =0f;
		float lerpRatio = 0f;
		float size;
		Vector2 softness;
		Vector2 offset;

		float startSize = Panel_Flexible.height;
		float endSize = isFlexible ? Screen.height : 0f;

		Vector2 startSoftness = Panel_Flexible.clipSoftness;
		Vector2 endSoftness = isFlexible ? Vector2.zero : new Vector2(0f, Screen.height);

		Vector2 startoffset = Panel_Flexible.clipOffset;
		Vector2 endoffset = isFlexible ? new Vector2(0f, -(endSize * 0.5f)) : Vector2.zero;

		var children = Panel_Flexible.GetComponentsInChildren<MenuButton>(false);
		float posY =0f;
		for(int i=0; i<children.Length; i++)
		{
			if(i==0)
			{
				posY = -BETWEEN_ICON_SIZE * 0.5f;
			}
			else
			{
				posY = children[0].transform.localPosition.y - (BETWEEN_ICON_SIZE * i);
			}
			
			children[i].transform.localPosition = new Vector3(0f, posY, 1f);
		}

		//BG.height = children.Length * (int)BETWEEN_ICON_SIZE;

		while (elapsedTime <= durationTime)
		{
			lerpRatio = elapsedTime / durationTime;

			size = Mathf.Lerp(startSize, endSize, lerpRatio);
			softness = Vector2.Lerp(startSoftness, endSoftness, lerpRatio);
			offset = Vector2.Lerp(startoffset, endoffset, lerpRatio);

			Panel_Flexible.SetRect(0f, 0f, MASK_PANEL_SIZE_X, size);
			Panel_Flexible.clipSoftness = softness;
			Panel_Flexible.clipOffset = offset;

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		Panel_Flexible.SetRect(0f, 0f, MASK_PANEL_SIZE_X, endSize);
		Panel_Flexible.clipSoftness = endSoftness;
		Panel_Flexible.clipOffset = endoffset;
	}
}
