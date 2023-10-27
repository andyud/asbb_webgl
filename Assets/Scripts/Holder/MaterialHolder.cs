using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialHolder : MonoBehaviour
{
	static MaterialHolder _instance;

	public Material _source_Line;

	public Material _source_Ball;
	public Material _source_BallTrail;
	public Material _source_Brick;
	public Material _source_AlphaQuad;
	public Material _source_PickUpOuter;
	public Material _source_Arrow;
	public Material _source_TouchGuideLine;

	public static Material _Material_Line;

	public static Material _Material_Arrow;
	public static Material _Material_ArrowLine;
	public static Material _Material_ShootDirectionLine;
	public static Material _Material_CollisionGuideBall;
	public static Material _Material_TouchGuideLine;

	public static Material _Material_PlayerBall_Position;
	public static Material _Material_PlayerBall;
	public static Material _Material_BallTrail;
	public static Material _Material_BallPickUp;
	public static Material _Material_PickUpOuter;
	public static Material _Material_Brick;
	public static Material _Material_BreakEffect;

	public static Material _Material_BallShadow_Position;
	public static Material _Material_BallShadow;
	public static Material _Material_BallTrailShadow;
	public static Material _Material_BallPickUp_Shadow;
	public static Material _Material_PickUpOuter_Shadow;
	public static Material _Material_BrickShadow;
	public static Material _Material_BreakEffect_Shadow;


	public static void Initialize()
	{
		_instance = FindObjectOfType<MaterialHolder>();



		Material mat = new Material(_instance._source_Line);
		mat.color = Static_ColorConfigs._Color_TextBlack;
		_Material_Line = mat;



		//		Color guideLineColor = Color.Lerp(ColorConfigs._Color_Ball, ColorConfigs._Color_Background, 0.5f);

		Color guideLineColor = Static_ColorConfigs._Color_Ball;
		guideLineColor.a = 0.5f;

		mat = new Material(_instance._source_Arrow);
		mat.color = guideLineColor;
		_Material_Arrow = mat;

		mat = new Material(_instance._source_AlphaQuad);
		mat.color = guideLineColor;
		_Material_ArrowLine = mat;


		guideLineColor = Static_ColorConfigs._Color_Ball;
		guideLineColor.a = 0.5f;

		mat = new Material(_instance._source_TouchGuideLine);
		mat.color = guideLineColor;
		_Material_ShootDirectionLine = mat;

		guideLineColor.a = 0.5f;

		mat = new Material(_instance._source_Ball);
		mat.color = guideLineColor;
		_Material_CollisionGuideBall = mat;

		guideLineColor = Static_ColorConfigs._Color_TextOrange;
		guideLineColor.a = 0.3f;

		mat = new Material(_instance._source_TouchGuideLine);
		mat.color = guideLineColor;
		_Material_TouchGuideLine = mat;




		mat = new Material(_instance._source_Ball);
		mat.color = Static_ColorConfigs._Color_Ball;
		_Material_PlayerBall_Position = mat;

		mat = new Material(_instance._source_Ball);
		mat.color = Static_ColorConfigs._Color_Ball;
		_Material_PlayerBall = mat;

		mat = new Material(_instance._source_BallTrail);
		mat.color = Static_ColorConfigs._Color_BallTrail;
		_Material_BallTrail = mat;

		mat = new Material(_instance._source_Ball);
		mat.color = Static_ColorConfigs._Color_PickUp;
		_Material_BallPickUp = mat;

		mat = new Material(_instance._source_PickUpOuter);
		mat.color = Static_ColorConfigs._Color_PickUp;
		_Material_PickUpOuter = mat;

		mat = new Material(_instance._source_Brick);
		mat.color = Static_ColorConfigs._Color_Brick_Min;
		_Material_Brick = mat;

		mat = new Material(_instance._source_AlphaQuad);
		mat.color = Static_ColorConfigs._Color_PickUp;
		_Material_BreakEffect = mat;

		mat = new Material(_instance._source_Ball);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(Static_ColorConfigs._Hue_Ball, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_BallShadow_Position = mat;

		mat = new Material(_instance._source_Ball);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(Static_ColorConfigs._Hue_Ball, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_BallShadow = mat;

		mat = new Material(_instance._source_Ball);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(Static_ColorConfigs._Hue_PickUpBall, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_BallPickUp_Shadow = mat;

		mat = new Material(_instance._source_PickUpOuter);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(Static_ColorConfigs._Hue_PickUpBall, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_PickUpOuter_Shadow = mat;

		mat = new Material(_instance._source_Brick);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(0, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_BrickShadow = mat;

		mat = new Material(_instance._source_AlphaQuad);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(0, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_BreakEffect_Shadow = mat;

		mat = new Material(_instance._source_BallTrail);
		mat.color = Static_ColorConfigs.GetColor_From_HCL(Static_ColorConfigs._Hue_Ball, Static_ColorConfigs._Chroma_Shadow, Static_ColorConfigs._Luma_Shadow);
		_Material_BallTrailShadow = mat;
	}
}
