using UnityEngine;
using System.Collections;

public static class Static_Calculator_UniformlyAcceleration
{
	public static void Calculate_InitialSpeed_N_Duration(float moveDistance, float acceleration, out float initialSpeed, out float duration)
	{
		initialSpeed = Mathf.Sqrt (2 * moveDistance * acceleration);
		duration = initialSpeed / acceleration;
	}

	public static void Calculate_Acceleration_N_InitialSpeed(float moveDistance, float duration, out float acceleration, out float initialSpeed)
	{
		initialSpeed = 2 * moveDistance / duration;
		acceleration = initialSpeed / duration;
	}

	public static void Calculate_Acceleration_N_MoveDistance(float initialSpeed, float duration, out float acceleration, out float moveDistance)
	{
		acceleration = initialSpeed / duration;
		moveDistance = initialSpeed * duration / 2;
	}
}
