using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class UserData
{
	static SaveData_Progress _progress;
	public static int _Score_Best
	{
		get
		{
			return _progress._score_Best;
		}
		set
		{
			_progress._score_Best = value;
			PlayerPrefs.SetString("_progress", JsonUtility.ToJson(_progress));
		}
	}

	static SaveData_Setting _setting;
	public static bool _IsTutorialCleared
	{
		get
		{
			return _setting._tutorialPlayed;
		}
		set
		{
			_setting._tutorialPlayed = value;
			PlayerPrefs.SetString("_setting", JsonUtility.ToJson(_setting));
		}
	}
	public static bool _OnSound
	{
		get
		{
			return _setting._onSound;
		}
		set
		{
			_setting._onSound = value;
			PlayerPrefs.SetString("_setting", JsonUtility.ToJson(_setting));
		}
	}
	public static bool _ShowRequesetRating
	{
		get
		{
			return _setting._showRequestRating;
		}
		set
		{
			_setting._showRequestRating = value;
			PlayerPrefs.SetString("_setting", JsonUtility.ToJson(_setting));
		}
	}
	public static bool _ShowContactPoint
	{
		get
		{
			return _setting._showContactPoint;
		}
		set
		{
			_setting._showContactPoint = value;
			PlayerPrefs.SetString("_setting", JsonUtility.ToJson(_setting));
		}
	}
	public static bool _ShowContactPoint_Selected
	{
		get
		{
			return _setting._showContactPoint_Selected;
		}
		set
		{
			_setting._showContactPoint_Selected = value;
			PlayerPrefs.SetString("_setting", JsonUtility.ToJson(_setting));
		}
	}
	public static bool _OnDarkMode
	{
		get
		{
			return _setting._onDarkMode;
		}
		set
		{
			_setting._onDarkMode = value;
			PlayerPrefs.SetString("_setting", JsonUtility.ToJson(_setting));
		}
	}

	static SaveData_PromoCondition _promoCondition;
	public static int _PlayCount
	{
		get
		{
			return _promoCondition._playCount;
		}
		set
		{
			_promoCondition._playCount = value;
			PlayerPrefs.SetString("_promoCondition", JsonUtility.ToJson(_promoCondition));
		}
	}
	public static bool _IsRated
	{
		get
		{
			return _promoCondition._isRated;
		}
		set
		{
			_promoCondition._isRated = value;
			PlayerPrefs.SetString("_promoCondition", JsonUtility.ToJson(_promoCondition));
		}
	}
	public static bool _IsFacebookVisited
	{
		get
		{
			return _promoCondition._isFacebookVisited;
		}
		set
		{
			_promoCondition._isFacebookVisited = value;
			PlayerPrefs.SetString("_promoCondition", JsonUtility.ToJson(_promoCondition));
		}
	}
	public static System.DateTime _ExpiredTime_RequestRate
	{
		get
		{
			return _promoCondition._expiredTime_RequestRate;
		}
		set
		{
			_promoCondition._expiredTime_RequestRate = value;
			PlayerPrefs.SetString("_promoCondition", JsonUtility.ToJson(_promoCondition));
		}
	}
	public static System.DateTime _ExpiredTime_RequestFacebookVisit
	{
		get
		{
			return _promoCondition._expiredTime_RequestFacebookVisit;
		}
		set
		{
			_promoCondition._expiredTime_RequestFacebookVisit = value;
			PlayerPrefs.SetString("_promoCondition", JsonUtility.ToJson(_promoCondition));
		}
	}

	static SaveData_GameStatus _gameStatus;
	public static GameStatus _GameStatus
	{
		get
		{
			return _gameStatus._status;
		}
		set
		{
			_gameStatus._status = value;
			PlayerPrefs.SetString("_gameStatus", JsonUtility.ToJson(_gameStatus));
		}
	}

	static SaveData_AC _ac;
	public static float _AC_ElapsedTime
	{
		get
		{
			return _ac._elapsedTime;
		}
		set
		{
			_ac._elapsedTime = value;
			PlayerPrefs.SetString("_ac", JsonUtility.ToJson(_ac));
		}
	}

	public static void Initialize()
	{
		_progress = JsonUtility.FromJson<SaveData_Progress>(PlayerPrefs.GetString("_progress"));
        if (_progress == null)
        {
			_progress = new SaveData_Progress();
        }

		//--
		_setting = JsonUtility.FromJson<SaveData_Setting>(PlayerPrefs.GetString("_setting"));
		if (_setting == null)
		{
			_setting = new SaveData_Setting();
			_setting._showContactPoint_Selected = true;
		}

		_promoCondition = JsonUtility.FromJson<SaveData_PromoCondition>(PlayerPrefs.GetString("_promoCondition"));
		if (_promoCondition == null)
		{
			_promoCondition = new SaveData_PromoCondition();
		}

		_gameStatus = JsonUtility.FromJson<SaveData_GameStatus>(PlayerPrefs.GetString("_gameStatus"));
		if (_gameStatus == null)
		{
			_gameStatus = new SaveData_GameStatus();
		}

		_ac = JsonUtility.FromJson<SaveData_AC>(PlayerPrefs.GetString("_ac"));
		if (_ac == null)
		{
			_ac = new SaveData_AC();
		}
    }
}
