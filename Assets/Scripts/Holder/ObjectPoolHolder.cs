using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolHolder : MonoBehaviour
{
	static ObjectPoolHolder _instance;
	
	public GameObject _prefab_Ball;
	public GameObject _prefab_Brick;
	public GameObject _prefab_BrickBreakEffect;
	public GameObject _prefab_BallPickUp;

	static Dictionary<Type, GameObject> _prefab_ByType_Dic = new Dictionary<Type, GameObject>();

	static Dictionary<Type, List<GameObject>> _objectList_ByType_Dic = new Dictionary<Type, List<GameObject>>();

	public static void Initialize()
	{
		_instance = FindObjectOfType<ObjectPoolHolder>();

		_prefab_ByType_Dic.Add(typeof(Ball), _instance._prefab_Ball);
		_prefab_ByType_Dic.Add(typeof(Brick), _instance._prefab_Brick);
		_prefab_ByType_Dic.Add(typeof(BrickBreakEffect), _instance._prefab_BrickBreakEffect);
		_prefab_ByType_Dic.Add(typeof(BallPickUp), _instance._prefab_BallPickUp);

		_objectList_ByType_Dic.Add(typeof(Ball), new List<GameObject>());
		_objectList_ByType_Dic.Add(typeof(Brick), new List<GameObject>());
		_objectList_ByType_Dic.Add(typeof(BrickBreakEffect), new List<GameObject>());
		_objectList_ByType_Dic.Add(typeof(BallPickUp), new List<GameObject>());

		Transform parentTransform = _instance.transform;
		List<Ball> ballList = new List<Ball>();
		List<BallPickUp> pickUpList = new List<BallPickUp>();
		List<Brick> brickList = new List<Brick>();
		List<BrickBreakEffect> effectList = new List<BrickBreakEffect>();
		
		for (int i = 0; i < 100; i++)
			ballList.Add(GetObject<Ball>(parentTransform));

		for (int i = 0; i < 10; i++)
			pickUpList.Add(GetObject<BallPickUp>(parentTransform));

		for (int i = 0; i < 60; i++)
			brickList.Add(GetObject<Brick>(parentTransform));

		for (int i = 0; i < 100; i++)
			effectList.Add(GetObject<BrickBreakEffect>(parentTransform));
		
		PutObjectList<Ball>(ballList);
		PutObjectList<BallPickUp>(pickUpList);
		PutObjectList<Brick>(brickList);
		PutObjectList<BrickBreakEffect>(effectList);
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

			if (go.activeSelf)
				go.SetActive(false);

			_objectList_ByType_Dic[typeof(T)].Remove(go);
			//			_object_List.Remove(go);

			return go.GetComponent<T>();
		}

		return null;
	}

	static GameObject GetObjectOnStandBy<T>() where T : MonoBehaviour
	{
		List<GameObject> objectList = _objectList_ByType_Dic[typeof(T)];
		if (objectList.Count > 0)
			return objectList[0];
		else
		{
			if (_prefab_ByType_Dic.ContainsKey(typeof(T)))
				return Instantiate(_prefab_ByType_Dic[typeof(T)], Vector3.zero, Quaternion.identity) as GameObject;
			else
				return null;
		}
	}

	public static void PutObject<T>(GameObject go) where T : MonoBehaviour
	{
		List<GameObject> objectList = _objectList_ByType_Dic[typeof(T)];
		if (objectList.Contains(go) == false)
			objectList.Add(go);

		go.transform.parent = _instance.transform;
		if (go.activeSelf)
			go.SetActive(false);
	}

	public static void PutObjectList<T>(List<T> list) where T : MonoBehaviour
	{
		for (int i = 0; i < list.Count; i++)
        			PutObject<T>(list[i].gameObject);
			}
}
