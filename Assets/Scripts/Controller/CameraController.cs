using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public Camera _uiCamera;
	public Camera _mainCamera;
	public Camera _rtCamera;
	public Renderer _rtRenderer;

	Vector3 _cameraPosition;
	Vector3 _tiltPosition;

	IEnumerator _thread_Director_GrayScale;
	IEnumerator _thread_Director_Tilt;

	public void Initialize()
	{
		if (Static_Calculator.GetScreenWidthRatio() > 0.75f)
		{
			if(Screen.width < Screen.height)
			{
				float v = ((float)Screen.width / Screen.height) / ((float)9 / 20);
				if (v < 1f)
					_rtCamera.orthographicSize = 1.12f;
				else
					_rtCamera.orthographicSize = ((float)Screen.height / Screen.width) * 0.5f;
			}
			else if(Screen.width == Screen.height)
				_rtCamera.orthographicSize = 0.5f;
			else
			{
				_rtCamera.orthographicSize = ((float)Screen.height / Screen.width) * 0.5f;
			}

			_rtRenderer.transform.localScale = Vector3.right*1f + Vector3.up*((float)Screen.height / (float)Screen.width);
		}
		else
		{
			_rtCamera.orthographicSize = ((float)Screen.height / Screen.width) * 0.5f;
			float width = 1;
			float height = Static_Calculator.GetScreenHeightRatio();
			_rtRenderer.transform.localScale = Vector3.right * width + Vector3.up * height;
		}

		_rtRenderer.material.SetFloat ("_LuminosityRatio", 1);
		_rtRenderer.material.SetFloat ("_LuminosityShift", 0);

		_rtCamera.farClipPlane = 100;
		_rtRenderer.transform.localPosition = Vector3.forward * 10;

		_mainCamera.backgroundColor = Static_ColorConfigs._Color_Background;
		_mainCamera.farClipPlane = 1000;
	}
	
	public void Reset()
	{
		_rtCamera.gameObject.SetActive (false);
		_rtRenderer.material.SetFloat ("_LuminosityAmount", 0);

		_thread_Director_GrayScale = null;
		_thread_Director_Tilt = null;

		_cameraPosition = Vector3.back * 10;
		_mainCamera.targetTexture = null;
	}

	public void SetGrayScale(float startRatio, float endRatio, float duration, System.Action endCallback)
	{
		RenderTexture rt = new RenderTexture (Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		_rtRenderer.material.SetTexture ("_MainTex", rt);
		_rtCamera.gameObject.SetActive (true);

		_mainCamera.targetTexture = rt;

		_thread_Director_GrayScale = Direct_GrayScale (startRatio, endRatio, duration, endCallback);
	}

	IEnumerator Direct_GrayScale(float startRatio, float endRatio, float duration, System.Action endCallback)
	{
		float lerpRatio = 0;
		IEnumerator thread = CoroutineUtil.TimeThread(duration, (float returnValue)=>{lerpRatio = returnValue;}, null);
		_rtRenderer.material.SetFloat ("_LuminosityAmount", startRatio);
		while(thread.MoveNext())
		{
			yield return thread;
			_rtRenderer.material.SetFloat ("_LuminosityAmount", Mathf.Lerp(startRatio, endRatio, lerpRatio));
		}
		_rtRenderer.material.SetFloat ("_LuminosityAmount", endRatio);
		
		if(endCallback != null)
			endCallback();
	}

	public void Tilt(float startAmplitude, float endAmplitude, float duration, System.Action endCallback)
	{
		_thread_Director_Tilt = Director_Tilt (startAmplitude, endAmplitude, duration, endCallback);
	}

	IEnumerator Director_Tilt(float startAmplitude, float endAmplitude, float duration, System.Action endCallback)
	{
		float lerpRatio = 0;
		IEnumerator thread = CoroutineUtil.TimeThread(duration, (float returnValue)=>{lerpRatio = returnValue;}, null);
		Vector3 tiltPosition = Vector3.zero;
		SetTilt (tiltPosition);
		while(thread.MoveNext())
		{
			yield return thread;
			Vector2 tilt = Random.insideUnitCircle * Mathf.Lerp(startAmplitude, endAmplitude, lerpRatio);
			tiltPosition = Static_Calculator.Vector2To3 (tilt);
			SetTilt (tiltPosition);
		}
		tiltPosition = Vector3.zero;
		SetTilt (tiltPosition);
		
		if(endCallback != null)
			endCallback();
	}

	void LateUpdate()
	{
		if(_thread_Director_Tilt != null)
		{
			if(_thread_Director_Tilt.MoveNext() == false)
				_thread_Director_Tilt = null;
		}

		if(_thread_Director_GrayScale != null)
		{
			if(_thread_Director_GrayScale.MoveNext() == false)
				_thread_Director_GrayScale = null;
		}

		_mainCamera.transform.localPosition = _cameraPosition + _tiltPosition;
		_tiltPosition = Vector3.zero;
	}

	public void SetStageWidth(float stageWidth)
	{
		_mainCamera.orthographicSize = 0.5f * stageWidth * Static_Calculator.GetScreenHeightRatio();
		_cameraPosition = Vector3.up * stageWidth * 0.5f + Vector3.back * 10;

		if (Static_Calculator.GetScreenWidthRatio() > 0.75f)
		{
			float origin = 9f / 20f;
			float current = ((float)Screen.width / Screen.height);

			if (current >= 0.5f)
			{
				_uiCamera.orthographicSize = 1.2f;
			}
			else if (current > origin)
			{
				float v = current - origin;
				float vv = 1f + v;
				_uiCamera.orthographicSize = vv;
			}
			else
			{
				_uiCamera.orthographicSize = 1f;
			}
		}
	}

	public void SetDarkMode()
	{
		_mainCamera.backgroundColor = Static_ColorConfigs._Color_Background;
	}

	protected void SetTilt(Vector3 tiltPosition)
	{
		_tiltPosition = tiltPosition;
	}
}
