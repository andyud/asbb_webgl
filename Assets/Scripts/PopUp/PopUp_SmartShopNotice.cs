using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp_SmartShopNotice : NoticePopUp
{
	public UIBasicSprite texture_wall;
	public UIBasicSprite texture_floor;
	public UIBasicSprite texture_Check;
	public UIButton btn_Ok;
	public UIButton btn_Skip;
	public UILabel label_BtnOK;
	public UILabel label_Title;
	public UILabel label_Comment;

	public override void SetString(string title, string comment)
	{
		label_Title.text = title;
		label_Comment.text = comment;
		// 체크 이미지의 경우 디폴트가 false;
		texture_Check.gameObject.SetActive(false);
		PlayerPrefs.SetString(PlayerPrefs_Config.ExpiredSmartShopNoticeSkipData, string.Empty);

	}

	protected override void Initialize_PopUp()
	{
		texture_wall.color = Static_ColorConfigs._Color_ButtonFrame;
		texture_floor.color = Static_ColorConfigs._Color_PopupBackGround;

		label_BtnOK.text = Static_TextConfigs.Btn_OK;

		btn_Ok.onClick.Clear();
		btn_Ok.onClick.Add(new EventDelegate(BtnEventOk));

		btn_Skip.onClick.Clear();
		btn_Skip.onClick.Add(new EventDelegate(BtnEventSkip));
	}

	public override void Refresh()
	{
		texture_floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}
	public override void SetUI() { }

	void BtnEventOk()
	{

		if (texture_Check.gameObject.activeSelf)
		{
			PlayerPrefs.SetString(PlayerPrefs_Config.ExpiredSmartShopNoticeSkipData, System.DateTime.Today.AddDays(3).ToString());
#if UNITY_ANDROID
			FirebaseLogController.LogEvent(FirebaseLogType.SkipNotice);
#endif
		}

		Close();
	}

	void BtnEventSkip()
	{
		texture_Check.gameObject.SetActive(!texture_Check.gameObject.activeSelf);
	}
}
