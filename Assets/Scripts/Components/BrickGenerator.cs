using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrickGenerator : MonoBehaviour
{
	public static BrickGenerator _instance;

	public Transform _transform;
	public List<Brick> _brick_List = new List<Brick> ();
	public List<Brick> _brick_Expired_List = new List<Brick> ();
	public List<BallPickUp> _pickUp_List = new List<BallPickUp>();
	public List<BrickBreakEffect> _breakEffect_List = new List<BrickBreakEffect>();

	public bool _IsGameOver;

	IEnumerator _thread_SetNewTurn;
	IEnumerator _thread_PickUpPumping;
	List<IEnumerator> _thread_BreakEffect_List = new List<IEnumerator>();
	List<IEnumerator> _thread_Expired_List = new List<IEnumerator>();

	static System.Random _random = new System.Random();

	public void Initialize()
	{
		_instance = this;
	}

	public bool IsWarningGameOver()
	{
		for(int i = 0; i < _brick_List.Count; i++)
		{
			if(_brick_List[i]._locationY == 1)
				return true;
		}

		return false;
	}

	public void Reset()
	{
		_IsGameOver = false;

		ObjectPoolHolder.PutObjectList<Brick> (_brick_List);
		_brick_List.Clear ();

		ObjectPoolHolder.PutObjectList<Brick> (_brick_Expired_List);
		_brick_Expired_List.Clear ();

		ObjectPoolHolder.PutObjectList<BallPickUp> (_pickUp_List);
		_pickUp_List.Clear ();

		ObjectPoolHolder.PutObjectList<BrickBreakEffect> (_breakEffect_List);
		_breakEffect_List.Clear ();

		_thread_SetNewTurn = null;
		_thread_PickUpPumping = null;
		_thread_BreakEffect_List.Clear ();

		_thread_PickUpPumping = Thread_PickUpPumping ();
	}

	public void Continue()
	{
		float left = -1 * InGameController._CurrentStageWidth * 0.5f + InGameController._BrickWidth * 0.5f;
		float bottom = InGameController._BrickHeight * 0.5f;

		bool isGameover = false;
		for(int i = 0; i < InGameController._BrickData_List.Count; i++)
		{
			BrickData data = InGameController._BrickData_List[i];

			Vector3 position = Vector3.right * (left + InGameController._BrickWidth * data._locationX) + Vector3.up * (bottom + InGameController._BrickHeight * data._locationY);
			Vector3 scale = Vector3.right * InGameController._BrickWidth + Vector3.up * InGameController._BrickHeight + Vector3.forward;
			scale -= Vector3.one * InGameController._BrickHeight * 0.05f;
			
			Brick brick = ObjectPoolHolder.GetObject<Brick>(_transform);
			brick.Initialize(data._locationX, data._locationY, data._hp, Callback_Destroyed);
			brick.SetScale(scale);
			brick._transform.localPosition = position;
			brick.gameObject.SetActive(true);

			brick._collider.enabled = true;
			brick.Refresh();
			
			_brick_List.Add(brick);

			if(data._locationY <= 0)
				isGameover = true;
		}

		for(int i = 0; i < InGameController._PickUpData_List.Count; i++)
		{
			PickUpData data = InGameController._PickUpData_List[i];

			Vector3 position = Vector3.right * (left + InGameController._BrickWidth * data._locationX) + Vector3.up * (bottom + InGameController._BrickHeight * data._locationY);
			
			BallPickUp pickUp = ObjectPoolHolder.GetObject<BallPickUp>(_transform);
			pickUp.Initialize(data._locationX, data._locationY, CallBack_PickUp);
			pickUp._transform.localPosition = position;
			pickUp.gameObject.SetActive(true);

			pickUp._collider.enabled = true;

			_pickUp_List.Add(pickUp);
		}

		if(isGameover)
			_IsGameOver = true;
		else
			_IsGameOver = false;
	}

	IEnumerator Thread_PickUpPumping()
	{
		Vector3 maxScale = Vector3.one * InGameController._BallRadius * 4 * 1f;
		Vector3 minScale = Vector3.one * InGameController._BallRadius * 4f * 2f / 3f;

		for(int i = 0; i < _pickUp_List.Count; i++)
		{
			_pickUp_List[i]._transform_Outer.localScale = maxScale;
			_pickUp_List[i]._transform_Shadow_Outer.localScale = maxScale;
		}

		float elapsedTime = 0;
		float frequancy = 1;
		float lerpRatio = 0;
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;

		while(true)
		{
			if(elapsedTime < frequancy*0.5f)
			{
				lerpRatio = elapsedTime / (frequancy*0.5f);
				for(int i = 0; i < _pickUp_List.Count; i++)
				{
					_pickUp_List[i]._transform_Outer.localScale = Vector3.Lerp(maxScale, minScale, curveAccel.Evaluate(lerpRatio));
					_pickUp_List[i]._transform_Shadow_Outer.localScale = Vector3.Lerp(maxScale, minScale, curveAccel.Evaluate(lerpRatio));
				}
			}
			else
			{
				lerpRatio = (elapsedTime-frequancy*0.5f) / (frequancy*0.5f);
				for(int i = 0; i < _pickUp_List.Count; i++)
				{
					_pickUp_List[i]._transform_Outer.localScale = Vector3.Lerp(minScale, maxScale, curveDeAccel.Evaluate(lerpRatio));
					_pickUp_List[i]._transform_Shadow_Outer.localScale = Vector3.Lerp(minScale, maxScale, curveDeAccel.Evaluate(lerpRatio));
				}
			}

			yield return 0;
			elapsedTime += Time.deltaTime;
			elapsedTime = elapsedTime%frequancy;
		}
	}

	public void SetNewTurn(System.Action endCallback)
	{
		_thread_SetNewTurn = Thread_SetNewTurn (endCallback);
	}

	IEnumerator Thread_SetNewTurn(System.Action endCallback)
	{
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;

		float left = -1 * InGameController._CurrentStageWidth * 0.5f + InGameController._BrickWidth * 0.5f;
		float top = InGameController._CurrentStageWidth - InGameController._BrickHeight * 0.5f;

		int createCount = GetCreateCount ();

		bool[] createSlot = new bool[InGameController._BrickCount_Width];
		for(int i = 0; i < createSlot.Length; i++)
			createSlot[i] = false;
		
		//		Debug.LogWarning("Initialize CreateSlot Start : " + createCount);
		
		for(int i = 0; i < createCount; i++)
		{
			//			Debug.LogWarning("CreateSlot : " + i);
			bool b = false;
			while(b == false)
			{
				int index = _random.Next(0, createSlot.Length);
				if(createSlot[index] == false)
				{
					createSlot[index] = true;
					b = true;
				}
			}
		}
		
		//		Debug.LogWarning("Initialize CreateSlot End");
		
		
		
		int ballPickUpCount = 1;

		if(ballPickUpCount + createCount > InGameController._BrickCount_Width)
			ballPickUpCount = InGameController._BrickCount_Width - createCount;
		bool[] pickUpSlot = new bool[InGameController._BrickCount_Width];
		for(int i = 0; i < pickUpSlot.Length; i++)
			pickUpSlot[i] = false;
		
		//		Debug.LogWarning("Initialize PickUpSlot Start : " + ballPickUpCount);
		
		for(int i = 0; i < ballPickUpCount; i++)
		{
			//			Debug.LogWarning("PickUpSlot : " + i);
			
			bool b = false;
			while(b == false)
			{
				int index = _random.Next(0, pickUpSlot.Length);
				if(pickUpSlot[index] == false && createSlot[index] == false)
				{
					pickUpSlot[index] = true;
					b = true;
				}
			}
		}
		
		//		Debug.LogWarning("Initialize PickUpSlot End");
		
		
		
		//		Debug.LogWarning("Create Brick Start");
		
		List<Brick> newBrick_List = new List<Brick> ();
		for(int i = 0; i < createSlot.Length; i++)
		{
			if(createSlot[i] == true)
			{
				Vector3 position = Vector3.right * (left + InGameController._BrickWidth * i) + Vector3.up * top;
				Vector3 scale = Vector3.right * InGameController._BrickWidth + Vector3.up * InGameController._BrickHeight + Vector3.forward;
				scale -= Vector3.one * InGameController._BrickHeight * 0.05f;
				
				Brick brick = ObjectPoolHolder.GetObject<Brick>(_transform);
				brick.Initialize(i, InGameController._BrickCount_Height-1, InGameController._CurrentTurn, Callback_Destroyed);
				brick.SetScale(scale);
				brick._transform.localPosition = position;
				brick.gameObject.SetActive(true);
				
				newBrick_List.Add(brick);
				_brick_List.Add(brick);
			}
		}
		
		//		Debug.LogWarning("Create Brick End");
		
		//		Debug.LogWarning("Create PickUp Start");
		
		List<BallPickUp> newPickUp_List = new List<BallPickUp> ();
		for(int i = 0; i < pickUpSlot.Length; i++)
		{
			if(pickUpSlot[i] == true)
			{
				Vector3 position = Vector3.right * (left + InGameController._BrickWidth * i) + Vector3.up * top;
				
				BallPickUp pickUp = ObjectPoolHolder.GetObject<BallPickUp>(_transform);
				pickUp.Initialize(i, InGameController._BrickCount_Height-1, CallBack_PickUp);
				pickUp._transform.localPosition = position;
				pickUp.gameObject.SetActive(true);
				
				newPickUp_List.Add(pickUp);
				_pickUp_List.Add(pickUp);
			}
		}
		
		//		Debug.LogWarning("Create PickUp End");
		
		Vector3[] startScale_List = new Vector3[_brick_List.Count];
		Vector3[] endScale_List = new Vector3[_brick_List.Count];
		Vector3[] pickScale_List = new Vector3[_brick_List.Count];
		
		for(int i = 0; i < newBrick_List.Count; i++)
		{
			startScale_List[i] = Vector3.zero;
			endScale_List[i] = newBrick_List[i]._transform_Model.localScale;
			pickScale_List[i] = endScale_List[i] + Vector3.one * InGameController._BrickHeight * 0.05f;		

			newBrick_List[i]._transform_Model.localScale = startScale_List[i];
		}
		
		Vector3 startPickUpScale = Vector3.zero;
		Vector3 endPickUpScale = Vector3.one;
		Vector3 pickPickUpScale = endPickUpScale * 1.1f;
		
		float elapsedTime = 0;
		float pickTime = 0.2f;
		float duration = 0.25f;
		float lerpRatio = 0;
		
		//		Debug.LogWarning("Direct Create Start");
		
		while(elapsedTime < duration)
		{
			if(elapsedTime < pickTime)
			{
				lerpRatio = elapsedTime / pickTime;
				for(int i = 0; i < newBrick_List.Count; i++)
					newBrick_List[i].SetScale(Vector3.Lerp(startScale_List[i], pickScale_List[i], curveDeAccel.Evaluate(lerpRatio)));
				
				for(int i = 0; i < newPickUp_List.Count; i++)
					newPickUp_List[i]._transform.localScale = Vector3.Lerp(startPickUpScale, pickPickUpScale, curveDeAccel.Evaluate(lerpRatio));
			}
			else
			{
				lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);
				for(int i = 0; i < newBrick_List.Count; i++)
					newBrick_List[i].SetScale(Vector3.Lerp(pickScale_List[i], endScale_List[i], curveAccel.Evaluate(lerpRatio)));
				
				for(int i = 0; i < newPickUp_List.Count; i++)
					newPickUp_List[i]._transform.localScale = Vector3.Lerp(pickPickUpScale, endPickUpScale, curveAccel.Evaluate(lerpRatio));
			}
			
			//			Debug.LogWarning("[Yield] : Direct Create");
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		//		Debug.LogWarning("Direct Create End");
		
		for(int i = 0; i < newBrick_List.Count; i++)
			newBrick_List[i].SetScale(endScale_List[i]);

		for(int i = 0; i < newPickUp_List.Count; i++)
			newPickUp_List[i]._transform.localScale = endPickUpScale;






		elapsedTime = 0;
		pickTime = 0.2f;
		duration = 0.25f;
		lerpRatio = 0;

		if(_brick_List.Count > 0 || _pickUp_List.Count > 0)
		{
//			Debug.LogWarning("MoveDown Start");

			Vector3[] startPosition_List = new Vector3[_brick_List.Count];
			Vector3[] endPosition_List = new Vector3[_brick_List.Count];
			Vector3[] pickPosition_List = new Vector3[_brick_List.Count];

			Color[] startColor_List = new Color[_brick_List.Count];
			Color[] endColor_List = new Color[_brick_List.Count];
		
			for(int i = 0; i < _brick_List.Count; i++)
			{
				Brick brick = _brick_List[i];
				brick._collider.enabled = false;

				startPosition_List[i] = brick._transform.localPosition;
				endPosition_List[i] = brick._transform.localPosition + Vector3.down * InGameController._BrickHeight;
				pickPosition_List[i] = brick._transform.localPosition + Vector3.down * InGameController._BrickHeight * 1.5f;

				float endHue = Static_ColorConfigs.GetBrickHue(brick._HP, InGameController._CurrentTurn);
				float endChroma = Static_ColorConfigs.GetBrickChroma(brick._HP, InGameController._CurrentTurn);
				float endLuma = Static_ColorConfigs.GetBrickLuma(brick._HP, InGameController._CurrentTurn);

				startColor_List[i] = brick._renderer_Model.material.color;
				endColor_List[i] = Static_ColorConfigs.GetColor_From_HCL(endHue, endChroma, endLuma);
			}

			Vector3[] startPickUpPosition = new Vector3[_pickUp_List.Count];
			Vector3[] endPickUpPosition = new Vector3[_pickUp_List.Count];
			Vector3[] pickPickUpPosition = new Vector3[_pickUp_List.Count];

			for(int i = 0; i < _pickUp_List.Count; i++)
			{
				BallPickUp pickUp = _pickUp_List[i];
				pickUp._collider.enabled = false;

				startPickUpPosition[i] = pickUp._transform.localPosition;
				endPickUpPosition[i] = pickUp._transform.localPosition + Vector3.down * InGameController._BrickHeight;
				pickPickUpPosition[i] = pickUp._transform.localPosition + Vector3.down * InGameController._BrickHeight * 1.5f;
			}

			bool isGameOver = false;
			for(int i = 0; i < _brick_List.Count; i++)
			{
				_brick_List[i]._locationY--;
				if(_brick_List[i]._locationY <= 0)
					isGameOver = true;
			}

			for(int i = 0; i < _pickUp_List.Count; i++)
				_pickUp_List[i]._locationY--;

		
			if(isGameOver)
			{
				elapsedTime = 0;
				duration = 0.25f;
				lerpRatio = 0;
			
				while(elapsedTime < duration)
				{
					lerpRatio = elapsedTime / duration;
					for(int i = 0; i < _brick_List.Count; i++)
					{
						_brick_List[i]._renderer_Model.material.color = Color.Lerp(startColor_List[i], endColor_List[i], curveDeAccel.Evaluate(lerpRatio));
						_brick_List[i]._transform.localPosition = Vector3.Lerp(startPosition_List[i], endPosition_List[i], curveAccel.Evaluate(lerpRatio*lerpRatio));
					}

					for(int i = 0; i < _pickUp_List.Count; i++)
						_pickUp_List[i]._transform.localPosition = Vector3.Lerp(startPickUpPosition[i], endPickUpPosition[i], curveAccel.Evaluate(lerpRatio*lerpRatio));
				
//					Debug.LogWarning("[Yield] : GameOver MoveDown");
					yield return 0;
					elapsedTime += Time.deltaTime;
				}
			
				for(int i = 0; i < _brick_List.Count; i++)
				{
					_brick_List[i]._transform.localPosition = endPosition_List[i];
					_brick_List[i].Refresh();
				}

				for(int i = 0; i < _pickUp_List.Count; i++)
					_pickUp_List[i]._transform.localPosition = endPickUpPosition[i];
			
				_IsGameOver = true;
			}
			else
			{
				elapsedTime = 0;
				pickTime = 0.2f;
				duration = 0.3f;
				lerpRatio = 0;
			
//				Debug.LogWarning("MoveDown Direct Start");

				while(elapsedTime < duration)
				{
					if(elapsedTime < pickTime)
					{
						lerpRatio = elapsedTime / pickTime;
						for(int i = 0; i < _brick_List.Count; i++)
						{
							_brick_List[i]._renderer_Model.material.color = Color.Lerp(startColor_List[i], endColor_List[i], curveDeAccel.Evaluate(lerpRatio));
							_brick_List[i]._transform.localPosition = Vector3.Lerp(startPosition_List[i], pickPosition_List[i], curveDeAccel.Evaluate(lerpRatio));
						}

						for(int i = 0; i < _pickUp_List.Count; i++)
							_pickUp_List[i]._transform.localPosition = Vector3.Lerp(startPickUpPosition[i], pickPickUpPosition[i], curveDeAccel.Evaluate(lerpRatio));
					}
					else
					{
						lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);
						for(int i = 0; i < _brick_List.Count; i++)
							_brick_List[i]._transform.localPosition = Vector3.Lerp(pickPosition_List[i], endPosition_List[i], curveAccel.Evaluate(lerpRatio));

						for(int i = 0; i < _pickUp_List.Count; i++)
							_pickUp_List[i]._transform.localPosition = Vector3.Lerp(pickPickUpPosition[i], endPickUpPosition[i], curveAccel.Evaluate(lerpRatio));
					}

//					Debug.LogWarning("[Yield] : MoveDown");
					yield return 0;
					elapsedTime += Time.deltaTime;
				}
			}

			for(int i = 0; i < _brick_List.Count; i++)
			{
				_brick_List[i]._transform.localPosition = endPosition_List[i];
				_brick_List[i].Refresh();
			}

			for(int i = 0; i < _pickUp_List.Count; i++)
			{
				_pickUp_List[i]._transform.localPosition = endPickUpPosition[i];
			}
//			Debug.LogWarning("MoveDown End");
		}

		for(int i = 0; i < _brick_List.Count; i++)
		{
			_brick_List[i]._collider.enabled = true;
		}
		
		for(int i = 0; i < _pickUp_List.Count; i++)
		{
			_pickUp_List[i]._collider.enabled = true;
		}

		if(endCallback != null)
			endCallback();
	}

	public int GetCreateCount()
	{
		float ratio_1 = 0.0f;
		float ratio_2 = 0.0f;
		float ratio_3 = 0.0f;
		float ratio_4 = 0.0f;
		float ratio_5 = 0.0f;

		int currentTurn = InGameController._CurrentTurn;
		if(currentTurn <= 5)
		{
			ratio_1 = 0.5f;
			ratio_2 = 1f;
		}
		else if(currentTurn <= 10)
		{
			float lerpRatio = (float)(currentTurn-5) / (float)(10-5);
			ratio_1 = Mathf.Lerp(0.5f, 1f/3f, lerpRatio);
			ratio_2 = ratio_1 + Mathf.Lerp(0.5f, 1f/3f, lerpRatio);
			ratio_3 = 1f;
		}
		else if(currentTurn <= 20)
		{
			float lerpRatio = (float)(currentTurn-10) / (float)(20-10);
			ratio_1 = Mathf.Lerp(1f/3f, 0, lerpRatio);
			ratio_2 = ratio_1 + Mathf.Lerp(1f/3f, 0.4f, lerpRatio);
			ratio_3 = ratio_2 + Mathf.Lerp(1f/3f, 0.4f, lerpRatio);
			ratio_4 = 1f;
		}
		else if(currentTurn <= 50)
		{
			float lerpRatio = (float)(currentTurn-20) / (float)(50-20);
			ratio_1 = 0;
			ratio_2 = ratio_1 + Mathf.Lerp(0.4f, 0.325f, lerpRatio);
			ratio_3 = ratio_2 + Mathf.Lerp(0.4f, 0.325f, lerpRatio);
			ratio_4 = ratio_3 + Mathf.Lerp(0.2f, 0.225f, lerpRatio);
			ratio_5 = 1;
		}
		else if(currentTurn <= 100)
		{
			float lerpRatio = (float)(currentTurn-50) / (float)(100-50);
			ratio_1 = 0;
			ratio_2 = ratio_1 + Mathf.Lerp(0.325f, 0.25f, lerpRatio);
			ratio_3 = ratio_2 + Mathf.Lerp(0.325f, 0.25f, lerpRatio);
			ratio_4 = ratio_3 + Mathf.Lerp(0.225f, 0.25f, lerpRatio);
			ratio_5 = 1;
		}
		else if(currentTurn <= 300)
		{
			float lerpRatio = (float)(currentTurn-100) / (float)(300-100);
			ratio_1 = 0;
			ratio_2 = ratio_1 + Mathf.Lerp(0.25f, 0.25f, lerpRatio);
			ratio_3 = ratio_2 + Mathf.Lerp(0.25f, 0.25f, lerpRatio);
			ratio_4 = ratio_3 + Mathf.Lerp(0.25f, 0.25f, lerpRatio);
			ratio_5 = 1;
		}
		else if(currentTurn <= 1000)
		{
			float lerpRatio = (float)(currentTurn-300) / (float)(1000-300);
			ratio_1 = 0;
			ratio_2 = ratio_1 + Mathf.Lerp(0.25f, 0.0f, lerpRatio);
			ratio_3 = ratio_2 + Mathf.Lerp(0.25f, 0.40f, lerpRatio);
			ratio_4 = ratio_3 + Mathf.Lerp(0.25f, 0.35f, lerpRatio);
			ratio_5 = 1;
		}
		else
		{
			ratio_1 = 0;
			ratio_2 = ratio_1 + 0.0f;
			ratio_3 = ratio_2 + 0.4f;
			ratio_4 = ratio_3 + 0.35f;
			ratio_5 = 1;
		}

		float criteria = (float)(_random.NextDouble ());

/*		float r1 = ratio_1;
		float r2 = Mathf.Max (0, ratio_2 - ratio_1);
		float r3 = Mathf.Max (0, ratio_3 - ratio_2);
		float r4 = Mathf.Max (0, ratio_4 - ratio_3);
		float r5 = Mathf.Max (0, ratio_5 - ratio_4);
		float rTotal = r1 + r2 + r3 + r4 + r5;
		Debug.LogWarning ("[1] : " + r1 + " / [2] : " + r2 + " / [3] : " + r3 + " / [4] : " + r4 + " / [5] : " + r5 + " / [TOTAL] : " + rTotal + " / CRITERA : " + criteria);
*/
		if (criteria < ratio_1)
			return 1;
		else if (criteria < ratio_2)
			return 2;
		else if (criteria < ratio_3)
			return 3;
		else if (criteria < ratio_4)
			return 4;
		else if (criteria < ratio_5)
			return 5;
		else
			return 3;
	}

	public void Callback_Destroyed(Brick brick, Vector3 impactPosition)
	{
		bool isPowermode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1;

		int effectCount_Width = isPowermode ? 9 : 6;
		int effectCount_Height = isPowermode ? 6 : 4;

		float positionGap_X = brick._transform_Model.localScale.x / effectCount_Width;
		float positionGap_Y = brick._transform_Model.localScale.y / effectCount_Height;
		Vector3 effectScale = Vector3.right * positionGap_X + Vector3.up * positionGap_Y;

		Vector3 initialPosition = brick._transform.position + (Vector3.left * (brick._transform_Model.localScale.x - positionGap_X) + Vector3.up * (brick._transform_Model.localScale.y - positionGap_Y)) * 0.5f;

		Material modelMat = new Material (MaterialHolder._Material_BreakEffect);
		modelMat.color = brick._renderer_Model.material.color;
		Material shadowMat = new Material (MaterialHolder._Material_BreakEffect_Shadow);
		shadowMat.color = brick._renderer_Shadow.material.color;

		List<BrickBreakEffect> list = new List<BrickBreakEffect> ();
		int multiValue = isPowermode ? 5 : 1;

		for(int i = 0; i < effectCount_Width; i++)
		{
			for(int j = 0; j < effectCount_Height; j++)
			{
				BrickBreakEffect effect = ObjectPoolHolder.GetObject<BrickBreakEffect>(_transform);
				effect.Initialize();
				effect.SetScale(effectScale);
				effect.SetMaterial(modelMat, shadowMat);
				effect._transform.position = initialPosition + Vector3.right * positionGap_X * i + Vector3.down * positionGap_Y * j;
				effect.gameObject.SetActive(true);

				_breakEffect_List.Add(effect);
				list.Add(effect);

				float power = 120;
				power += Random.Range(-1f, 1f) * 12f;
				float angle = Static_Calculator.XYMeter2Angle((effect._transform.position - impactPosition));
				angle += Random.Range(-1f, 1f) * 30f;

				Vector2 force = Static_Calculator.Vector2XYForce(angle, power);
				effect.AddForce(force * multiValue, impactPosition * multiValue);
			}
		}

		_thread_BreakEffect_List.Add(Thread_BreakEffect_Brick(list, modelMat, shadowMat));

		_brick_List.Remove (brick);
		_brick_Expired_List.Add (brick);
	}

	IEnumerator Thread_BreakEffect_Brick(List<BrickBreakEffect> list, Material modelMat, Material shadowMat)
	{
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
		
		float elapsedTime = 0;
		float pickTime = 0.75f;
		float duration = 1.5f;
		float lerpRatio = 0;
		
		float matFloatRatio = 0.25f;
		
		float pickHue = Mathf.Lerp(Static_ColorConfigs._Hue_Brick_Min, Static_ColorConfigs._Hue_Brick_Max, matFloatRatio);
		float pickChroma = Mathf.Lerp(Static_ColorConfigs._Chroma_Brick_Min, Static_ColorConfigs._Chroma_Brick_Max, matFloatRatio);
		float pickLuma = Mathf.Lerp(Static_ColorConfigs._Luma_Brick_Min, Static_ColorConfigs._Luma_Brick_Max, matFloatRatio);
		
		float endHue = Static_ColorConfigs._Hue_Brick_Min;
		float endChroma = Static_ColorConfigs._Chroma_Brick_Min;
		float endLuma = Static_ColorConfigs._Luma_Brick_Min;
		
		Color startColor = modelMat.color;
		Color pickColor = Static_ColorConfigs.GetColor_From_HCL (pickHue, pickChroma, pickLuma);
		Color endColor = Static_ColorConfigs.GetColor_From_HCL (endHue, endChroma, endLuma);
		endColor.a = 0;
		
		Color pickShadowColor = shadowMat.color;
		Color endShadowColor = shadowMat.color;
		endShadowColor.a = 0;
		
		
		Vector3 startScale = Vector3.one;
		Vector3 endScale = Vector3.one * 0.5f;
		
		while(elapsedTime < duration)
		{
			lerpRatio = elapsedTime / duration;
			for(int i = 0; i < list.Count; i++)
				list[i]._transform.localScale = Vector3.Lerp(startScale, endScale, curveAccel.Evaluate(lerpRatio));
			
			if(elapsedTime < pickTime)
			{
				lerpRatio = elapsedTime / pickTime;
				modelMat.color = Color.Lerp(startColor, pickColor, curveDeAccel.Evaluate(lerpRatio));
			}
			else
			{
				lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);
				modelMat.color = Color.Lerp(pickColor, endColor, curveAccel.Evaluate(lerpRatio));
				shadowMat.color = Color.Lerp(pickShadowColor, endShadowColor, curveAccel.Evaluate(lerpRatio));
			}
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		for(int i = 0; i < list.Count; i++)
		{
			_breakEffect_List.Remove(list[i]);
			ObjectPoolHolder.PutObject<BrickBreakEffect>(list[i].gameObject);
		}
	}

	void CallBack_PickUp(BallPickUp pickUp, Vector3 impactPosition)
	{
		CreateBreakEffect_PickUp (pickUp, impactPosition);

		_pickUp_List.Remove (pickUp);
		ObjectPoolHolder.PutObject<BallPickUp> (pickUp.gameObject);
	}

	void CreateBreakEffect_PickUp(BallPickUp pickUp, Vector3 impactPosition)
	{
		int effectCount = 24;
		float angleGap = 360f / effectCount;
		
		float radiusMax = InGameController._BallRadius * 2f;
		float radiusMin = radiusMax * 3f / 4f;
		
		float outerDistance_Max = pickUp._transform_Outer.localScale.x * 0.5f;
		float scaleRatio = outerDistance_Max / radiusMax;
		float outerDistance_Min = radiusMin * scaleRatio;
		Vector3 effectScale = Vector3.one * (outerDistance_Max - outerDistance_Min);
		float outerDistance = (outerDistance_Max + outerDistance_Min)*0.5f;
		
		Vector3 initialPosition = pickUp._transform.position;
		
		Material modelMat = new Material (MaterialHolder._Material_BreakEffect);
		Material shadowMat = new Material (MaterialHolder._Material_BreakEffect_Shadow);
		modelMat.color = Static_ColorConfigs._Color_PickUp;
		shadowMat.color = Static_ColorConfigs._Color_PickUp_Shadow;

		List<BrickBreakEffect> list = new List<BrickBreakEffect> ();
		for(int i = 0; i < effectCount; i++)
		{
			BrickBreakEffect effect = ObjectPoolHolder.GetObject<BrickBreakEffect>(_transform);
			effect.Initialize();
			effect.SetScale(effectScale);
			effect.SetMaterial(modelMat, shadowMat);
			effect._transform.position = initialPosition + Static_Calculator.Vector2To3(Static_Calculator.Vector2XYForce(angleGap*i, outerDistance));
			effect.gameObject.SetActive(true);
			
			_breakEffect_List.Add(effect);
			list.Add(effect);

			float power = 120;
			power += Random.Range(-1f, 1f) * 12f;
			float angle = Static_Calculator.XYMeter2Angle((effect._transform.position - impactPosition));
			angle += Random.Range(-1f, 1f) * 30f;

			Vector2 force = Static_Calculator.Vector2XYForce(angle, power);

			effect.AddForce(force, initialPosition);
		}
		
		_thread_BreakEffect_List.Add(Thread_BreakEffect_PickUp(list, modelMat, shadowMat));
	}

	IEnumerator Thread_BreakEffect_PickUp(List<BrickBreakEffect> list, Material modelMat, Material shadowMat)
	{
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
		
		float elapsedTime = 0;
		float pickTime = 0.75f;
		float duration = 1.5f;
		float lerpRatio = 0;


		Color startColor = modelMat.color;
		Color pickColor = Static_ColorConfigs.GetColor_From_HCL (Static_ColorConfigs._Hue_PickUpBall, Static_ColorConfigs._Chroma_Brick_Max, Static_ColorConfigs._Luma_Brick_Max);
		Color endColor = Static_ColorConfigs.GetColor_From_HCL (Static_ColorConfigs._Hue_PickUpBall, Static_ColorConfigs._Chroma_Brick_Min, Static_ColorConfigs._Luma_Brick_Min);
		endColor.a = 0;

		Color pickShadowColor = shadowMat.color;
		Color endShadowColor = shadowMat.color;
		endShadowColor.a = 0;

		Vector3 startScale = Vector3.one;
		Vector3 pickScale = Vector3.one * 2f;
		Vector3 endScale = Vector3.one * 0.5f;

		while(elapsedTime < duration)
		{
			lerpRatio = elapsedTime / duration;
			if(elapsedTime < pickTime)
			{
				lerpRatio = elapsedTime / pickTime;

				modelMat.color = Color.Lerp(startColor, pickColor, curveDeAccel.Evaluate(lerpRatio));

				for(int i = 0; i < list.Count; i++)
					list[i]._transform.localScale = Vector3.Lerp(startScale, pickScale, curveDeAccel.Evaluate(lerpRatio));
			}
			else
			{
				lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);

				modelMat.color = Color.Lerp(pickColor, endColor, curveAccel.Evaluate(lerpRatio));
				shadowMat.color = Color.Lerp(pickShadowColor, endShadowColor, curveAccel.Evaluate(lerpRatio));

				for(int i = 0; i < list.Count; i++)
					list[i]._transform.localScale = Vector3.Lerp(pickScale, endScale, curveAccel.Evaluate(lerpRatio));
			}
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		for(int i = 0; i < list.Count; i++)
		{
			_breakEffect_List.Remove(list[i]);
			ObjectPoolHolder.PutObject<BrickBreakEffect>(list[i].gameObject);
		}
	}

	public static List<BallPickUp> GetBottomPickUp_List()
	{
		List<BallPickUp> list = new List<BallPickUp> ();
		for(int i = 0; i < _instance._pickUp_List.Count; i++)
		{
			BallPickUp pickUp = _instance._pickUp_List[i];
			if(pickUp._locationY <= 1)
				list.Add(pickUp);
		}

		return list;
	}

	public void SetDarkMode()
	{
		if(_brick_List != null)
		{
			for (int i = 0; i < _brick_List.Count; i++)
			{
				_brick_List[i].Refresh();
			}
		}

		if(_pickUp_List != null)
		{
			for (int i = 0; i < _pickUp_List.Count; i++)
			{
				_pickUp_List[i].SetDarkMode();
			}
		}
	}

	public void MoveNext()
	{
		if(_thread_PickUpPumping != null)
			_thread_PickUpPumping.MoveNext();

		if(_thread_SetNewTurn != null)
		{
			if(_thread_SetNewTurn.MoveNext() == false)
				_thread_SetNewTurn = null;
		}

		for(int i = 0; i < _brick_List.Count; i++)
			_brick_List[i].MoveNext();

		for(int i = 0; i < _thread_BreakEffect_List.Count; i++)
		{
			if(_thread_BreakEffect_List[i].MoveNext() == false)
				_thread_Expired_List.Add(_thread_BreakEffect_List[i]);
		}

		for(int i = 0; i < _thread_Expired_List.Count; i++)
			_thread_BreakEffect_List.Remove(_thread_Expired_List[i]);

		_thread_Expired_List.Clear ();

		if(_brick_Expired_List.Count > 0)
		{
			ObjectPoolHolder.PutObjectList<Brick> (_brick_Expired_List);
			_brick_Expired_List.Clear();
		}
	}
}
