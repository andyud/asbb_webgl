using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameStatus
{
	public int _ballCount;
	public float _ballPositionX;
	public float _ballPositionY;
	public bool _shootStarted;
	public float _shootAngle;
	public int _currentTurn;
	public int _currentScore;
	public bool _isGameOver;
	public List<BrickData> _brickData_List = new List<BrickData>();
	public List<PickUpData> _pickUpData_List = new List<PickUpData>();

	public void Reset()
	{
		_ballCount = 1;
		_ballPositionX = 0;
		_ballPositionY = 0;
		_shootStarted = false;
		_shootAngle = 0;
		_currentTurn = 0;
		_currentScore = 1;
		_brickData_List.Clear ();
		_pickUpData_List.Clear ();
	}

	public void UpdateStatus(GameStatus status)
	{
		_ballCount = status._ballCount;
		_ballPositionX = status._ballPositionX;
		_ballPositionY = status._ballPositionY;
		_shootStarted = status._shootStarted;
		_shootAngle = status._shootAngle;
		_currentTurn = status._currentTurn;
		_currentScore = status._currentScore;
		_brickData_List = status._brickData_List;
		_pickUpData_List = status._pickUpData_List;
	}
}

[System.Serializable]
public class BrickData
{
	public int _locationX;
	public int _locationY;
	public int _hp;
}

[System.Serializable]
public class PickUpData
{
	public int _locationX;
	public int _locationY;
}