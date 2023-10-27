
public enum AlertPopUpType
{
	None = 0,
	SmartShop_RequestAdvertisingIdentifierAsync_Error,
	Network_NotReachable,
	SmartShop_WebRequest_Error,
	Availability_SmartShopPromotionItem,
	Expiration_SmartShopPromotionItem,

	PowerMode_LimitCount,
	PowerMode_State_Moving,

	SpeedMode_State_Moving,

	RewardAdOpeningAfterForceClose,

	PowerMode_Guide,
	SpeedMode_Guide,

	Restore_Success,
	Restore_Fail,

	Not_Connection_Network,

	FCM_Reward_Item,

	Update_Reward_Item,

	Duplicate_Receipt,

	IAP_Loading,

	Failed_RankingScore,

	Expired_Ranking,

	Force_Update,
	Recommended_Update,

	Load_Fail_Ads,
}

public enum ReasonType
{
	none = 0,
	success = 1,
	failed = 2,
	expired = 3,
	update = 4,
	popup = 5
}
