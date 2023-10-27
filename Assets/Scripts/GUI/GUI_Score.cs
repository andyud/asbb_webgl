using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_Score : MonoBehaviour
{
	public UILabel _label_Record_Title;
	public UILabel _label_Record_Value;
	public UILabel _label_Score_Title;
	public UILabel _label_Score_Value;

	ObjectPoolHolder _objectPool;
//	List<UILabel> _label_List;

//	System.Text.StringBuilder _strBuilder = new System.Text.StringBuilder();

	List<IEnumerator> _thread_Director_List;
	List<IEnumerator> _thread_Director_Expired_List;

	public void Initialize()
	{
		Color color = Static_ColorConfigs._Color_ButtonFrame;

		_label_Record_Title.color = color;
		_label_Record_Value.color = color;
		_label_Score_Title.color = color;
		_label_Score_Value.color = color;

		_label_Record_Title.text = Static_TextConfigs._Record_Title;
		_label_Score_Title.text = Static_TextConfigs._Score_Title;
//		_label_List = new List<UILabel> ();

		_thread_Director_List = new List<IEnumerator> ();
		_thread_Director_Expired_List = new List<IEnumerator> ();
	}

	public void Reset()
	{
		UpdateScore (1, 0, true);

		_thread_Director_List.Clear ();
		_thread_Director_Expired_List.Clear ();

//		for(int i = 0; i < _label_List.Count; i++)
//			ObjectPoolHolder.PutObject<UILabel>(_label_List[i].gameObject);

//		_label_List.Clear ();
	}

	public void UpdateScore(int score, int delta, bool isNewRecord)
	{
		if(isNewRecord)
		{
			_label_Record_Value.text = score.ToString ();
			_label_Record_Title.color = Static_ColorConfigs._Color_TextStrongGreen;
			_label_Record_Value.color = Static_ColorConfigs._Color_TextStrongGreen;
		}
		else
		{
			_label_Record_Value.text = UserData._Score_Best.ToString ();
			_label_Record_Title.color = Static_ColorConfigs._Color_ButtonFrame;
			_label_Record_Value.color = Static_ColorConfigs._Color_ButtonFrame;
		}

        _label_Score_Value.text = score.ToString ();

		_label_Score_Value.transform.localPosition = Vector3.right * (_label_Record_Value.transform.localPosition.x + _label_Record_Value.width);
		_label_Score_Value.transform.localPosition += Vector3.up * _label_Score_Title.transform.localPosition.y;

/*		if(delta > 0)
		{
			UILabel label = ObjectPool_GUI.GetObject<UILabel> (transform);

			_strBuilder.Remove(0, _strBuilder.Length);
			_strBuilder.AppendFormat("{0} {1}", "+", delta);
			label.text = _strBuilder.ToString();
			label.color = ColorConfigs._Color_TextStrongGreen;

			_label_List.Add(label);

			_thread_Director_List.Add(Direct_ValueChange(label, _label_Score_Value));
		}*/
	}

	IEnumerator Direct_ValueChange(UILabel label, UILabel pivotObject)
	{
		UIWidget.Pivot pivot = pivotObject.pivot;
		pivotObject.pivot = UIWidget.Pivot.Right;
		
		label.pivot = UIWidget.Pivot.Left;
		label.depth = pivotObject.depth + 1;
		
		label.transform.parent = pivotObject.transform.parent;
		label.transform.localScale = Vector3.one;
		label.transform.localPosition = pivotObject.transform.localPosition;
		
		pivotObject.pivot = pivot;
		
		
		
		float elapsedTime = 0;
		float duration = 1.0f;
		AnimationCurve curve = CurveHolder._DeAcceleratedRising;
		
		float startAlpha = 1;
		float endAlpha = 0;
		Vector3 startLocalPosition = label.transform.localPosition;
		Vector3 endLocalPosition = startLocalPosition + Vector3.right * 48;

		label.gameObject.SetActive (true);
		while(elapsedTime < duration)
		{
			float lerpRatio = elapsedTime / duration;
			label.alpha = Mathf.Lerp(startAlpha, endAlpha, lerpRatio);
			label.transform.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, curve.Evaluate(lerpRatio));
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		label.alpha = endAlpha;
		label.transform.localPosition = endLocalPosition;
//		_label_List.Remove (label);

//		ObjectPoolHolder.PutObject (label.gameObject);
	}
	
	public void MoveNext()
	{
		for(int i = 0; i < _thread_Director_List.Count; i++)
		{
			if(_thread_Director_List[i].MoveNext() == false)
				_thread_Director_Expired_List.Add(_thread_Director_List[i]);
		}

		for (int i = 0; i < _thread_Director_Expired_List.Count; i++)
			_thread_Director_List.Remove(_thread_Director_Expired_List[i]);

		_thread_Director_Expired_List.Clear ();
	}
}
