using System;
using UnityEngine;

public class Popup_Alert : AlertPopUp
{
	public UIBasicSprite texture_wall;
	public UIBasicSprite texture_floor;
	public UIButton btn_Retry;
	public UIButton btn_Close;
	public UILabel label_BtnTry;
	public UILabel label_Title;
	public UILabel label_Comment;

	private AlertPopUpType _type;

	public override void SetString(AlertPopUpType type)
	{
		var title = string.Empty;
		var Comment = string.Empty;
		var btnLabel = string.Empty;
		_type = type;

		btn_Close.gameObject.SetActive(true);

		switch (type)
		{
			default:
			case AlertPopUpType.None:
				btnLabel = Comment = title = string.Empty;
				break;
			case AlertPopUpType.Availability_SmartShopPromotionItem:
				title = Static_TextConfigs.Availability_SmartShopPromotionItem_Title;
				Comment = string.Format(Static_TextConfigs.Availability_SmartShopPromotionItem_Comment, DateTime.Parse(PlayerPrefs.GetString(PlayerPrefs_Config.SmartShopRewardExpiredDate, string.Empty)));
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Expiration_SmartShopPromotionItem:
				title = Static_TextConfigs.Expiration_SmartShopPromotionItem_Title;
				Comment = Static_TextConfigs.Expiration_SmartShopPromotionItem_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Network_NotReachable:
				title =	  Static_TextConfigs.Network_NotReachable_Title;
				Comment = Static_TextConfigs.Network_NotReachable_Comment;
				btnLabel = Static_TextConfigs.Btn_Retry;
				break;
			case AlertPopUpType.SmartShop_RequestAdvertisingIdentifierAsync_Error:
				title =   Static_TextConfigs.RequestAdvertisingIdentifierAsync_Error_Title;
				Comment = Static_TextConfigs.RequestAdvertisingIdentifierAsync_Error_Comment;
				btnLabel = Static_TextConfigs.Btn_Retry;
				break;
			case AlertPopUpType.SmartShop_WebRequest_Error:
				title =   Static_TextConfigs.WebRequest_Error_Title;
				Comment = Static_TextConfigs.WebRequest_Error_Comment;
				btnLabel = Static_TextConfigs.Btn_Retry;
				break;
			case AlertPopUpType.PowerMode_State_Moving:
				title = Static_TextConfigs.PowerMode_State_Moving_Title;
				Comment = Static_TextConfigs.PowerMode_State_Moving_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.PowerMode_LimitCount:
				title = Static_TextConfigs.PowerMode_LimitCount_Title;
				Comment = Static_TextConfigs.PowerMode_LimitCount_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.SpeedMode_State_Moving:
				title = Static_TextConfigs.SpeedMode_State_Moving_Title;
				Comment = Static_TextConfigs.SpeedMode_State_Moving_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.RewardAdOpeningAfterForceClose:
				title = Static_TextConfigs.RewardAdOpeningAfterForceClose_Title;
				Comment = Static_TextConfigs.RewardAdOpeningAfterForceClose_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.PowerMode_Guide:
				title = Static_TextConfigs.PowerModeGuide_Title;
				Comment = Static_TextConfigs.PowerModeGuide_Comment;

				if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward())
					btnLabel = Static_TextConfigs.Btn_OK;
				else if (PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, -1) > 0)
					btnLabel = Static_TextConfigs.Btn_OK;
				else
					btnLabel = Static_TextConfigs.Btn_AdView;

				break;
			case AlertPopUpType.SpeedMode_Guide:
				title = Static_TextConfigs.SpeedModeGuide_Title;
				Comment = Static_TextConfigs.SpeedModeGuide_Comment;

