using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
	public Transform _transform;
	public CircleCollider2D _collider;
	public Rigidbody2D _rigidbody;
	public Transform _transform_Model;
	public Renderer _renderer_Model;
	public Transform _transform_Shadow;
	public Renderer _renderer_Shadow;
	public TrailRenderer _trail;
	public TrailRenderer _trailShadow;

	public bool _onMoving;
	public bool _onDirect;
	public bool _isHitted;

	public int _power;
	public int _frameDelay;

	System.Action<Ball, Collider2D> _callback_Trigger;

	public Vector2 _velocity_Previous;



	public void Initialize(float radius)
	{
		useGUILayout = false;

		_transform.localScale = Vector3.one;
		_transform.localEulerAngles = Vector3.zero;

		_rigidbody.isKinematic = true;
		_collider.enabled = false;
		_collider.radius = radius;

		_transform_Model.localScale = Vector3.one * radius * 2;

		_transform_Shadow.localScale = Vector3.one * radius * 2;
		_transform_Shadow.localPosition = Vector3.forward * 0.1f;
		_transform_Shadow.position += InGameController._PositionModifier_Shadow;
		_trailShadow.transform.localPosition = Vector3.forward * 0.1f;
		_trailShadow.transform.position += InGameController._PositionModifier_Shadow;

		_renderer_Model.material = MaterialHolder._Material_PlayerBall;
		_renderer_Shadow.material = MaterialHolder._Material_BallShadow;
		_renderer_Shadow.enabled = false;
		_trail.material = MaterialHolder._Material_BallTrail;
		_trailShadow.material = MaterialHolder._Material_BallTrailShadow;
		_trailShadow.enabled = false;


		_trail.gameObject.SetActive (false);
		_trailShadow.gameObject.SetActive (false);

		_trail.time = 0;
		_trailShadow.time = 0;

		_onMoving = false;
		_onDirect = false;
		_callback_Trigger = null;
	}

	public void Sleep()
	{
		_onMoving = false;

		if(_rigidbody.isKinematic == false)
		{
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.Sleep ();
			_rigidbody.isKinematic = true;
		}

		_collider.enabled = false;
	}

	public void OffTrail()
	{
		_trail.time = 0;
		_trailShadow.time = 0;
	}

	public void Shoot(Vector3 velocity, System.Action<Ball, Collider2D> triggerCallback)
	{
		_callback_Trigger = triggerCallback;

		_trail.time = 0.4f;
		_trailShadow.time = 0.4f;

		_onMoving = true;
		_isHitted = false;
		_rigidbody.isKinematic = false;
		_rigidbody.velocity = velocity;
		_collider.enabled = true;
		_trail.gameObject.SetActive (true);
		_trailShadow.gameObject.SetActive (true);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(_callback_Trigger != null)
			_callback_Trigger(this, col);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		Brick brick = col.collider.GetComponent<Brick> ();
		if(brick != null)
		{
			Vector3 impactDir = _rigidbody.velocity.normalized;
			Vector3 impactPosition = col.contacts[0].point;

			_isHitted = true;

			if (brick.DealDamage(_power, impactDir, impactPosition, this, _velocity_Previous))
			{
				//_isHitted = true;
			}
			else
				_rigidbody.velocity = _velocity_Previous;
		}
	}

	public void FixedMoveNext()
	{
	}

	public void MoveNext()
	{
	}
}
