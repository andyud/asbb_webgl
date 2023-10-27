using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wrapped_Int
{
	int[] _dataArray;
	public int _Value
	{
		get
		{
			int value = 0;
			for(int i = 0; i < _dataArray.Length; i++)
			{
				if(i%2 == 0)
					value += _dataArray[i];
				else
					value -= _dataArray[i];
			}

			return value;
		}
		set
		{
			int gap = value - _Value;
			int index = GetRandomIndex();
			AddSlotData(gap, index);
		}
	}

	void AddSlotData(int addValue, int index)
	{
		if(index%2 == 0)
			_dataArray[index] += addValue;
		else
			_dataArray[index] -= addValue;
	}

	public Wrapped_Int(int slotCount)
	{
		_dataArray = new int[slotCount];
	}

	public void Reset()
	{
		System.Random random = new System.Random ();

		for (int i = 0; i < _dataArray.Length; i++)
			_dataArray [i] = random.Next (-100, 100);
		
		_Value = 0;
	}

	public void Add(int addValue)
	{
		int index = GetRandomIndex ();
		AddSlotData(addValue, index);
	}

	int GetRandomIndex()
	{
		return Random.Range(0, _dataArray.Length);
	}
}
