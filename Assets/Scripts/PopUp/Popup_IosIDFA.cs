
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif


public class Popup_IosIDFA : PopUp
{
	public UIBasicSprite texture_wall;
	public UIBasicSprite texture_floor;

	public UILabel label_Title;
	public UILabel label_Message;

	public UIButton btn_Next;
	public UILabel label_Next;

	protected override void Initialize_PopUp()
	{
		texture_wall.color = Static_ColorConfigs._Color_ButtonFrame;
		texture_floor.color = Static_ColorConfigs._Color_PopupBackGround;

		label_Title.text = Static_TextConfigs.IosIDFA_Title;
		label_Message.text = Static_TextConfigs.IosIDFA_Message;
		label_Next.text = Static_TextConfigs.IosIDFA_Next;

		btn_Next.onClick.Clear();
		btn_Next.onClick.Add(new EventDelegate(NextBtnClick));
	}

	public override void Refresh()
	{
		texture_floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}

	public override void SetUI(){}

	private void NextBtnClick()
	{
#if UNITY_IOS
		ATTrackingStatusBinding.RequestAuthorizationTracking();
#endif
		Close();
	}
}
