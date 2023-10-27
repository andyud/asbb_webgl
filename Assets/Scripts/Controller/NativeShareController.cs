using System.Collections;
using System.IO;
using UnityEngine;

public class NativeShareController : MonoBehaviour
{
	private static NativeShareController instance;
	public static NativeShareController Instance
	{
		get
		{
			if (instance == null)
				instance = new GameObject("NativeShareController").AddComponent<NativeShareController>();

			return instance;
		}
	}
	
	public void ShareImage()
	{
		StopCoroutine(TakeScreenshotAndShare());
		StartCoroutine(TakeScreenshotAndShare());
	}
	
	public void ShareText()
	{
		StopCoroutine(TextShare());
		StartCoroutine(TextShare());
	}

	private IEnumerator TextShare()
	{
		yield return new WaitForEndOfFrame();

		new NativeShare().SetSubject(Static_TextConfigs.Share_Subject).SetText(Static_TextConfigs.Share_Text).SetUrl(Static_APP_Config._Market_URL).
			SetCallback((result, shareTarget) =>
			{
				Debug.Log($"Share result: {result} / shareTarget: {shareTarget}");
			}).Share();
	}

	private IEnumerator TakeScreenshotAndShare()
	{
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		Destroy(ss);

		new NativeShare().AddFile(filePath).//SetSubject(TextConfigs.Share_Subject).SetText(TextConfigs.Share_Text).SetUrl(FN._Market_URL).
			SetCallback((result, shareTarget) =>
			{
				Debug.Log($"Share result: {result} / shareTarget: {shareTarget}");
			}).Share();
	}
}
