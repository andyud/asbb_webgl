using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{
	public Transform _transform;
	public BoxCollider2D _collider;
	public Rigidbody2D _rigidbody;
	public Transform _modelHolder;
	public Transform _transform_Model;
	public Renderer _renderer_Model;
	public Transform _transform_Shadow;
	public Renderer _renderer_Shadow;
	public TextMesh _textmesh;

	public BrickData _data;
	public int _HP {get {return _data._hp;} set{_data._hp = value;}}
	public int _locationX {get {return _data._locationX;} set{_data._locationX = value;}}
	public int _locationY {get {return _data._locationY;} set{_data._locationY = value;}}

	bool _isDestroyed;

	System.Action<Brick, Vector3> _callback_Destroyed;

	IEnumerator _thread_Director;


	public void Initialize(int locationX, int locationY, int hp, System.Action<Brick, Vector3> destroyedCallback)
	{
		useGUILayout = false;

		_callback_Destroyed = destroyedCallback;

		if(_data == null)
			_data = new BrickData();

		_locationX = locationX;
		_locationY = locationY;
		_HP = hp;

		_transform.localScale = Vector3.one;
		_transform.localEulerAngles = Vector3.zero;
		_transform.localPosition = Vector3.zero;

		_collider.enabled = false;
		_renderer_Model.enabled = true;
		_renderer_Shadow.enabled = false;
		_textmesh.gameObject.SetActive(true);

		_modelHolder.transform.localScale = Vector3.one;

		_transform_Model.localScale = Vector3.one;
		_renderer_Model.material = MaterialHolder._Material_Brick;

		_transform_Shadow.localScale = Vector3.one;
		_transform_Shadow.localPosition = Vector3.forward * 0.1f;
		_transform_Shadow.position += InGameController._PositionModifier_Shadow;
		_renderer_Shadow.material = MaterialHolder._Material_BrickShadow;

		_textmesh.font = FontHolder.GetFont ();
		_textmesh.GetComponent<Renderer>().material = _textmesh.font.material;
		_textmesh.transform.localScale = Vector3.one * 0.15f;

		_thread_Director = null;

		Refresh ();
	}

	public void SetScale(Vector3 scale)
	{
		_collider.size = scale;
		_transform_Model.localScale = scale;
		_transform_Shadow.localScale = scale;
	}

	public void IncreaseHP()
	{
		_HP++;
	}

	public void Refresh()
	{
		_textmesh.text = _HP.ToString ();
		//_textmesh.color = ColorConfigs._Color_TextWhite;
		_textmesh.color = UserData._OnDarkMode ? Static_ColorConfigs._Color_Background : Static_ColorConfigs._Color_TextWhite;

		float hue = Static_ColorConfigs.GetBrickHue (_HP, InGameController._CurrentTurn);
		float chroma = Static_ColorConfigs.GetBrickChroma (_HP, InGameController._CurrentTurn);
		float luma = Static_ColorConfigs.GetBrickLuma (_HP, InGameController._CurrentTurn);
		_renderer_Model.material.color = Static_ColorConfigs.GetColor_From_HCL (hue, chroma, luma);
		_renderer_Shadow.material.color = Static_ColorConfigs.GetColor_From_HCL (hue, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
	}

	public bool DealDamage(int power, Vector3 impactDir, Vector3 impactPosition, Ball ball, Vector2 nextPos)
	{
		if(_HP > 0)
		{
			SoundController.Play_Impact_Brick();

			_HP -= power;
			_textmesh.text = _HP.ToString ();

			if(_HP > 0)
				_thread_Director = Direct_Hitted ();
			else
			{
				_renderer_Model.enabled = false;
				_renderer_Shadow.enabled = false;
				_textmesh.gameObject.SetActive(false);

				if(_callback_Destroyed != null)
					_callback_Destroyed(this, impactPosition);

                if (_HP < 0)
                {
                    ball._power = Mathf.Abs(_HP);
                    ball._rigidbody.velocity = nextPos;
                    return false;
                }
            }
			return true;
		}
		else
			return false;
	}

	IEnumerator Direct_Hitted()
	{
		float elapsedTime = 0;
		float pickTime = InGameController._SecPerFrame * 4;
		float duration = pickTime * 3f;


		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;

/*		float startHue = ColorConfigs.GetBrickHue (_HP + 1, InGameController._CurrentTurn);
		float startChroma = ColorConfigs.GetBrickChroma (_HP + 1, InGameController._CurrentTurn);
		float startLuma = ColorConfigs.GetBrickLuma (_HP + 1, InGameController._CurrentTurn);

		float endHue = ColorConfigs.GetBrickHue (_HP, InGameController._CurrentTurn);
		float endChroma = ColorConfigs.GetBrickChroma (_HP, InGameController._CurrentTurn);
		float endLuma = ColorConfigs.GetBrickLuma (_HP, InGameController._CurrentTurn);

		float pickHue = Mathf.Lerp (startHue, endHue, pickTime / duration);
		float pickChroma = Mathf.Lerp (startChroma, endChroma, pickTime / duration);
		float pickLuma = ColorConfigs._Luma_Hit;

		Color startColor = ColorConfigs.GetColor_From_HCL (startHue, startChroma, startLuma);
		Color pickColor = ColorConfigs.GetColor_From_HCL (pickHue, pickChroma, pickLuma);
		Color endColor = ColorConfigs.GetColor_From_HCL (endHue, endChroma, endLuma);

		Color startShadowColor = ColorConfigs.GetColor_From_HCL (startHue, ColorConfigs._Chroma_Shadow, ColorConfigs._Luma_Shadow);
		Color pickShadowColor = ColorConfigs.GetColor_From_HCL (pickHue, ColorConfigs._Chroma_Shadow, ColorConfigs._Luma_Shadow);
		Color endShadowColor = ColorConfigs.GetColor_From_HCL (endHue, ColorConfigs._Chroma_Shadow, ColorConfigs._Luma_Shadow);
*/
		float defaultHue = Static_ColorConfigs.GetBrickHue (_HP + 1, InGameController._CurrentTurn);
		float defaultChroma = Static_ColorConfigs.GetBrickChroma (_HP + 1, InGameController._CurrentTurn);
		float defaultLuma = Static_ColorConfigs.GetBrickLuma (_HP + 1, InGameController._CurrentTurn);

		float endHue = Static_ColorConfigs.GetBrickHue (_HP, InGameController._CurrentTurn);
		float endChroma = Static_ColorConfigs.GetBrickChroma (_HP, InGameController._CurrentTurn);
		float endLuma = Static_ColorConfigs.GetBrickLuma (_HP, InGameController._CurrentTurn);

		float pickHue = Mathf.Lerp (defaultHue, endHue, pickTime / duration);
		float pickChroma = Mathf.Lerp (defaultChroma, endChroma, pickTime / duration);
		float pickLuma = Static_ColorConfigs._Luma_Hit;

		Color defaultColor = Static_ColorConfigs.GetColor_From_HCL (defaultHue, defaultChroma, defaultLuma);

		Color startColor = Color.Lerp (_renderer_Model.material.color, defaultColor, 0.5f);
		Color pickColor = Static_ColorConfigs.GetColor_From_HCL (pickHue, pickChroma, pickLuma);
		Color endColor = Static_ColorConfigs.GetColor_From_HCL (endHue, endChroma, endLuma);

		Color startShadowColor = _renderer_Shadow.material.color;
		Color pickShadowColor = Static_ColorConfigs.GetColor_From_HCL (pickHue, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		Color endShadowColor = Static_ColorConfigs.GetColor_From_HCL (endHue, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);

		float lerpRatio = 0;

		while(elapsedTime < duration)
		{
			if(elapsedTime < pickTime)
			{
				lerpRatio = elapsedTime / pickTime;
				_renderer_Model.material.color = Color.Lerp(startColor, pickColor, curveDeAccel.Evaluate(lerpRatio));
				_renderer_Shadow.material.color = Color.Lerp(startShadowColor, pickShadowColor, curveDeAccel.Evaluate(lerpRatio));
			}
			else
			{
				lerpRatio = (elapsedTime-pickTime) / (duration - pickTime);
				_renderer_Model.material.color = Color.Lerp(pickColor, endColor, curveAccel.Evaluate(lerpRatio));
				_renderer_Shadow.material.color = Color.Lerp(pickShadowColor, endShadowColor, curveAccel.Evaluate(lerpRatio));
			}

			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		_renderer_Model.material.color = endColor;
		_renderer_Shadow.material.color = endShadowColor;

		Refresh ();
	}

	public void MoveNext()
	{
		if(_thread_Director != null)
		{
			if(_thread_Director.MoveNext() == false)
				_thread_Director = null;
		}
	}
}