				if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward())
					btnLabel = Static_TextConfigs.Btn_OK;
				else if (PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, -1) > 0)
					btnLabel = Static_TextConfigs.Btn_OK;
				else
					btnLabel = Static_TextConfigs.Btn_AdView;

				break;
			case AlertPopUpType.Restore_Success:
				title = Static_TextConfigs.RestoreSuccess_Title;
				Comment = Static_TextConfigs.RestoreSuccess_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Restore_Fail:
				title = Static_TextConfigs.RestoreFail_Title;
				Comment = Static_TextConfigs.RestoreFail_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Not_Connection_Network:
				title = Static_TextConfigs.NotConnectionNetwork_Title;
				Comment = Static_TextConfigs.NotConnectionNetwork_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.FCM_Reward_Item:
				title = Static_TextConfigs.FCM_RewardItem_Title;
				Comment = string.Format(Static_TextConfigs.FCM_RewardItem_Comment, PlayerPrefs.GetInt(PlayerPrefs_Config.FCMRewardCount, 0));
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Update_Reward_Item:
				title = Static_TextConfigs.Update_RewardItem_Title;
				Comment = string.Format(Static_TextConfigs.Update_RewardItem_Comment, SpeedMode_Config.FirstRunFreeRewardCount);
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Duplicate_Receipt:
				title = Static_TextConfigs.Duplicate_Receipt_Title;
				Comment = Static_TextConfigs.Duplicate_Receipt_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.IAP_Loading:
				title = Static_TextConfigs.IAP_Loading_Title;
				Comment = Static_TextConfigs.IAP_Loading_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Failed_RankingScore:
				title = Static_TextConfigs.Failed_RankingScore_Title;
				Comment = Static_TextConfigs.Failed_RankingScore_Commnet;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Expired_Ranking:
				title = Static_TextConfigs.Expired_Ranking_Title;
				Comment = Static_TextConfigs.Expired_Ranking_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Force_Update:
				title = Static_TextConfigs.Force_Update_Title;
				Comment = Static_TextConfigs.Force_Update_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				btn_Close.gameObject.SetActive(false);
				break;
			case AlertPopUpType.Recommended_Update:
				title = Static_TextConfigs.Recommended_Update_Title;
				Comment = Static_TextConfigs.Recommended_Update_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
			case AlertPopUpType.Load_Fail_Ads:
				title = Static_TextConfigs.AdsLoad_Fail_Title;
				Comment = Static_TextConfigs.AdsLoad_Fail_Comment;
				btnLabel = Static_TextConfigs.Btn_OK;
				break;
		}
		
		label_Title.text = title;
		label_Comment.text = Comment;
		label_BtnTry.text = btnLabel;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.AlertPopup, type.ToString());
#endif
	}


	public override void CloseSystemPopup()
	{
		Close();
	}

	protected override void Initialize_PopUp()
	{
		texture_wall.color = Static_ColorConfigs._Color_ButtonFrame;
		texture_floor.color = Static_ColorConfigs._Color_PopupBackGround;

		btn_Retry.onClick.Clear();
		btn_Retry.onClick.Add(new EventDelegate(BtnEvent_Retry));

		btn_Close.onClick.Clear();
		btn_Close.onClick.Add(new EventDelegate(BtnEvent_Close));
	}

	public override void Refresh()
	{
		texture_floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}

	public override void SetUI(){}

	void BtnEvent_Retry()
	{
		if(_type != AlertPopUpType.Force_Update)
			Close();

		switch (_type)
		{
			case AlertPopUpType.Network_NotReachable:
				Main._main.InitNetworkCall();
				break;
			case AlertPopUpType.SmartShop_RequestAdvertisingIdentifierAsync_Error:
				SmartShopWebRequest.RequestAdvertisingIdentifierAsync();
				break;
			case AlertPopUpType.SmartShop_WebRequest_Error:
				SmartShopWebRequest.RetryCoroutineRequest();
				break;
			case AlertPopUpType.PowerMode_Guide:
               Main. _main._gui.CheckPowerMode();
				break;
			case AlertPopUpType.SpeedMode_Guide:
                Main._main._gui.CheckSpeedMode();
				break;
			case AlertPopUpType.Update_Reward_Item:
			case AlertPopUpType.FCM_Reward_Item:
				InGameController._instance._shooter.InitSpeedAndPowerMode();
				break;
			case AlertPopUpType.Force_Update:
			case AlertPopUpType.Recommended_Update:
				Application.OpenURL(Static_APP_Config._Market_URL);
				break;
		}
	}

	void BtnEvent_Close()
	{
		if(_type != AlertPopUpType.Force_Update)
			Close();

		switch (_type)
		{
			case AlertPopUpType.Update_Reward_Item:
			case AlertPopUpType.FCM_Reward_Item:
				InGameController._instance._shooter.InitSpeedAndPowerMode();
				break;
			case AlertPopUpType.Recommended_Update:
				//WebViewController.instance.ShowNoticeWebView();
				break;
		}
	}
}