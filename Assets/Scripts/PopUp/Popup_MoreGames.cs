using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;

public class Popup_MoreGames : PopUp
{
	private const string MoreGames = "/MoreGames";

	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;

	public Transform ParentMoreGames;
	public GameObject MoreGameItemPrefab;
	public UIScrollView ScrollView;
	public UITable Table;

	[Header("[Label]")]
	public UILabel TitleLabel;
	public UILabel MoreBtnLabel;
	public UILabel LoadFailLabel;

	[Header("[Button]")]
	public UIButton CloseBtn;
	public UIButton CloseBtn_LoadingPanel;
	public UIButton MoreGameBtn;

	[Header("[Loading]")]
	public GameObject LoadingPanel;
	public GameObject LoadingBG;
	public GameObject LoadingObject;

	private List<Popup_MoreGamesItem> _itemList = null;
	private bool _isLoadSuccess;

	public override void SetUI() { }

	protected override void Initialize_PopUp()
	{
		CloseBtn.onClick.Clear();
		CloseBtn.onClick.Add(new EventDelegate(() => Close()));

		CloseBtn_LoadingPanel.onClick.Clear();
		CloseBtn_LoadingPanel.onClick.Add(new EventDelegate(() => Close()));

		MoreGameBtn.onClick.Clear();
		MoreGameBtn.onClick.Add(new EventDelegate(() =>
		{
			if(Application.platform == RuntimePlatform.Android)
				Application.OpenURL("https://play.google.com/store/apps/collection/cluster?clp=igM4ChkKEzc3NTk5OTMxMTE4NDI1ODIyNDIQCBgDEhkKEzc3NTk5OTMxMTE4NDI1ODIyNDIQCBgDGAA%3D:S:ANO1ljJNv9c&gsr=CjuKAzgKGQoTNzc1OTk5MzExMTg0MjU4MjI0MhAIGAMSGQoTNzc1OTk5MzExMTg0MjU4MjI0MhAIGAMYAA%3D%3D:S:ANO1ljJjQeQ");

			if (Application.platform == RuntimePlatform.IPhonePlayer)
				Application.OpenURL("https://apps.apple.com/developer/tdi-co-ltd/id1095214293#see-all/i-phonei-pad-apps");
		}));

		LoadingPanel.SetActive(false);
		LoadingBG.SetActive(false);
		LoadingObject.SetActive(false);
		CloseBtn_LoadingPanel.gameObject.SetActive(false);

		TitleLabel.text = Static_TextConfigs.MoreGamesPopup_Title;
		MoreBtnLabel.text = Static_TextConfigs.MoreGamesPopup_MoreBtnLabel;
		LoadFailLabel.text = Static_TextConfigs.MoreGamesPopup_LoadFailLabel;
	}
	
	public override void Refresh()
	{
#if UNITY_ANDROID
		if (FireBaseController.ConfigData == null || FireBaseController.ConfigData.MoreGames == null)
		{
			LoadFailLabel.gameObject.SetActive(true);
			return;
		}
#endif
		LoadFailLabel.gameObject.SetActive(false);

		if (_itemList != null)
		{
			for (int i = 0; i < _itemList.Count; i++)
				Destroy(_itemList[i].gameObject);
		}

		StopCoroutine("Loading");
		StartCoroutine("Loading");

		Table.Reposition();
		ScrollView.ResetPosition();

		StopCoroutine("Load");
		StartCoroutine("Load");
	}

	IEnumerator Load()
	{
#if UNITY_ANDROID
		var moreGames = FireBaseController.ConfigData.MoreGames;

		_itemList = new List<Popup_MoreGamesItem>();

		string folderPath = string.Empty;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			folderPath = Application.persistentDataPath + MoreGames;
		else
			folderPath = Application.dataPath + MoreGames;

		var language = Main.language == SystemLanguage.Korean ? SystemLanguage.Korean.ToString() : SystemLanguage.English.ToString();
		bool isReset = moreGames.Any(v => File.Exists(string.Format("{0}/{1}", folderPath, string.Format("{0}{1}", v.TextureName, language))) == false);
		if(isReset)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			else
			{
				var filePaths = Directory.GetFiles(folderPath);
				foreach (var path in filePaths)
					File.Delete(path);
			}
		}

		yield return new WaitForEndOfFrame();

		for (int i = moreGames.Length -1; i >= 0; i--)
		{
			if (moreGames[i].IsActive)
			{
				var item = Instantiate(MoreGameItemPrefab).GetComponent<Popup_MoreGamesItem>();
				item.transform.SetParent(ParentMoreGames);
				item.transform.localPosition = Vector3.zero;
				item.transform.localScale = Vector3.one;
				item.gameObject.SetActive(true);

				var clickUrl = Application.platform == RuntimePlatform.IPhonePlayer ? moreGames[i].IosClickUrl : moreGames[i].AosClickUrl;
				var imageUrl = Main.language == SystemLanguage.Korean ? moreGames[i].Ko_ImageUrl : moreGames[i].En_ImageUrl;
				var textureName = string.Format("/{0}{1}", moreGames[i].TextureName, language);
				string savePath = folderPath + textureName;
				bool isNext = false;
				item.SetTexture(imageUrl, textureName, clickUrl, folderPath, savePath, () => isNext = true);
				_itemList.Add(item);

				yield return new WaitUntil(() => isNext);
				Table.Reposition();
			}
		}
#else
		yield break;
#endif
		ScrollView.ResetPosition();
		_isLoadSuccess = true;
	}

	IEnumerator Loading()
	{
		float durationTime = 1f;
		float elapsedTime = 0f;
		float ratioLerp = 0f;
		var start = Vector3.zero;
		var end = new Vector3(0f, 0f, 360f);
		Time.timeScale = 1.25f;

		LoadingPanel.SetActive(true);
		LoadingBG.SetActive(true);
		LoadingObject.SetActive(true);
		CloseBtn_LoadingPanel.gameObject.SetActive(true);

		_isLoadSuccess = false;
		while (_isLoadSuccess == false)
		{
			if (elapsedTime > durationTime)
				elapsedTime = 0;

			ratioLerp = elapsedTime / durationTime;

			LoadingObject.transform.localEulerAngles = Vector3.Lerp(start, end, ratioLerp);

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		LoadingObject.transform.localEulerAngles = start;

		LoadingPanel.SetActive(false);
		LoadingBG.SetActive(false);
		LoadingObject.SetActive(false);
		CloseBtn_LoadingPanel.gameObject.SetActive(false);


	}

	private void ResetPosition()
	{
		ScrollView.ResetPosition();
	}
}
