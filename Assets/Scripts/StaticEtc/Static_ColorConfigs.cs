using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ColorElement
{
	RED = 0,
	GREEN,
	BLUE,

	MAX,
	NONE,
}

public static class Static_ColorConfigs
{
	static Color _ILLUM_COMP = new Color(0.299f, 0.587f, 0.114f);

	public static Color _Color_DarkMode;
	public static Color _Color_PopupDarkMode;

	public static Color _Color_Background;
	public static Color _Color_ButtonFrame;
	public static Color _Color_ButtonBase;
	public static Color _Color_PopupBackGround;

	public static float _Hue_Ball;
	public static float _Hue_PickUpBall;

	public static float _Hue_Brick_Min;
	public static float _Hue_Brick_Max;
	public static float _Chroma_Brick_Min;
	public static float _Chroma_Brick_Max;
	public static float _Luma_Brick_Min;
	public static float _Luma_Brick_Max;
	public static float _Luma_Hit;

	public static float _Chroma_Shadow;
	public static float _Luma_Shadow;

	public static Color _Color_Ball;
	public static Color _Color_BallTrail;
	public static Color _Color_PickUp;
	public static Color _Color_BallGuideLine;

	public static Color _Color_PowerBall;
	public static Color _Color_PowerBallGuideLine;

	public static Color _Color_Brick_Min;
	public static Color _Color_Brick_Max;

	public static Color _Color_Ball_Shadow;
	public static Color _Color_PickUp_Shadow;
	public static Color _Color_Brick_Shadow;

	public static Color _Color_TextBlack { get; private set; }
	public static Color _Color_TextGray { get; private set; }
	public static Color _Color_TextWhite { get; private set; }
	public static Color _Color_TextRed { get; private set; }
	public static Color _Color_TextGreen { get; private set; }
	public static Color _Color_TextBlue { get; private set; }
	public static Color _Color_TextStrongRed { get; private set; }
	public static Color _Color_TextStrongGreen { get; private set; }
	public static Color _Color_TextStrongBlue { get; private set; }
	public static Color _Color_TextOrange { get; private set; }
	public static Color _Color_WarningRed { get; private set; }
	public static Color _Color_TextStrongNavy { get; private set; }

	public static float _Hue_Red { get; private set; }
	public static float _Hue_Green { get; private set; }
	public static float _Hue_Blue { get; private set; }

	public static Color _Color_FigureRed { get; private set; }
	public static Color _Color_FigureGreen { get; private set; }
	public static Color _Color_FigureBlue { get; private set; }
	public static Color _Color_JointRed { get; private set; }
	public static Color _Color_JointGreen { get; private set; }
	public static Color _Color_JointBlue { get; private set; }
	public static Color _Color_OppositeFigureRed { get; private set; }
	public static Color _Color_OppositeFigureGreen { get; private set; }
	public static Color _Color_OppositeFigureBlue { get; private set; }
	public static Color _Color_BGRed { get; private set; }
	public static Color _Color_BGGreen { get; private set; }
	public static Color _Color_BGBlue { get; private set; }

	public static Color _Color_FacebookBlue { get; private set; }

	public static void Initialize()
	{
		Initialize_GUIColor();
		Initialize_ColorElement();

		Initialize_Color();
	}

	public static void SetDarkMode(bool isDark)
	{
		Initialize_Color();
	}

	static void Initialize_Color()
	{
		_Color_DarkMode = Color.gray;
		_Color_PopupDarkMode = new Color(157f, 157f, 157f, 255f) / 255f;

		_Color_Background = UserData._OnDarkMode ? _Color_DarkMode : new Color(230f, 230f, 230f, 255f) / 255f;
		_Color_PopupBackGround = UserData._OnDarkMode ? _Color_PopupDarkMode : new Color(230f, 230f, 230f, 255f) / 255f;


		_Color_ButtonFrame = _Color_TextBlack;
		_Color_ButtonBase = _Color_Background;

		_Hue_Ball = 210f;
		_Hue_PickUpBall = 135f;

		_Luma_Hit = 0.8f;

		float ballChroma = 0.6f;
		float ballLuma = 0.6f;
		float pickUpLuma;

		if (UserData._OnDarkMode)
		{
			_Hue_Brick_Min = 380f;
			_Hue_Brick_Max = 355f;

			_Chroma_Brick_Min = 0.5f;
			_Chroma_Brick_Max = 0.8f;

			_Luma_Brick_Min = 0.9f;
			_Luma_Brick_Max = 0.8f;

			pickUpLuma = 0.9f;
		}
		else
		{
			_Hue_Brick_Min = 380f;
			_Hue_Brick_Max = 355f;

			_Chroma_Brick_Min = 0.5f;
			_Chroma_Brick_Max = 0.8f;

			_Luma_Brick_Min = 0.7f;
			_Luma_Brick_Max = 0.5f;

			pickUpLuma = 0.7f;
		}

		_Color_Ball = GetColor_From_HCL(_Hue_Ball, ballChroma, ballLuma);
		_Color_BallTrail = GetColor_From_HCL(_Hue_Ball, ballChroma, ballLuma);
		_Color_PickUp = GetColor_From_HCL(_Hue_PickUpBall, ballChroma, pickUpLuma);
		_Color_Brick_Min = GetColor_From_HCL(_Hue_Brick_Min, _Chroma_Brick_Min, _Luma_Brick_Min);
		_Color_Brick_Max = GetColor_From_HCL(_Hue_Brick_Max, _Chroma_Brick_Max, _Luma_Brick_Max);

		_Color_BallGuideLine = _Color_Ball;
		_Color_BallGuideLine.a = 0.5f;

		_Color_PowerBall = _Color_Ball;
		_Color_PowerBallGuideLine = _Color_BallGuideLine;

		_Chroma_Shadow = 0.1f;
		_Luma_Shadow = 0.8f;

		_Color_Ball_Shadow = GetColor_From_HCL(_Hue_Ball, _Chroma_Shadow, _Luma_Shadow);
		_Color_PickUp_Shadow = GetColor_From_HCL(_Hue_PickUpBall, _Chroma_Shadow, _Luma_Shadow);
		_Color_Brick_Shadow = GetColor_From_HCL(_Hue_Brick_Max, _Chroma_Shadow, _Luma_Shadow);
	}

