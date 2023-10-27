using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameController : MonoBehaviour
{
	public static InGameController _instance;
	public class ScoreData
	{
		public Wrapped_Int _currentScore;
		public bool _isNewRecord;

		public ScoreData()
		{
			_currentScore = new Wrapped_Int(10);
			_isNewRecord = false;
		}

		public void SetWrapped()
		{
			_currentScore.Reset();
		}
	}
	ScoreData _data_Score;

	public bool _IsNewRecord { get { return _data_Score._isNewRecord; } set { _data_Score._isNewRecord = value; } }
	public int _Score_Current
	{
		get
		{
            if (_data_Score == null)
            {
				Reset();
            }
			return _data_Score._currentScore._Value;
		}
		set
		{
			int gap = value - _Score_Current;
			_data_Score._currentScore.Add(gap);
			if (_Score_Current > UserData._Score_Best)
				_IsNewRecord = true;

			if (_callback_ScoreChange != null)
				_callback_ScoreChange(_Score_Current, gap, _IsNewRecord);
		}
	}


	public CameraController _cameras;

	protected GameState _state;

	public void SetState(GameState state)
	{
		_state = state;
		if(_state!=null){
			_state.Reset();
		}
	}

	public GameState GetState()
	{
		return _state;
	}

	System.Action<int, int, bool> _callback_ScoreChange;

	public void Initialize(System.Action<int, int, bool> scoreChangeCallback)
	{
		Debug.Log(">>> InGameController: Initialize 1");
		_instance = this;
		_callback_ScoreChange = scoreChangeCallback;
		_cameras.Initialize();
		Debug.Log(">>> InGameController: Initialize 2");
		_state_Ready = new GameState_Ready(this);
		_state_Continue = new GameState_Continue(this);
		_state_Waiting = new GameState_Wait(this);
		_state_Moving = new GameState_Moving(this);
		_state_GameOver = new GameState_GameOver(this);
		Debug.Log(">>> InGameController: Initialize 3");
		_cameras = _cameras as CameraController;
		_stage.Initialize();

		_generator.Initialize();
		_shooter.Initialize();
		_guideLine.Initialize();
		Debug.Log(">>> InGameController: Initialize 4");
	}

	public void Reset()
	{
		_data_Score = new ScoreData();
		_data_Score.SetWrapped();

		_cameras.Reset();

		_status.Reset();
		SetState(_state_Ready);
	}

	public void StartTutorial()
	{
		SetState(_state_Waiting);
	}

	public void StartGame()
	{
		SetState(_state_Waiting);
	}

	public void RestartGame()
	{
		Reset();
		StartGame();
	}

	protected void GameOver()
	{
		SoundController.Play_GameOver();
		_stage.Direct_BreakLine();
		_shooter.Direct_GameOver();

		float duration = 1.0f;

		_cameras.Tilt(0.1f, 0.01f, duration, null);
		_cameras.SetGrayScale(0, 1, duration, Callback_GameOverDirectEnd);

		SetState(_state_GameOver);
	}


	public void MoveNext()
	{
		if (_state != null)
			_state.MoveNext();

		if (_thread_Director != null)
		{
			if (_thread_Director.MoveNext() == false)
				_thread_Director = null;
		}

		if (_stage != null)
			_stage.MoveNext();
	}

	public GameStatus _status;
	public static int _BallCount { get { return _instance._status._ballCount; } set { _instance._status._ballCount = value; } }
	public static float _BallPositionX { get { return _instance._status._ballPositionX; } set { _instance._status._ballPositionX = value; } }
	public static float _BallPositionY { get { return _instance._status._ballPositionY; } set { _instance._status._ballPositionY = value; } }
	public static Vector3 _BallPosition { get { return Vector3.right * _BallPositionX + Vector3.up * _BallPositionY; } set { _BallPositionX = value.x; _BallPositionY = value.y; } }
	public static bool _ShootStarted { get { return _instance._status._shootStarted; } set { _instance._status._shootStarted = value; } }
	public static float _ShootAngle { get { return _instance._status._shootAngle; } set { _instance._status._shootAngle = value; } }
	public static int _CurrentTurn { get { return _instance._status._currentTurn; } set { _instance._status._currentTurn = value; } }
	public static int _CurrentScore { get { return _instance._status._currentScore; } set { _instance._status._currentScore = value; } }
	public static List<BrickData> _BrickData_List { get { return _instance._status._brickData_List; } set { _instance._status._brickData_List = value; } }
	public static List<PickUpData> _PickUpData_List { get { return _instance._status._pickUpData_List; } set { _instance._status._pickUpData_List = value; } }

	public static void Save_TurnEnd()
	{
				_BallCount = _instance._shooter.OriginBallCount;
		//_BallCount = _instance._shooter._ball_List.Count;
		_BallPosition = _instance._shooter._turnStartPosition;
		_ShootStarted = false;
		_ShootAngle = 0;
		_CurrentScore = _instance._Score_Current;

		_BrickData_List.Clear();
		for (int i = 0; i < _instance._generator._brick_List.Count; i++)
			_BrickData_List.Add(_instance._generator._brick_List[i]._data);

		_PickUpData_List.Clear();
		for (int i = 0; i < _instance._generator._pickUp_List.Count; i++)
			_PickUpData_List.Add(_instance._generator._pickUp_List[i]._data);

		UserData._GameStatus = _instance._status;
	}

	public static void Save_TurnStart(float shootAngle)
	{
		_ShootStarted = true;
		_ShootAngle = shootAngle;

		UserData._GameStatus = _instance._status;
	}

	public static float _SecPerFrame { get { return 0.0166667f; } }
	public static int _BrickCount_Width { get { return 6; } }
	public static int _BrickCount_Height { get { return 9; } }
	public static int _CurrentStageWidth { get { return 18; } }
	public static float _BrickWidth { get { return (float)_CurrentStageWidth / _BrickCount_Width; } }
	public static float _BrickHeight { get { return (float)_CurrentStageWidth / _BrickCount_Height; } }

	public static Vector3 _PositionModifier_Shadow { get { return Vector3.right * 0.1f + Vector3.down * 0.2f; } }

	public static float _BallRadius { get { return _BrickWidth * 0.12f; } }
	public static float _BallSelectCriteria { get { return _BallRadius * 5; } }

	public static GameState _state_Ready;
	public static GameState_Continue _state_Continue;
	public static GameState _state_Waiting;
	public static GameState _state_Moving;
	public static GameState _state_GameOver;

	public Stage _stage;

	public BrickGenerator _generator;
	public Shooter _shooter;
	public ControlGuideLine _guideLine;

	IEnumerator _thread_Director;

	public static void SetScore(int ballCount)
	        {
			_instance._Score_Current = ballCount;
			}

	public void Callback_TurnEnd()
	{
		SetState(_state_Waiting);
	}

	public void Callback_BrickDownToLimit()
	{
		GameOver();
	}

	public static void Direct_Ready()
	{
		_instance._stage.Direct_CreateLine();
	}

	public void Continue(GameStatus status)
	{
		_status.UpdateStatus(status);
		SetState(_state_Continue);
	}

	public void SetDarkMode()
	{
		_cameras.SetDarkMode();
	}


	void Callback_GameOverDirectEnd()
	{
		Main.GameOver();
	}

	void FixedUpdate()
	{
		if (_state != null)
		_state.FixedMoveNext();
	}

	void DebugControl()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Time.timeScale *= 0.9f;
			Debug.LogWarning("TimeScale : " + Time.timeScale);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			Time.timeScale = 1f;
			Debug.LogWarning("TimeScale : " + Time.timeScale);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			Time.timeScale *= 1.1f;
			Debug.LogWarning("TimeScale : " + Time.timeScale);
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			Time.timeScale -= 0.1f;
			Debug.LogWarning("TimeScale : " + Time.timeScale);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Time.timeScale = 0f;
			Debug.LogWarning("TimeScale : " + Time.timeScale);
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			Time.timeScale += 0.1f;
			Debug.LogWarning("TimeScale : " + Time.timeScale);
		}
	}
}
