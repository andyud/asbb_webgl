using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SaveData
{

}

[Serializable]
public class SaveData_Progress : SaveData
{
	public int _score_Best;

	public SaveData_Progress()
	{
		_score_Best = 0;
	}
}

[Serializable]
public class SaveData_Setting : SaveData
{
	public bool _tutorialPlayed;
	public bool _onSound;
	public bool _showRequestRating;
	public bool _showContactPoint;
	public bool _showContactPoint_Selected;
	public bool _onDarkMode;

	public SaveData_Setting()
	{
		_tutorialPlayed = false;
		_onSound = true;
		_showRequestRating = true;
		_showContactPoint = true;
		_showContactPoint_Selected = false;
		_onDarkMode = false;
	}
}

[Serializable]
public class SaveData_PromoCondition : SaveData
{
	public int _playCount;
	public bool _isRated;
	public bool _isFacebookVisited;
	public System.DateTime _expiredTime_RequestRate;
	public System.DateTime _expiredTime_RequestFacebookVisit;
	
	public SaveData_PromoCondition()
	{
		_playCount = 0;
		_isRated = false;
		_isFacebookVisited = false;
		_expiredTime_RequestRate = DateTime.Now;
		_expiredTime_RequestFacebookVisit = DateTime.Now;
	}
}

[Serializable]
public class SaveData_GameStatus : SaveData
{
	public GameStatus _status;

	public SaveData_GameStatus()
	{
		_status = null;
	}
}

[Serializable]
public class SaveData_AC : SaveData
{
	public float _elapsedTime;
	
	public SaveData_AC()
	{
		_elapsedTime = 0;
	}
}