	public static float GetBrickHue(int hp, int currentTurn)
	{
		return Mathf.Lerp(Static_ColorConfigs._Hue_Brick_Min, Static_ColorConfigs._Hue_Brick_Max, CalculateColorLerpRatio(hp, currentTurn));
	}

	public static float GetBrickChroma(int hp, int currentTurn)
	{
		return Mathf.Lerp(Static_ColorConfigs._Chroma_Brick_Min, Static_ColorConfigs._Chroma_Brick_Max, CalculateColorLerpRatio(hp, currentTurn));
	}

	public static float GetBrickLuma(int hp, int currentTurn)
	{
		return Mathf.Lerp(Static_ColorConfigs._Luma_Brick_Min, Static_ColorConfigs._Luma_Brick_Max, CalculateColorLerpRatio(hp, currentTurn));
	}

	static float CalculateColorLerpRatio(int hp, int currentTurn)
	{
		float lerpRatio = 1;
		if (currentTurn > 1)
			lerpRatio = (float)(hp - 1) / (float)(currentTurn - 1);

		return lerpRatio;
	}

	static void Initialize_GUIColor()
	{
		_Color_TextBlack = new Color(16f, 16f, 16f, 255f) / 255f;
		_Color_TextGray = new Color(0.5f, 0.5f, 0.5f, 1);
		_Color_TextWhite = new Color(247f, 247f, 247f, 255f) / 255f;

		Color red = new Color(0.95f, 0.05f, 0.05f, 1f);
		Color green = new Color(0.05f, 0.95f, 0.05f, 1f);
		Color blue = new Color(0.05f, 0.05f, 0.95f, 1f);
		Color orange = new Color(0.95f, 0.5f, 0.05f, 1f);

		float targetIllumination = 0.6f;
		_Color_TextRed = ModifyIllumination(red, targetIllumination);
		_Color_TextGreen = ModifyIllumination(green, targetIllumination);
		_Color_TextBlue = ModifyIllumination(blue, targetIllumination);
		_Color_TextOrange = ModifyIllumination(orange, targetIllumination);

		float targetChroma = 0.5f;
		targetIllumination = 0.5f;
		_Color_TextStrongRed = GetColor_From_HCL(0, targetChroma, targetIllumination);
		_Color_TextStrongGreen = GetColor_From_HCL(120, targetChroma, targetIllumination);
		_Color_TextStrongBlue = GetColor_From_HCL(240, targetChroma, targetIllumination);
		_Color_TextStrongNavy = GetColor_From_HCL(225f, .223f, .215f);

		_Color_WarningRed = new Color(1, 0, 0, 1);

		_Color_FacebookBlue = new Color(59f, 87f, 157f, 255f) / 255f;
	}

	static void Initialize_ColorElement()
	{
		float hueModifier = 0;
		_Hue_Red = (0f + hueModifier) % 360f;
		_Hue_Green = (120f + hueModifier) % 360f;
		_Hue_Blue = (240f + hueModifier) % 360f;

		float targetIllumination = 0.7f;
		float targetChroma = 0.9f;
		_Color_FigureRed = GetColor_From_HCL(_Hue_Red, targetChroma, targetIllumination);
		_Color_FigureGreen = GetColor_From_HCL(_Hue_Green, targetChroma, targetIllumination);
		_Color_FigureBlue = GetColor_From_HCL(_Hue_Blue, targetChroma, targetIllumination);

		targetIllumination = 0.9f;
		targetChroma = 0.1f;
		_Color_JointRed = GetColor_From_HCL(_Hue_Red, targetChroma, targetIllumination);
		_Color_JointGreen = GetColor_From_HCL(_Hue_Green, targetChroma, targetIllumination);
		_Color_JointBlue = GetColor_From_HCL(_Hue_Blue, targetChroma, targetIllumination);

		targetIllumination = 0.2f;
		targetChroma = 0.4f;
		_Color_OppositeFigureRed = GetColor_From_HCL(_Hue_Red + 180, targetChroma, targetIllumination);
		_Color_OppositeFigureGreen = GetColor_From_HCL(_Hue_Green + 180, targetChroma, targetIllumination);
		_Color_OppositeFigureBlue = GetColor_From_HCL(_Hue_Blue + 180, targetChroma, targetIllumination);

		targetIllumination = 0.1f;
		targetChroma = 0.1f;
		_Color_BGRed = GetColor_From_HCL(_Hue_Red, targetChroma, targetIllumination);
		_Color_BGGreen = GetColor_From_HCL(_Hue_Green, targetChroma, targetIllumination);
		_Color_BGBlue = GetColor_From_HCL(_Hue_Blue, targetChroma, targetIllumination);
	}

