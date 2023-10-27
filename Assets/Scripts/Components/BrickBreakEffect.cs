using UnityEngine;
using System.Collections;

public class BrickBreakEffect : MonoBehaviour
{
	public Transform _transform;
	public Rigidbody2D _rigidbody;
	public Transform _transform_Model;
	public Renderer _renderer_Model;
	public Transform _transform_Shadow;
	public Renderer _renderer_Shadow;

//	Vector3 _force;
//	IEnumerator _thread;

	public void Initialize()
	{
		useGUILayout = false;

		_transform.localScale = Vector3.one;
		_transform.localEulerAngles = Vector3.zero;

//		_rigidbody.useGravity = false;
		_rigidbody.gravityScale = 0f;
		_rigidbody.isKinematic = true;

		_transform_Shadow.localPosition = Vector3.forward * 0.1f;
		_transform_Shadow.position += Vector3.right * 0.1f + Vector3.down * 0.2f;

		_renderer_Model.material = MaterialHolder._Material_BreakEffect;
		_renderer_Shadow.material = MaterialHolder._Material_BreakEffect_Shadow;

		_renderer_Shadow.enabled = !UserData._OnDarkMode;

		//		_force = Vector3.zero;
		//		_thread = null;
	}

	public void SetScale(Vector3 scale)
	{
		_transform_Model.localScale = scale;
		_transform_Shadow.localScale = scale;
	}

	public void SetMaterial(Material modelMat, Material shadowMat)
	{
		_renderer_Model.material = modelMat;
		_renderer_Shadow.material = shadowMat;
	}

	public void AddForce(Vector2 force, Vector3 position)
	{
//		_rigidbody.useGravity = true;
		_rigidbody.gravityScale = 1.5f;
		_rigidbody.isKinematic = false;
		_rigidbody.velocity = Vector2.zero;
//		_rigidbody.angularVelocity = Vector3.zero;
		_rigidbody.angularVelocity = 0f;

		_rigidbody.AddForceAtPosition (force, position);
//		_rigidbody.AddForce(force);

//		_force = force;
//		_thread = Thread_Move ();
	}

/*	IEnumerator Thread_Move()
	{
		while(true)
		{
			Vector3 moveVector = Vector3.down * 9.81f * Time.deltaTime;
			moveVector += _force * Time.deltaTime;
			_transform.localPosition += moveVector;
			yield return 0;
		}
	}

	public void MoveNext()
	{
		if(_thread != null)
			_thread.MoveNext();
	}*/
}
