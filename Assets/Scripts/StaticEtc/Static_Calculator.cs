using UnityEngine;
using System.Collections;

public static class Static_Calculator
{
	public static float GetScreenHeightRatio()
	{
		if(GetScreenWidthRatio() > 0.75f)
			return 20f / 9f;
		else
			return (float)Screen.height / (float)Screen.width;
	}

	public static float GetScreenWidthRatio()
	{
		return (float)Screen.width / (float)Screen.height;
	}

	public static Vector2 Vector2XYForce(float angle, float power)
	{
		float forceForward = power * Mathf.Cos(angle*Mathf.Deg2Rad);
		float forceUp = power * Mathf.Sin(angle*Mathf.Deg2Rad);
		
		Vector2 v = Vector2.right * forceForward + Vector2.up * forceUp;
		
		return v;
	}

	public static float XYMeter2Angle(Vector2 xy)
	{
		float angle = Mathf.Atan2(xy.y, xy.x) * Mathf.Rad2Deg;		
		return angle;
	}

	public static float XYMeter2Angle(Vector3 xyz)
	{
		float angle = Mathf.Atan2(xyz.y, xyz.x) * Mathf.Rad2Deg;		
		return angle;
	}

	public static float XYMeter2Power(Vector2 xy)
	{
		float power = Mathf.Pow((Mathf.Pow(xy.x, 2.0f) + Mathf.Pow(xy.y, 2.0f)), 0.5f);		
		return power;
	}

	public static float XYMeter2Power(Vector3 xyz)
	{
		float power = Mathf.Pow((Mathf.Pow(xyz.x, 2.0f) + Mathf.Pow(xyz.y, 2.0f)), 0.5f);		
		return power;
	}

	public static Vector3 Vector2To3(Vector2 v)
	{
		return Vector3.right * v.x + Vector3.up * v.y;
	}

	public static Vector2 Vector3To2(Vector3 v)
	{
		return Vector2.right * v.x + Vector2.up * v.y;
	}
}
