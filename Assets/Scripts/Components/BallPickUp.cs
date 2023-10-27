using UnityEngine;
using System.Collections;

public class BallPickUp : MonoBehaviour
{
	public Transform _transform;
	public CircleCollider2D _collider;

	public Transform _transform_Model;
	public Renderer _renderer_Model;
	public Transform _transform_Outer;
	public Renderer _renderer_Outer;

	public Transform _transform_Shadow;
	public Renderer _renderer_Shadow;
	public Transform _transform_Shadow_Outer;
	public Renderer _renderer_Shadow_Outer;

	public PickUpData _data;
	public int _locationX { get { return _data._locationX; } set { _data._locationX = value; } }
	public int _locationY { get { return _data._locationY; } set { _data._locationY = value; } }
	public bool _isExpired = false;

	System.Action<BallPickUp, Vector3> _callback_Expired;

	public void Initialize(int locationX, int locationY, System.Action<BallPickUp, Vector3> expiredCallback)
	{
		useGUILayout = false;

		if(_data == null)
			_data = new PickUpData();

		_locationX = locationX;
		_locationY = locationY;

		_transform.localScale = Vector3.one;
		_transform.localEulerAngles = Vector3.zero;
		_transform.localPosition = Vector3.zero;

		_collider.enabled = false;
		_collider.isTrigger = true;
		_collider.radius = InGameController._BallRadius * 2f;

		_transform_Model.localScale = Vector3.one * InGameController._BallRadius * 2;
		_transform_Outer.localScale = Vector3.one * InGameController._BallRadius * 4;

		_transform_Shadow.localScale = Vector3.one * InGameController._BallRadius * 2;
		_transform_Shadow.localPosition = Vector3.forward * 0.1f;
		_transform_Shadow.position += InGameController._PositionModifier_Shadow;

		_transform_Shadow_Outer.localScale = Vector3.one * InGameController._BallRadius * 4;
		_transform_Shadow_Outer.localPosition = Vector3.forward * 0.1f;
		_transform_Shadow_Outer.position += InGameController._PositionModifier_Shadow;

		_renderer_Model.material = MaterialHolder._Material_BallPickUp;
		_renderer_Shadow.material = MaterialHolder._Material_BallPickUp_Shadow;
		_renderer_Shadow.enabled = false;
		_renderer_Outer.material = MaterialHolder._Material_PickUpOuter;
		_renderer_Shadow_Outer.material = MaterialHolder._Material_PickUpOuter_Shadow;
		_renderer_Shadow_Outer.enabled = false;

		_isExpired = false;

		_callback_Expired = expiredCallback;
	}

	public void GetPickUp(Vector3 impactPosition)
	{
		if(_isExpired == false)
		{
			SoundController.Play_Impact_PickUp();

			_isExpired = true;
			_collider.enabled = false;

			if(_callback_Expired != null)
				_callback_Expired(this, impactPosition);
		}
	}

	public void SetDarkMode()
	{
		_renderer_Model.material = MaterialHolder._Material_BallPickUp;
		_renderer_Outer.material = MaterialHolder._Material_PickUpOuter;
	}
}
