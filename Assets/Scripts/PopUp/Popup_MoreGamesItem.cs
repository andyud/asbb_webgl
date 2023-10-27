using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Popup_MoreGamesItem : MonoBehaviour
{
	public UITexture Image;
	public BoxCollider Collider;
	public UIButton Btn;

	private Action _callback;

	public void SetTexture(string url, string textureName, string clickUrl, string folderPath, string savePath, Action callback)
	{
		_callback = callback;

		Btn.onClick.Clear();
		Btn.onClick.Add(new EventDelegate(() =>
		{
			Application.OpenURL(clickUrl);
		}));

		if (File.Exists(savePath))
		{
			var bytes = File.ReadAllBytes(savePath);
			var saveTexture = new Texture2D(1, 1);
			saveTexture.LoadImage(bytes);
			Image.mainTexture = saveTexture;
			Image.MakePixelPerfect();
			Collider.size = Image.localSize;

			if (_callback != null)
				_callback.Invoke();
		}
		else
		{
			StartCoroutine(WebLoadTexture(savePath, url, textureName));
		}
	}

	IEnumerator WebLoadTexture(string savePath, string url, string textureName)
	{
		var uwr = UnityWebRequestTexture.GetTexture(url);
		yield return uwr.SendWebRequest();

		if (uwr.isNetworkError || uwr.isHttpError)
		{
			Debug.LogError(uwr.error);
		}
		else
		{
			var loadTexture = DownloadHandlerTexture.GetContent(uwr);
			Image.mainTexture = loadTexture;
			Image.MakePixelPerfect();
			Collider.size = Image.localSize;
			SaveFile(savePath, loadTexture, textureName);

			if (_callback != null)
				_callback.Invoke();
		}
	}

	private void SaveFile(string savePath, Texture2D texture, string name)
	{
		File.WriteAllBytes(savePath, texture.EncodeToPNG());
	}
}
