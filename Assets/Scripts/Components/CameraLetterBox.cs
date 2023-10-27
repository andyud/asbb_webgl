using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLetterBox : MonoBehaviour
{
	void Start()
	{
		if (Static_Calculator.GetScreenWidthRatio() > 0.75f)
		{
			Camera camera = GetComponent<Camera>();
			Rect rect = camera.rect;
			float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 20); // (가로 / 세로)
			float scalewidth = 1f / scaleheight;
			if (scaleheight < 1f)
			{
				rect.height = scaleheight;
				rect.y = (1f - scaleheight) / 2f;
			}
			else
			{
				rect.width = scalewidth;
				rect.x = (1f - scalewidth) / 2f;
			}
			camera.rect = rect;
		}
	}

	void OnPreCull() => GL.Clear(true, true, Color.black);
}
