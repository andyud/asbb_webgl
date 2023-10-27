using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool_GUI : MonoBehaviour
{
	static ObjectPool_GUI _instance;
	static Dictionary<Type, GameObject> _prefab_ByType_Dic = new Dictionary<Type, GameObject>();
	static List<GameObject> _object_List = new List<GameObject>();
	public GameObject _prefab_UILabel;


	public static void Initialize()
	{
		_instance = FindObjectOfType<ObjectPool_GUI>();
		_instance._prefab_UILabel.GetComponent<UILabel>().trueTypeFont = FontHolder.GetFont();
		_instance._prefab_UILabel.GetComponent<UILabel>().fontStyle = FontStyle.Normal;

		_prefab_ByType_Dic.Add(typeof(UILabel), _instance._prefab_UILabel);

		Transform parentTransform = _instance.transform;
		List<GameObject> reserved = new List<GameObject>();
		for (int i = 0; i < 30; i++)
		{
			reserved.Add(GetObject<UILabel>(parentTransform).gameObject);
		}

		for (int i = 0; i < reserved.Count; i++)
			PutObject(reserved[i]);
	}

	public static T GetObject<T>(Transform parentTransform) where T : MonoBehaviour
	{
		GameObject go = GetObjectOnStandBy<T>();

		if (go != null)
		{
			Transform tran = go.transform;
			tran.parent = parentTransform;
			tran.localScale = Vector3.one;
			tran.localEulerAngles = Vector3.zero;
			tran.localPosition = Vector3.zero;

			go.SetActive(false);
			_object_List.Remove(go);

			return go.GetComponent<T>();
		}

		return null;
	}

	static GameObject GetObjectOnStandBy<T>() where T : MonoBehaviour
	{
		for (int i = 0; i < _object_List.Count; i++)
		{
			if (_object_List[i].GetComponent<T>() != null)
				return _object_List[i];
		}

		if (_prefab_ByType_Dic.ContainsKey(typeof(T)))
			return Instantiate(_prefab_ByType_Dic[typeof(T)], Vector3.zero, Quaternion.identity) as GameObject;
		else
			return null;
	}

	public static void PutObject(GameObject go)
	{
		if (_object_List.Contains(go) == false)
			_object_List.Add(go);

		go.transform.parent = _instance.transform;
		go.SetActive(false);
	}
}
