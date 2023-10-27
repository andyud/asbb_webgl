using UnityEngine;
using System.Collections;

public class ControlGuideLine : MonoBehaviour
{
	public Transform _transform_TouchLine;
	public MeshRenderer _renderer_TouchLine;

	public Transform _transform_ShootLine;
	public MeshRenderer _renderer_ShootLine;

	public Transform _transform_Ball;
	public Renderer _renderer_Ball;

	public Transform _transform_ShootArrow;
	public MeshRenderer _renderer_ShootArrow;
	public MeshRenderer _renderer_ShootArrowLine;

	Vector2 _tiling_1 = Vector2.one;
	Vector2 _textureOffset_1 = Vector2.zero;

	Vector2 _tiling_2 = Vector2.one;
	Vector2 _textureOffset_2 = Vector2.zero;

	public void Initialize()
	{
		_tiling_1 = Vector2.one;
		_textureOffset_1 = Vector2.zero;
		_tiling_2 = Vector2.one;
		_textureOffset_2 = Vector2.zero;

		_renderer_TouchLine.material = MaterialHolder._Material_TouchGuideLine;
		_renderer_ShootLine.material = MaterialHolder._Material_ShootDirectionLine;
		_renderer_Ball.material = MaterialHolder._Material_CollisionGuideBall;
		_renderer_ShootArrow.material = MaterialHolder._Material_Arrow;
		_renderer_ShootArrowLine.material = MaterialHolder._Material_ArrowLine;

		_renderer_TouchLine.transform.localPosition = Vector3.right * 0.5f + Vector3.back * 0.55f;
		_renderer_ShootLine.transform.localPosition = Vector3.right * 0.5f + Vector3.back * 0.55f;
		_renderer_Ball.transform.localScale = Vector3.one * InGameController._BallRadius * 2;
		_renderer_Ball.transform.localPosition = Vector3.back * 0.7f;
		_renderer_ShootArrowLine.transform.localScale = (Vector3.right * InGameController._CurrentStageWidth * 0.25f) + (Vector3.up * InGameController._BallRadius);
//		_renderer_ShootArrowLine.transform.localScale = (Vector3.right * InGameController._CurrentStageWidth * 0.15f) + (Vector3.up * InGameController._BallRadius);
		_renderer_ShootArrowLine.transform.localPosition = Vector3.right * _renderer_ShootArrowLine.transform.localScale.x * 0.5f + Vector3.back * 0.6f;
		_renderer_ShootArrow.transform.localScale = Vector3.one * InGameController._BallRadius * 2;
		_renderer_ShootArrow.transform.localPosition = Vector3.right * (_renderer_ShootArrowLine.transform.localScale.x + _renderer_ShootArrow.transform.localScale.y * 0.5f) + Vector3.back * 0.6f;
		_renderer_ShootArrow.transform.localEulerAngles = Vector3.back * 90;

		_transform_TouchLine.gameObject.SetActive (false);
		_transform_ShootLine.gameObject.SetActive (false);
		_transform_Ball.gameObject.SetActive (false);
		_transform_ShootArrow.gameObject.SetActive (false);
	}