	public static float GetFigureHue(ColorElement element)
	{
		if (element == ColorElement.RED)
			return _Hue_Red;
		else if (element == ColorElement.GREEN)
			return _Hue_Green;
		else if (element == ColorElement.BLUE)
			return _Hue_Blue;
		else
			return 0;
	}

	public static Color GetFigureColor(ColorElement element)
	{
		if (element == ColorElement.RED)
			return _Color_FigureRed;
		else if (element == ColorElement.GREEN)
			return _Color_FigureGreen;
		else if (element == ColorElement.BLUE)
			return _Color_FigureBlue;
		else
			return Color.white;
	}

	public static Color GetJointColor(ColorElement element)
	{
		if (element == ColorElement.RED)
			return _Color_JointRed;
		else if (element == ColorElement.GREEN)
			return _Color_JointGreen;
		else if (element == ColorElement.BLUE)
			return _Color_JointBlue;
		else
			return Color.white;
	}

	public static Color GetOppositeColor(ColorElement element)
	{
		if (element == ColorElement.RED)
			return _Color_OppositeFigureRed;
		else if (element == ColorElement.GREEN)
			return _Color_OppositeFigureGreen;
		else if (element == ColorElement.BLUE)
			return _Color_OppositeFigureBlue;
		else
			return Color.white;
	}

	public static Color GetBGColor(ColorElement element)
	{
		if (element == ColorElement.RED)
			return _Color_BGRed;
		else if (element == ColorElement.GREEN)
			return _Color_BGGreen;
		else if (element == ColorElement.BLUE)
			return _Color_BGBlue;
		else
			return Color.white;
	}

	public static Color GetColor_From_HSV(float hue, float saturate, float value)
	{
		float chroma = value * saturate;
		Color color = GetColorPosition_From_HueAndChroma(hue, chroma);

		float modifier = value - chroma;
		color.r += modifier;
		color.g += modifier;
		color.b += modifier;

		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);

		return color;
	}

	public static Color GetColor_From_HSL(float hue, float saturate, float lightness)
	{
		float chroma = (1 - Mathf.Abs(2 * lightness - 1)) * saturate;
		Color color = GetColorPosition_From_HueAndChroma(hue, chroma);

		float modifier = lightness - 0.5f * chroma;
		color.r += modifier;
		color.g += modifier;
		color.b += modifier;

		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);

		return color;
	}

	public static Color GetColor_From_HCL(float hue, float chroma, float illum)
	{
		Color color = GetColorPosition_From_HueAndChroma(hue, chroma);

		float currIllum = color.r * _ILLUM_COMP.r + color.g * _ILLUM_COMP.g + color.b * _ILLUM_COMP.b;
		float modifier = illum - currIllum;

		color.r += modifier;
		color.g += modifier;
		color.b += modifier;

		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);

		return color;
	}

	public static Color GetColorPosition_From_HueAndChroma(float hue, float chroma)
	{
		float h = (hue / 60f) % 6;
		float x = (1 - Mathf.Abs(h % 2 - 1));

		Color color = Color.black;

		if (0 <= h && h < 1)
			color = new Color(1, x, 0);
		else if (1 <= h && h < 2)
			color = new Color(x, 1, 0);
		else if (2 <= h && h < 3)
			color = new Color(0, 1, x);
		else if (3 <= h && h < 4)
			color = new Color(0, x, 1);
		else if (4 <= h && h < 5)
			color = new Color(x, 0, 1);
		else if (5 <= h && h < 6)
			color = new Color(1, 0, x);
		else
			Debug.LogWarning("Undefined Hue angle.");

		color *= chroma;
		color.a = 1;

		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);

		return color;
	}

	public static Color ModifyIllumination(Color color, float targetIllum)
	{
		float curIllum = color.r * _ILLUM_COMP.r + color.g * _ILLUM_COMP.g + color.b * _ILLUM_COMP.b;
		float modifier = targetIllum - curIllum;

		color.r += modifier;
		color.g += modifier;
		color.b += modifier;

		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);

		return color;
	}
}
