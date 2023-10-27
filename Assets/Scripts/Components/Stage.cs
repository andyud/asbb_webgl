using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage : MonoBehaviour
{
	Transform _transform;
	public Transform _wall_Left;
	public Transform _wall_Right;
	public Transform _wall_Top;
	public Transform _wall_Bottom;
	public Transform _finishLine;

	public Renderer _renderer_Line_Top;
	public Renderer _renderer_Line_Bottom;

	float _stageWidth;

	public List<BrickBreakEffect> _breakEffect_Top_List = new List<BrickBreakEffect>();
	public List<BrickBreakEffect> _breakEffect_Bottom_List = new List<BrickBreakEffect>();

	IEnumerator _thread_Direct;
	bool _isLineBroken;

	public void Initialize()
	{
		_isLineBroken = false;

		_transform = transform;

		_renderer_Line_Top.material = MaterialHolder._Material_Line;
		_renderer_Line_Bottom.material = MaterialHolder._Material_Line;

		_wall_Bottom.gameObject.SetActive (false);
		_finishLine.gameObject.SetActive (false);

		_thread_Direct = null;
	}

	public void Reset()
	{
		ObjectPoolHolder.PutObjectList<BrickBreakEffect> (_breakEffect_Top_List);
		_breakEffect_Top_List.Clear ();

		ObjectPoolHolder.PutObjectList<BrickBreakEffect> (_breakEffect_Bottom_List);
		_breakEffect_Bottom_List.Clear ();
	}

	public void SetStageWidth(float stageWidth)
	{
		_stageWidth = stageWidth;

		_transform.localPosition = Vector3.up * stageWidth * 0.5f;

		_wall_Left.localScale = Vector3.one * stageWidth * 2;
		_wall_Left.localPosition = Vector3.left * (stageWidth + _wall_Left.localScale.x) * 0.5f;
		
		_wall_Right.localScale = Vector3.one * stageWidth * 2;
		_wall_Right.localPosition = Vector3.right * (stageWidth + _wall_Right.localScale.x) * 0.5f;
		
		_wall_Top.localScale = Vector3.one * stageWidth * 2;
		_wall_Top.localPosition = Vector3.up * (stageWidth + _wall_Top.localScale.y) * 0.5f;

		_renderer_Line_Top.transform.localScale = Vector3.right * stageWidth + Vector3.up * stageWidth * 0.0125f;
		_renderer_Line_Top.transform.localPosition = Vector3.up * (stageWidth + _renderer_Line_Top.transform.localScale.y) * 0.5f;
		
		_renderer_Line_Bottom.transform.localScale = Vector3.right * stageWidth + Vector3.up * stageWidth * 0.0125f;
		_renderer_Line_Bottom.transform.localPosition = Vector3.down * (stageWidth + _renderer_Line_Top.transform.localScale.y) * 0.5f;

		Vector3 effectScale = Vector3.one * stageWidth * 0.0125f;
		effectScale.x = effectScale.x * 2;
		int effectCount = (int)((float)stageWidth / effectScale.x)+1;
		for(int i = 0; i < effectCount; i++)
		{
			BrickBreakEffect effect = ObjectPoolHolder.GetObject<BrickBreakEffect>(_transform);
			effect.Initialize();
			effect.SetScale(effectScale);
			effect._transform.position = Vector3.right*(stageWidth * (-0.5f) + effectScale.x*0.5f + effectScale.x*i) + Vector3.up * _renderer_Line_Bottom.transform.position.y;

			_breakEffect_Bottom_List.Add(effect);



			effect = ObjectPoolHolder.GetObject<BrickBreakEffect>(_transform);
			effect.Initialize();
			effect.SetScale(effectScale);
			effect._transform.position = Vector3.right*(stageWidth * (-0.5f) + effectScale.x*0.5f + effectScale.x*i) + Vector3.up * _renderer_Line_Top.transform.position.y;
			
			_breakEffect_Top_List.Add(effect);
		}
	}

	public void Direct_CreateLine()
	{
		if(_isLineBroken == false)
			return;

		_isLineBroken = false;

		_renderer_Line_Top.gameObject.SetActive (false);
		_renderer_Line_Bottom.gameObject.SetActive (false);

		Material modelMat = new Material (_renderer_Line_Top.material);
		Color color = modelMat.color;
		color.a = 0;
		modelMat.color = color;

		Material shadowMat = new Material (_renderer_Line_Top.material);
		color = Color.white * 0.8f;
		color.a = 0;
		shadowMat.color = color;

		BrickBreakEffect effect;
		for(int i = 0; i < _breakEffect_Top_List.Count; i++)
		{
			effect = _breakEffect_Top_List[i];
			effect.SetMaterial(modelMat, shadowMat);
			effect.gameObject.SetActive(true);

			float distance = 5;
			distance += Random.Range(-1f, 1f) * 0.5f;
			float angle = 270 + 30 * effect._transform.position.x / (_stageWidth*0.5f);
			angle += Random.Range(-1f, 1f) * 15f;
			Vector3 startPosition = Static_Calculator.Vector2To3(Static_Calculator.Vector2XYForce(angle, distance));
			effect._transform.position += startPosition;



			effect = _breakEffect_Bottom_List[i];
			effect.SetMaterial(modelMat, shadowMat);
			effect.gameObject.SetActive(true);

			distance = 5;
			distance += Random.Range(-1f, 1f) * 0.5f;
			angle = 270 + 30 * effect._transform.position.x / (_stageWidth*0.5f);
			angle += Random.Range(-1f, 1f) * 15f;
			startPosition = Static_Calculator.Vector2To3(Static_Calculator.Vector2XYForce(angle, distance));
			effect._transform.position += startPosition;
		}

		_thread_Direct = Thread_CreateLine (modelMat, shadowMat);
	}

	IEnumerator Thread_CreateLine(Material modelMat, Material shadowMat)
	{
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
		
		float elapsedTime = 0;
//		float pickTime = 0.75f;
		float duration = 1.5f;
		float lerpRatio = 0;
		
		Color startColor_Model = modelMat.color;
		Color endColor_Model = startColor_Model;
		endColor_Model.a = 1;
		Color startColor_Shadow = shadowMat.color;
		Color endColor_Shadow = startColor_Shadow;
		endColor_Shadow.a = 1;
		
		Vector3 startScale = Vector3.one * 0.5f;
		Vector3 endScale = Vector3.one;

		Vector3[] startPosition_Top_Array = new Vector3[_breakEffect_Top_List.Count];
		Vector3[] endPosition_Top_Array = new Vector3[_breakEffect_Top_List.Count];
		Vector3[] startPosition_Bottom_Array = new Vector3[_breakEffect_Bottom_List.Count];
		Vector3[] endPosition_Bottom_Array = new Vector3[_breakEffect_Bottom_List.Count];

		Vector3 effectScale = Vector3.one * _stageWidth * 0.0125f;
		effectScale.x = effectScale.x * 2;
		for(int i = 0; i < _breakEffect_Top_List.Count; i++)
		{
			startPosition_Top_Array[i] = _breakEffect_Top_List[i]._transform.position;
			startPosition_Bottom_Array[i] = _breakEffect_Bottom_List[i]._transform.position;

			endPosition_Top_Array[i] = Vector3.right*(_stageWidth * (-0.5f) + effectScale.x*0.5f + effectScale.x*i) + Vector3.up * _renderer_Line_Top.transform.position.y;
			endPosition_Bottom_Array[i] = Vector3.right*(_stageWidth * (-0.5f) + effectScale.x*0.5f + effectScale.x*i) + Vector3.up * _renderer_Line_Bottom.transform.position.y;
		}
		
		while(elapsedTime < duration)
		{
			lerpRatio = elapsedTime / duration;
			for(int i = 0; i < _breakEffect_Top_List.Count; i++)
			{
				_breakEffect_Top_List[i]._transform.localScale = Vector3.Lerp(startScale, endScale, curveAccel.Evaluate(lerpRatio));
				_breakEffect_Top_List[i]._transform.position = Vector3.Lerp(startPosition_Top_Array[i], endPosition_Top_Array[i], curveDeAccel.Evaluate(lerpRatio));

				_breakEffect_Bottom_List[i]._transform.localScale = Vector3.Lerp(startScale, endScale, curveAccel.Evaluate(lerpRatio));
				_breakEffect_Bottom_List[i]._transform.position = Vector3.Lerp(startPosition_Bottom_Array[i], endPosition_Bottom_Array[i], curveDeAccel.Evaluate(lerpRatio));
			}
			
//			if(elapsedTime < pickTime)
//			{
//				lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);
				modelMat.color = Color.Lerp(startColor_Model, endColor_Model, curveDeAccel.Evaluate(lerpRatio));
				shadowMat.color = Color.Lerp(startColor_Shadow, endColor_Shadow, curveDeAccel.Evaluate(lerpRatio));
//			}
//			else
//			{
//				modelMat.color = endColor_Model;
//				shadowMat.color = endColor_Shadow;
//			}
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		for(int i = 0; i < _breakEffect_Top_List.Count; i++)
		{
			_breakEffect_Top_List[i]._transform.position = endPosition_Top_Array[i];
			_breakEffect_Top_List[i]._transform.localScale = endScale;
			_breakEffect_Top_List[i].gameObject.SetActive(false);

			_breakEffect_Bottom_List[i]._transform.position = endPosition_Bottom_Array[i];
			_breakEffect_Bottom_List[i]._transform.localScale = endScale;
			_breakEffect_Bottom_List[i].gameObject.SetActive(false);
		}

		_renderer_Line_Top.gameObject.SetActive (true);
		_renderer_Line_Bottom.gameObject.SetActive (true);
	}

	public void Direct_BreakLine()
	{
		_isLineBroken = true;

		_renderer_Line_Top.gameObject.SetActive (false);
		_renderer_Line_Bottom.gameObject.SetActive (false);

		Material modelMat = new Material (_renderer_Line_Top.material);
		Material shadowMat = new Material (_renderer_Line_Top.material);
		shadowMat.color = Color.white * 0.8f;

		BrickBreakEffect effect;
		for(int i = 0; i < _breakEffect_Top_List.Count; i++)
		{
			effect = _breakEffect_Top_List[i];
			effect.SetMaterial(modelMat, shadowMat);
			effect.gameObject.SetActive(true);

			float power = 60;
			power += Random.Range(-1f, 1f) * 6f;
//			float angle = Calculator.XYMeter2Angle((effect._transform.position - Vector3.zero));
//			angle += Random.Range(-1f, 1f) * 15f;
			float angle = 270 + 60 * effect._transform.position.x / (_stageWidth*0.5f);
			angle += Random.Range(-1f, 1f) * 15f;
			Vector2 force = Static_Calculator.Vector2XYForce(angle, power);
			effect.AddForce(force, Vector3.zero);

			effect = _breakEffect_Bottom_List[i];
			effect.SetMaterial(modelMat, shadowMat);
			effect.gameObject.SetActive(true);

			power = 60;
			power += Random.Range(-1f, 1f) * 6f;
			angle = 270 + 60 * effect._transform.position.x / (_stageWidth*0.5f);
			angle += Random.Range(-1f, 1f) * 15f;
//			angle = Calculator.XYMeter2Angle((effect._transform.position - Vector3.zero));
//			angle += Random.Range(-1f, 1f) * 15f;
			force = Static_Calculator.Vector2XYForce(angle, power);
			effect.AddForce(force, Vector3.zero);
		}

		_thread_Direct = Thread_BreakLine (modelMat, shadowMat);
	}

	IEnumerator Thread_BreakLine(Material modelMat, Material shadowMat)
	{
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
//		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
		
		float elapsedTime = 0;
		float pickTime = 0.75f;
		float duration = 1.5f;
		float lerpRatio = 0;

		Color startColor_Model = modelMat.color;
		Color endColor_Model = startColor_Model;
		endColor_Model.a = 0;
		Color startColor_Shadow = shadowMat.color;
		Color endColor_Shadow = startColor_Shadow;
		endColor_Shadow.a = 0;
				
		Vector3 startScale = Vector3.one;
		Vector3 endScale = Vector3.one * 0.5f;

		while(elapsedTime < duration)
		{
			lerpRatio = elapsedTime / duration;
			for(int i = 0; i < _breakEffect_Top_List.Count; i++)
			{
				_breakEffect_Top_List[i]._transform.localScale = Vector3.Lerp(startScale, endScale, curveAccel.Evaluate(lerpRatio));
				_breakEffect_Bottom_List[i]._transform.localScale = Vector3.Lerp(startScale, endScale, curveAccel.Evaluate(lerpRatio));
			}
			
			if(elapsedTime < pickTime)
			{
//				lerpRatio = elapsedTime / pickTime;
//				modelMat.SetFloat("_Hue", Mathf.Lerp(startHue, pickHue, curveDeAccel.Evaluate(lerpRatio)));
//				modelMat.SetFloat("_Chroma", Mathf.Lerp(startChroma, pickChroma, curveDeAccel.Evaluate(lerpRatio)));
//				modelMat.SetFloat("_Luma", Mathf.Lerp(startLuma, pickLuma, curveDeAccel.Evaluate(lerpRatio)));
			}
			else
			{
				lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);
				modelMat.color = Color.Lerp(startColor_Model, endColor_Model, curveAccel.Evaluate(lerpRatio));
				shadowMat.color = Color.Lerp(startColor_Shadow, endColor_Shadow, curveAccel.Evaluate(lerpRatio));
			}
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		ObjectPoolHolder.PutObjectList(_breakEffect_Top_List);
		ObjectPoolHolder.PutObjectList(_breakEffect_Bottom_List);

		_breakEffect_Top_List.Clear();
		_breakEffect_Bottom_List.Clear();
	}

	public void MoveNext()
	{
//		for(int i = 0; i < _breakEffect_Top_List.Count; i++)
//			_breakEffect_Top_List[i].MoveNext();

//		for(int i = 0; i < _breakEffect_Bottom_List.Count; i++)
//			_breakEffect_Bottom_List[i].MoveNext();

		if(_thread_Direct != null)
		{
			if(_thread_Direct.MoveNext() == false)
				_thread_Direct = null;
		}
	}
}