	public void SetShootInfo(Vector3 touchStartPosition, Vector3 touchCurrentPosition, Vector3 ballPosition, bool isSnaped)
	{
		if (isSnaped)
		{
			_transform_ShootLine.gameObject.SetActive (true);
			_transform_TouchLine.gameObject.SetActive (false);
		}
		else
		{
			if(UserData._ShowContactPoint)
				_transform_ShootLine.gameObject.SetActive (true);
			else
				_transform_ShootLine.gameObject.SetActive (false);

			_transform_TouchLine.gameObject.SetActive (true);
		}

		_transform_ShootArrow.gameObject.SetActive (true);
		
		Vector3 dir = touchCurrentPosition - touchStartPosition;
		float distance = dir.magnitude;
		float touchAngle = (Static_Calculator.XYMeter2Angle (dir) + 360f) % 360f;
		float shootAngle = touchAngle;

		if (!(0 <= shootAngle && shootAngle <= 180)) {
			if (shootAngle <= 270)
				shootAngle = 180;
			else
				shootAngle = 0;
		}
		shootAngle = Mathf.Clamp (shootAngle, 10f, 170f);

		float lineLength = distance;

		if(UserData._ShowContactPoint)
		{
			int mask = (1 << LayerMask.NameToLayer ("Brick")) | (1 << LayerMask.NameToLayer ("Stage"));
			Vector2 shootDir = Static_Calculator.Vector2XYForce (shootAngle, 1);
			RaycastHit2D hitInfo = Physics2D.CircleCast (Static_Calculator.Vector3To2 (ballPosition) + shootDir * InGameController._BallRadius, InGameController._BallRadius, shootDir, InGameController._CurrentStageWidth * 3, mask);
			if (hitInfo.collider != null)
			{
				_transform_Ball.localPosition = ballPosition + Static_Calculator.Vector2To3 (shootDir) * (hitInfo.distance + InGameController._BallRadius);
				_transform_Ball.gameObject.SetActive (true);
			}
			else
			{
				Debug.LogWarning ("Control RaycastInfo is NULL");
				_transform_Ball.gameObject.SetActive (false);
			}

			mask = (1 << LayerMask.NameToLayer ("Stage"));
			RaycastHit2D stageHitInfo = Physics2D.CircleCast (Static_Calculator.Vector3To2 (ballPosition) + shootDir * InGameController._BallRadius, InGameController._BallRadius, shootDir, InGameController._CurrentStageWidth * 3, mask);
			if (stageHitInfo.collider != null)
			{
				lineLength = stageHitInfo.distance;
			}
			else
			{
				Debug.LogWarning ("Control Stage RaycastInfo is NULL");
				lineLength = InGameController._CurrentStageWidth * 3f;
			}

/*			if(isSnaped)
				lineLength = Vector3.Distance(ballPosition, touchCurrentPosition);
			else
				lineLength = Vector3.Distance(touchStartPosition, touchCurrentPosition);

			lineLength = Mathf.Max(lineLength, hitInfo.distance);
*/
//			lineLength = InGameController._CurrentStageWidth * 3;
		}
		else
			_transform_Ball.gameObject.SetActive (false);

		_transform_ShootLine.localPosition = ballPosition;
		_transform_ShootLine.localEulerAngles = Vector3.forward * shootAngle;
		_transform_ShootLine.localScale = Vector3.up + Vector3.forward + Vector3.right * lineLength;

		_transform_TouchLine.localPosition = touchStartPosition;
		_transform_TouchLine.localEulerAngles = Vector3.forward * touchAngle;
		_transform_TouchLine.localScale = Vector3.up + Vector3.forward + Vector3.right * distance;

		_transform_ShootArrow.localPosition = ballPosition;
		_transform_ShootArrow.localEulerAngles = Vector3.forward * shootAngle;
		_transform_ShootArrow.localScale = Vector3.one;

		bool isPowermode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1;
		_renderer_ShootLine.material.color = isPowermode ? Static_ColorConfigs._Color_PowerBallGuideLine : Static_ColorConfigs._Color_BallGuideLine;
		_renderer_Ball.material.color = isPowermode ? Static_ColorConfigs._Color_PowerBallGuideLine : Static_ColorConfigs._Color_BallGuideLine;
		_renderer_ShootArrow.material.color = isPowermode ? Static_ColorConfigs._Color_PowerBallGuideLine : Static_ColorConfigs._Color_BallGuideLine;
		_renderer_ShootArrowLine.material.color = isPowermode ? Static_ColorConfigs._Color_PowerBallGuideLine : Static_ColorConfigs._Color_BallGuideLine;
	}

	public void SetOff()
	{
		_transform_TouchLine.gameObject.SetActive (false);
		_transform_ShootLine.gameObject.SetActive (false);
		_transform_Ball.gameObject.SetActive (false);
		_transform_ShootArrow.gameObject.SetActive (false);
	}

	void Update()
	{
		_tiling_1 = Vector2.right * 2.5f * _transform_TouchLine.localScale.x;
		_textureOffset_1 -= Vector2.right * 2.5f * Time.deltaTime;
		_textureOffset_1.x = _textureOffset_1.x % 1f;

		_renderer_TouchLine.material.SetTextureScale("_MainTex", _tiling_1);
		_renderer_TouchLine.material.SetTextureOffset("_MainTex", _textureOffset_1);

		_tiling_2 = Vector2.right * 2.5f * _transform_ShootLine.localScale.x;
		_textureOffset_2 -= Vector2.right * 2.5f * Time.deltaTime;
		_textureOffset_2.x = _textureOffset_2.x % 1f;

		_renderer_ShootLine.material.SetTextureScale("_MainTex", _tiling_2);
		_renderer_ShootLine.material.SetTextureOffset("_MainTex", _textureOffset_2);
	}
}
