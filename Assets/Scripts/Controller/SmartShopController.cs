using UnityEngine;
using System;

public class SmartShopController
{
	// smartshop 구매에 따른 보상 아이템 관련
	public static bool reward_Activated = false;

	// smartshop btn 관련
	public static bool isActivated_Btn = false;

	private static AndroidJavaObject activity;

	// 링크를 통한 접근.
	public static void CallbackActivitySmartShop()
	{
		
	}

	public static void CallActivitySmartShop(string productId = "")
	{
	
	}

	public static void CheckRewardItemExpireDateTime()
	{
		Debug.Log("CheckRewardItemExpireDateTime()");

		// 보상 아이템 유효.
		if (reward_Activated)
		{
			Debug.Log("reward_Activated");
			PopUpController.AddAlert(AlertPopUpType.Availability_SmartShopPromotionItem);
#if UNITY_ANDROID
			AdController.Hide_BannerView();
#endif
		}
		else // 보상 아이템 만료.
		{
			Debug.Log("reward_Expired");

			// 만료후 최초 접근시 만료 안내 팝업 띄우도록..
			var smartShopRewardExpiredDate = PlayerPrefs.GetString(PlayerPrefs_Config.SmartShopRewardExpiredDate, string.Empty);
			if(!string.IsNullOrEmpty(smartShopRewardExpiredDate))
			{
				var ExpiredDate = DateTime.Parse(smartShopRewardExpiredDate);
				if (DateTime.Today > ExpiredDate)
				{
					PlayerPrefs.SetString(PlayerPrefs_Config.SmartShopRewardExpiredDate, string.Empty);
					PopUpController.AddAlert(AlertPopUpType.Expiration_SmartShopPromotionItem);
				}
				else
				{
					Debug.LogError("error => CheckRewardItemExpireDateTime()");
				}
			}
		}
	}
}
