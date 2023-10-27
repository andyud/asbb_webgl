using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PopUp : MonoBehaviour
{
	public Transform _pivot_CloseEndTransform;
	public Transform _pivot_OpenEndTransform;

	public Collider[] _collider_Array;

	protected System.Action _callback_Close;



	public void Initialize (System.Action closeCallback)
	{
		_callback_Close = closeCallback;

		Initialize_PopUp ();
		UpdateColliderCash ();
	}

	protected void UpdateColliderCash()
	{
		_collider_Array = GetComponentsInChildren<Collider> ();
	}

	protected abstract void Initialize_PopUp ();
	public abstract void Refresh();

	protected void Close()
	{
		if(_callback_Close != null)
			_callback_Close();
	}

	public void EnableButtons(bool b)
	{
		for(int i = 0; i < _collider_Array.Length; i++)
			_collider_Array[i].enabled = b;
	}

    public abstract void SetUI();
}
