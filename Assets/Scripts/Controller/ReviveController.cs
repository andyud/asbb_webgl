using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveController : MonoBehaviour
{
	[HideInInspector]
	public bool IsActive { get; set; }

	private Action<bool> _callBack;

	private static ReviveController _instance;

	public static ReviveController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<ReviveController>();
				_instance.IsActive = false;
			}

			return _instance;
		}
	}

	public void CheckRevive(Action<bool> callback)
	{
		_callBack = callback;
		StartCoroutine(RevivePlay());
	}

	private IEnumerator RevivePlay()
	{
		if (PlayerPrefs.GetInt(PlayerPrefs_Config.ReviveCount, 0) <= 0)
		{
			if (_callBack != null)
				_callBack.Invoke(false);

			yield break;
		}
			

		// 마지막 줄에 벽돌이 있는지 체크
		List<Brick> brickBreakList = new List<Brick>();
		if (BrickGenerator._instance._brick_List.Exists(v => v._locationY.Equals(0)))
		{
			for(int i=0; i<Revive.DeleteLineCount; i++)
			{
				var list = BrickGenerator._instance._brick_List.FindAll(v => v._locationY.Equals(i));
				brickBreakList.AddRange(list);
			}

            PopUpController.Open_Revive();

			yield return new WaitUntil(() => PopUpController._IsPopUpOpened == false);
			yield return new WaitForEndOfFrame();
#if UNITY_ANDROID
			if(AdController.isReviveShow)
			{
				yield return new WaitUntil(() => AdController.isReviveClose);
				AdController.isReviveShow = false;
				AdController.isReviveClose = false;
			}
#endif
			yield return new WaitForEndOfFrame();

			if (IsActive)
			{
				Debug.Log("ReviveController.RevivePlay() / IsActive is true");

				IsActive = false;
				var reviveCount = PlayerPrefs.GetInt(PlayerPrefs_Config.ReviveCount, 0);
				PlayerPrefs.SetInt(PlayerPrefs_Config.ReviveCount, reviveCount - 1);

				if (brickBreakList != null)
				{
					for (int i = 0; i < brickBreakList.Count; i++)
					{
						BrickGenerator._instance.Callback_Destroyed(brickBreakList[i], Vector3.zero);
						yield return 0;
					}
				}

				if (_callBack != null)
					_callBack.Invoke(true);
			}
			else
			{
				if (_callBack != null)
					_callBack.Invoke(false);
			}
		}
	}
}
