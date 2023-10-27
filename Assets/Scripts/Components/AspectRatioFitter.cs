using UnityEngine;
using System.Collections;

public class AspectRatioFitter : MonoBehaviour
{
	public float _defaultRatio;
	public float _screenHeight;
	public float _screenWidth;
	Transform _transform;

	void Awake()
	{
		float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 16); // (가로 / 세로)
		if(scaleheight < 1f)
		{
			_defaultRatio = 1.5f;
		}
		else
		{
			_defaultRatio = 1.5f / scaleheight;
		}

		//_defaultRatio = 3f / 2f;
		_transform = transform;

		StartCoroutine (Update_Scale ());
	}
	
	IEnumerator Update_Scale()
	{
		while(true)
		{
			if(IsScreenSizeChanged())
				ChangeTransformScale();

			yield return 0;
		}
	}

	bool IsScreenSizeChanged()
	{
		if(_screenHeight != (float)Screen.height || _screenWidth != (float)Screen.width)
			return true;
		else
			return false;
	}

	void ChangeTransformScale()
	{
		_screenHeight = (float)Screen.height;
		_screenWidth = (float)Screen.width;
		float currentRatio = _screenWidth/_screenHeight;
		float modifier = Mathf.Min (currentRatio * _defaultRatio, 1);
		
		_transform.localScale = Vector3.one * modifier;
	}
}