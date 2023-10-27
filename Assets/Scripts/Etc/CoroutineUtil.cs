using UnityEngine;
using System.Collections;

public class CoroutineUtil : MonoBehaviour
{
	public static IEnumerator TimeThread(float duration, System.Action<float> frameCallback, System.Action endCallback)
	{
		float elapsedTime = 0;
		while(elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += Time.deltaTime;
			
			if(frameCallback != null)
				frameCallback(elapsedTime / duration);
		}
		
		if(endCallback != null)
			endCallback();
	}

	public static IEnumerator RealTimeThread(float duration, System.Action<float> frameCallback, System.Action endCallback)
	{
		float elapsedTime = 0;
		while(elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += RealTime.deltaTime;
			
			if(frameCallback != null)
				frameCallback(elapsedTime / duration);
		}
		
		if(endCallback != null)
			endCallback();
	}
}
