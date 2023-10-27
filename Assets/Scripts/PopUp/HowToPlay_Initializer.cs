using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HowToPlay_Initializer : MonoBehaviour
{
	public UILabel _label_Guide_1;
	public UILabel _label_Guide_2;
	public UILabel _label_Guide_3;
	public UILabel _label_Guide_4;
	public UILabel _label_Guide_5;

	public List<Transform> _transform_GuideFinger_List;

	public UIBasicSprite _texture_SwipeArrow;
	public List<UIBasicSprite> _texture_GuideArrow_List;
	public List<HowToPlay_Ball> _ball_List;
	public List<HowToPlay_Trail> _trail_List;
	public List<HowToPlay_PickUp> _pickUp_List;
	public List<HowToPlay_Brick> _phase_1_brick_1_List;
	public List<HowToPlay_Brick> _phase_1_brick_2_List;
	public List<HowToPlay_BreakEffect> _breakEffect_List;
	public List<HowToPlay_Brick> _phase_4_brick_1_List;
	public List<HowToPlay_Brick> _phase_4_brick_2_List;
	public List<HowToPlay_Brick> _phase_4_brick_3_List;
	public List<HowToPlay_Brick> _phase_4_brick_4_List;
	public List<HowToPlay_Brick> _phase_4_brick_5_List;
	public List<HowToPlay_Brick> _phase_4_brick_6_List;
	public List<HowToPlay_Brick> _phase_4_brick_7_List;
	public List<HowToPlay_Brick> _phase_4_brick_8_List;

	public void Initialize()
	{
		Initialize_Text ();
		Initialize_Image ();
	}

	void Initialize_Text()
	{
		_label_Guide_1.color = Static_ColorConfigs._Color_TextOrange;
		_label_Guide_2.color = Static_ColorConfigs._Color_TextOrange;
		_label_Guide_3.color = Static_ColorConfigs._Color_TextOrange;
		_label_Guide_4.color = Static_ColorConfigs._Color_TextOrange;
		_label_Guide_5.color = Static_ColorConfigs._Color_TextOrange;
		
		_label_Guide_1.text = Static_TextConfigs._HowToPlay_1;
		_label_Guide_2.text = Static_TextConfigs._HowToPlay_2;
		_label_Guide_3.text = Static_TextConfigs._HowToPlay_3;
		_label_Guide_4.text = Static_TextConfigs._HowToPlay_4;
		_label_Guide_5.text = Static_TextConfigs._HowToPlay_5;
	}

	void Initialize_Image ()
	{
		Vector3 startPosition = Vector3.up * (-614.5f);
		Vector3 endPosition = Vector3.right * 256f + Vector3.up * (-171f);
		for(int i = 0; i < _transform_GuideFinger_List.Count; i++)
		{
			float lerpRatio = 0.5f + 0.5f* (float)i/(float)(_transform_GuideFinger_List.Count-1);
			_transform_GuideFinger_List[i].localPosition = Vector3.Lerp(startPosition, endPosition, lerpRatio);
		}

		_texture_SwipeArrow.color = Static_ColorConfigs._Color_Ball;
		_texture_SwipeArrow.alpha = 0.5f;

		for(int i = 0; i < _texture_GuideArrow_List.Count; i++)
			_texture_GuideArrow_List[i].color = Static_ColorConfigs._Color_TextGray;

		for(int i = 0; i < _ball_List.Count; i++)
			_ball_List[i].Initialize();

		for(int i = 0; i < _trail_List.Count; i++)
			_trail_List[i].Initialize();

		for(int i = 0; i < _pickUp_List.Count; i++)
			_pickUp_List[i].Initialize();

		for(int i = 0; i < _phase_1_brick_1_List.Count; i++)
			_phase_1_brick_1_List[i].Initialize(Static_ColorConfigs._Color_Brick_Min, 1);
		
		for(int i = 0; i < _phase_1_brick_2_List.Count; i++)
			_phase_1_brick_2_List[i].Initialize(Static_ColorConfigs._Color_Brick_Max, 2);

		for(int i = 0; i < _breakEffect_List.Count; i++)
		{
			_breakEffect_List[i].Initialize(Static_ColorConfigs._Color_Brick_Min);
			Vector3 dir = _breakEffect_List[i].transform.localPosition - Vector3.down * 68f;
			float distance = (Vector3.right * dir.x + Vector3.up * dir.y * 2).magnitude;
			_breakEffect_List[i].transform.localPosition += dir.normalized * distance * 0.2f;
		}

		Color color = Static_ColorConfigs._Color_Brick_Min;
		for(int i = 0; i < _phase_4_brick_1_List.Count; i++)
			_phase_4_brick_1_List[i].Initialize(color, 1);

		color = Color.Lerp (Static_ColorConfigs._Color_Brick_Min, Static_ColorConfigs._Color_Brick_Max, 1f / 7f);
		for(int i = 0; i < _phase_4_brick_2_List.Count; i++)
			_phase_4_brick_2_List[i].Initialize(color, 2);

		color = Color.Lerp (Static_ColorConfigs._Color_Brick_Min, Static_ColorConfigs._Color_Brick_Max, 2f / 7f);
		for(int i = 0; i < _phase_4_brick_3_List.Count; i++)
			_phase_4_brick_3_List[i].Initialize(color, 3);

		color = Color.Lerp (Static_ColorConfigs._Color_Brick_Min, Static_ColorConfigs._Color_Brick_Max, 3f / 7f);
		for(int i = 0; i < _phase_4_brick_4_List.Count; i++)
			_phase_4_brick_4_List[i].Initialize(color, 4);

		color = Color.Lerp (Static_ColorConfigs._Color_Brick_Min, Static_ColorConfigs._Color_Brick_Max, 4f / 7f);
		for(int i = 0; i < _phase_4_brick_5_List.Count; i++)
			_phase_4_brick_5_List[i].Initialize(color, 5);

		color = Color.Lerp (Static_ColorConfigs._Color_Brick_Min, Static_ColorConfigs._Color_Brick_Max, 5f / 7f);
		for(int i = 0; i < _phase_4_brick_6_List.Count; i++)
			_phase_4_brick_6_List[i].Initialize(color, 6);

		color = Color.Lerp (Static_ColorConfigs._Color_Brick_Min, Static_ColorConfigs._Color_Brick_Max, 6f / 7f);
		for(int i = 0; i < _phase_4_brick_7_List.Count; i++)
			_phase_4_brick_7_List[i].Initialize(color, 7);

		color = Static_ColorConfigs._Color_Brick_Max;
		for(int i = 0; i < _phase_4_brick_8_List.Count; i++)
			_phase_4_brick_8_List[i].Initialize(color, 8);
	}
}
