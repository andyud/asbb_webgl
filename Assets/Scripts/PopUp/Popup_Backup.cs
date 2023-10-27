using LitJson;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Popup_Backup : PopUp
{
	private enum AlertType
	{
		None = 0,
		Success,
		Fail,
		NotConnectionNetwork,
		StateIsSignOut,
		NotSupported,
	}

	private const string RESPONSE_RESULT = "success";

	[SerializeField] private UIBasicSprite _wallSprite;
	[SerializeField] private UIBasicSprite _floorSprite;

	[SerializeField] private GameObject _powerSpeedDim;
	[SerializeField] private GameObject _continueDim;
	[SerializeField] private GameObject _totalDim;

	[SerializeField] private GameObject _mainPanelObject;
	[SerializeField] private GameObject _iconGroupObject;
	[SerializeField] private GameObject _loadingObject;

	[SerializeField] private UILabel _nonPurchaseTitle;
	[SerializeField] private UILabel _userBestScore;
	[SerializeField] private UILabel _userIAPTitle;
	[SerializeField] private UILabel _userBackUpLastDate;

	[SerializeField] private GameObject _alertObject;
	[SerializeField] private UILabel _alertTitle;
	[SerializeField] private UILabel _alertComment;
	[SerializeField] private UILabel _alertOkBtnLabel;
	[SerializeField] private UIButton _alertOkBtn;
	[SerializeField] private UISprite _alertBG;

	[SerializeField] private UIButton _backupRestoreBtn;
	[SerializeField] private UIButton _closeBtn;

	private NewBackUpData _backupData;
	private bool _isLoading;

	protected override void Initialize_PopUp()
	{
		_floorSprite.color = Static_ColorConfigs._Color_PopupBackGround;

		_userIAPTitle.text = Static_TextConfigs.LoginPopup_IAP_Title;
		_nonPurchaseTitle.text = Static_TextConfigs.LoginPopup_IAP_NonPurchase;
		_alertOkBtnLabel.text = Static_TextConfigs.Btn_OK;

		_backupRestoreBtn.onClick.Clear();
		_backupRestoreBtn.onClick.Add(new EventDelegate(() =>
		{
#if UNITY_ANDROID
			SetBackupDataAndRequest(FireBaseController.GetUserId());
#endif
		}));

		_alertOkBtn.onClick.Clear();
		_alertOkBtn.onClick.Add(new EventDelegate(() =>
		{
			StopCoroutine("Open");
			StartCoroutine(Open(_mainPanelObject));
			StopCoroutine("Close");
			StartCoroutine(Close(_alertObject));
		}));

		_closeBtn.onClick.Clear();
		_closeBtn.onClick.Add(new EventDelegate(() =>
		{
			Close();
		}));

		_alertObject.SetActive(false);
	}

	public override void Refresh()
	{
		_isLoading = false;
		RefreshUserData();
	}
	public override void SetUI() { }

	private void RefreshUserData()
	{
		StopCoroutine(Loading());
		StartCoroutine(Loading());
		AppServerController.Instance.GetUserBackUpData(vv =>
		{
			_backupData = null;
			
			if (vv.result)
				_backupData = vv.data;
			else
				OpenAlert(AlertType.Fail);

			SetState();
		});
	}

	// appservice 서버로 post 하기 전 데이타 셋팅
	private void SetBackupDataAndRequest(string userId)
	{
		StopCoroutine(Loading());
		StartCoroutine(Loading());

		if (_backupData == null)
			_backupData = new NewBackUpData();

		if (UserData._Score_Best <= _backupData.BestScore)
			UserData._Score_Best = _backupData.BestScore;
		else
			_backupData.BestScore = UserData._Score_Best;

		_backupData.LastBackupDate = DateTime.Now;

		AppServerController.Instance.ValidationUserReceipt(v =>
		{

			if (v.result && v.data != null)
			{
				AppServerController.Instance.SetIAPItem(v.data);
				AppServerController.Instance.SetUserBackUpData(_backupData, vv =>
				{
					SetState();
					OpenAlert(vv.result ? AlertType.Success : AlertType.Fail);
				});
			}
			else
			{
				AppServerController.Instance.SetUserBackUpData(_backupData, vv =>
				{
					if (vv.result)
					{
						AppServerController.Instance.GetIAP(isDuplicate =>
						{
							if (isDuplicate)
							{
#if UNITY_ANDROID
								FireBaseController.AuthSignOut();
#endif
								PopUpController.CloseCurrentPopUp();
								PopUpController.AddAlert(AlertPopUpType.Duplicate_Receipt);
							}
							else
							{
								SetState();
								OpenAlert(AlertType.Success);
							}
						},
						errorCallback: () =>
						{
#if UNITY_ANDROID
							FireBaseController.AuthSignOut();
#endif
							PopUpController.CloseCurrentPopUp();
							PopUpController.AddAlert(AlertPopUpType.Restore_Fail);
						});
					}
					else
					{
						SetState();
						OpenAlert(AlertType.Fail);
					}
				});
			}
		});
	}

	private void SetState()
	{
		StopCoroutine(Loading());

		_loadingObject.SetActive(false);
		_powerSpeedDim.SetActive(false);
		_continueDim.SetActive(false);
		_totalDim.SetActive(false);

		var purchaseCount = 0;
		if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Speed, 0) > 0 || PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Speed, 0) > 0)
		{
			_powerSpeedDim.SetActive(true);
			purchaseCount++;
		}

		if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Continue, 0) > 0 || PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Continue, 0) > 0)
		{
			_continueDim.SetActive(true);
			purchaseCount++;
		}

		if (PlayerPrefs.GetInt(PlayerPrefs_Config.Purchase_Total, 0) > 0 || PlayerPrefs.GetInt(PlayerPrefs_Config.BeforeApp_Purchase_Total, 0) > 0)
		{
			_totalDim.SetActive(true);
			purchaseCount++;
		}

		if (purchaseCount <= 0)
		{
			_iconGroupObject.SetActive(false);
			_nonPurchaseTitle.gameObject.SetActive(true);
		}
		else
		{
			_iconGroupObject.SetActive(true);
			_nonPurchaseTitle.gameObject.SetActive(false);
		}

		_userBestScore.text = string.Format(Static_TextConfigs.LoginPopup_BestScore_Title, _backupData.BestScore);
		_userBackUpLastDate.text = string.Format(Static_TextConfigs.LoginPopup_BackUpDate_Title, _backupData.LastBackupDate);
	}

	private void OpenAlert(AlertType type)
	{
		_isLoading = false;
		_alertBG.color = Static_ColorConfigs._Color_Background;

		var title = string.Empty;
		var comment = string.Empty;

		//StopCoroutine(Loading());
		//_loadingObject.SetActive(false);

		StopCoroutine("Open");
		StartCoroutine(Open(_alertObject));
		StopCoroutine("Close");
		StartCoroutine(Close(_mainPanelObject));

		switch (type)
		{
			case AlertType.NotConnectionNetwork:
				title = Static_TextConfigs.NotConnectionNetwork_Title;
				comment = Static_TextConfigs.NotConnectionNetwork_Comment;
				break;
			case AlertType.Fail:
				title = Static_TextConfigs.BackupFail_Title;
				comment = Static_TextConfigs.BackupFail_Comment;
				break;
			case AlertType.Success:
				title = Static_TextConfigs.BackupSuccess_Title;
				comment = Static_TextConfigs.BackupSuccess_Comment;
				PlayerPrefs.SetString(PlayerPrefs_Config.BackupUpdateDateTime, _backupData.LastBackupDate.ToString());
				break;
			case AlertType.StateIsSignOut:
				title = Static_TextConfigs.StateIsSignOut_Title;
				comment = Static_TextConfigs.StateIsSignOut_Comment;
				break;
			case AlertType.NotSupported:
				title = Static_TextConfigs.NotSupported_Title;
				comment = Static_TextConfigs.NotSupported_Comment;
				break;
		}

		_alertTitle.text = title;
		_alertComment.text = comment;
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.BackUpLoginAlert, type.ToString());
#endif
	}

	IEnumerator Loading()
	{
		float durationTime = 1f;
		float elapsedTime = 0f;
		float ratioLerp = 0f;
		var start = Vector3.zero;
		var end = new Vector3(0f, 0f, 360f);
		Time.timeScale = 1.25f;

		_loadingObject.SetActive(true);

		_isLoading = true;
		while (_isLoading)
		{
			if (elapsedTime > durationTime)
				elapsedTime = 0;

			ratioLerp = elapsedTime / durationTime;

			_loadingObject.transform.localEulerAngles = Vector3.Lerp(start, end, ratioLerp);

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		_loadingObject.transform.localEulerAngles = start;
		_loadingObject.SetActive(false);

	}

	IEnumerator Open(GameObject obj)
	{
		obj.SetActive(true);

		obj.transform.position = _pivot_CloseEndTransform.position;
		Vector3 startPosition = obj.transform.localPosition;
		obj.transform.position = _pivot_OpenEndTransform.position;
		Vector3 endPosition = obj.transform.localPosition;

		Vector3 startScale = Vector3.one * 1f / 8f;
		Vector3 endScale = Vector3.one;

		AnimationCurve curve = CurveHolder._AcceleratedRising;

		float lerpRatio = 0;
		float durationTime = 0.3f;
		float elapsedTime = 0f;

		obj.transform.localScale = startScale;
		obj.transform.localPosition = startPosition;

		while (elapsedTime < durationTime)
		{
			lerpRatio = elapsedTime / durationTime;
			obj.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(lerpRatio));
			obj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(lerpRatio));
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		obj.transform.localScale = endScale;
		obj.transform.localPosition = endPosition;
	}

	IEnumerator Close(GameObject obj)
	{
		obj.transform.position = _pivot_OpenEndTransform.position;
		Vector3 startPosition = obj.transform.localPosition;
		obj.transform.position = _pivot_CloseEndTransform.position;
		Vector3 endPosition = obj.transform.localPosition;

		Vector3 startScale = Vector3.one;
		Vector3 endScale = Vector3.one * 1f / 8f;

		AnimationCurve curve = CurveHolder._AcceleratedRising;

		float lerpRatio = 0;
		float durationTime = 0.3f;
		float elapsedTime = 0f;

		obj.transform.localScale = startScale;
		obj.transform.localPosition = startPosition;

		while (elapsedTime < durationTime)
		{
			lerpRatio = elapsedTime / durationTime;
			obj.transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(lerpRatio));
			obj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(lerpRatio));
			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		obj.transform.localScale = endScale;
		obj.transform.localPosition = endPosition;

		obj.SetActive(false);
	}
}