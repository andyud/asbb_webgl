using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp_Revive : PopUp
{
	[SerializeField] private UIButton _adViewBtn;
	[SerializeField] private UIButton _closeBtn;

	[SerializeField] private UILabel _titleLabel;
	[SerializeField] private UILabel _countDownLabel;
	[SerializeField] private UILabel _adViewBtnLabel;

	[SerializeField] private GameObject _adIconObject;
	[SerializeField] private UISprite BG;

	protected override void Initialize_PopUp()
	{
		BG.color = Static_ColorConfigs._Color_PopupBackGround;

		_titleLabel.text = Static_TextConfigs.RevivePopup_Title;

		_adViewBtn.onClick.Clear();
		_adViewBtn.onClick.Add(new EventDelegate(() =>
		{
			StopCoroutine(CountDown());


            if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 1 ||
                PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) == 1)
            {
                ReviveController.Instance.IsActive = true;
            }
            else
            {
				ShowRewardAd();
			}

			Close();
		}));

		_closeBtn.onClick.Clear();
		_closeBtn.onClick.Add(new EventDelegate(() =>
		{
			Debug.Log("PopUP_Revive._closeBtn onClick");
			Close();
		}));
	}

	private void ShowRewardAd()
	{
#if UNITY_ANDROID
		AdController.Show_Reward_Revive();
#endif
		Debug.Log("PopUp_Revive._adViewBtn onClick");
	}

	public override void SetUI() { }

	public override void Refresh()
	{
		BG.color = Static_ColorConfigs._Color_PopupBackGround;

		if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) == 1 ||
			PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) == 1 )
		{
			_adIconObject.SetActive(false);
			_adViewBtnLabel.text = Static_TextConfigs.Btn_Revive;
		    _adViewBtnLabel.transform.localPosition = Vector3.zero;
		}
		else
		{
			_adIconObject.SetActive(true);
			_adViewBtnLabel.text = Static_TextConfigs.Btn_AdView;
		    _adViewBtnLabel.transform.localPosition = new Vector3(58f, 0f, 0f);
		}

		StopCoroutine(CountDown());
		StartCoroutine(CountDown());
	}

	IEnumerator CountDown()
	{
		int limitCount = 10;
		var waitForSeconds = new WaitForSeconds(1.25f);

		while (limitCount >= 0)
		{
			_countDownLabel.text = limitCount.ToString();
			yield return waitForSeconds;
			limitCount--;
		}

		yield return new WaitForEndOfFrame();

		Close();
	}
}
