using UnityEngine;
using System.Collections;

public class GameState_Wait : GameState
{
	InGameController _gameInstance;

	bool _isGeneratorReady = false;
	bool _isShooterReady = false;


	public GameState_Wait(InGameController game) : base(game)
	{
		_gameInstance = game as InGameController;
	}

	public override void Reset ()
	{
//		#if UNITY_EDITOR
//		Debug.LogWarning("GameState : Wait");
//		#endif

		_isGeneratorReady = false;
		_isShooterReady = false;

		_gameInstance._guideLine.SetOff ();

		TouchInputRecognizer._AnnounceTouch_Start = null;
		TouchInputRecognizer._AnnounceTouch_Stay = null;
		TouchInputRecognizer._AnnounceTouch_End = null;

		InGameController._CurrentTurn++;
        if (Main.isAdSkip)
        {
            Main.isAdSkip = false;            
            InGameController._CurrentTurn = (UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn;
        }

        _gameInstance._generator.SetNewTurn (Callback_GeneratorReady);
		
		_gameInstance._shooter.SetNewTurn ();
		if(InGameController._CurrentTurn > 1)
			_gameInstance._shooter.AddBall (Callback_ShooterReady);
		else
		{
			InGameController.SetScore (1);
			Callback_ShooterReady();
		}
	}

	void Callback_GeneratorReady()
	{
		InGameController.SetScore(InGameController._CurrentTurn);
		_isGeneratorReady = true;
		if(_isShooterReady)
			SetNewTurnReady();
	}

	void Callback_ShooterReady()
	{
		_isShooterReady = true;
		if(_isGeneratorReady)
			SetNewTurnReady();
	}

	void SetNewTurnReady()
	{
		//InGameController.Save_TurnEnd ();
		if(_gameInstance._generator._IsGameOver)
		{
			// a/b 테스트 항목 A: 기존 플레이 B: 부활기능 적용
			//if(FireBaseController.ConfigData != null && FireBaseController.ConfigData.IsReviveFeature)
			{
				ReviveController.Instance.CheckRevive((isRevive) =>
				{
					if (isRevive)
					{
						TouchInputRecognizer._AnnounceTouch_Stay = DelegateResponse_InputStay;
						TouchInputRecognizer._AnnounceTouch_End = DelegateResponse_InputEnd;
						TouchInputRecognizer._AnnounceTouch_Cancel = DelegateResponse_InputCancel;
						_gameInstance._generator._IsGameOver = false;
					}
					else
					{
						_gameInstance.Callback_BrickDownToLimit();
					}

					InGameController.Save_TurnEnd();
				});
			}
			//else
			//{
			//	InGameController.Save_TurnEnd();
			//	_gameInstance.Callback_BrickDownToLimit();
			//}
		}
		else
		{
			InGameController.Save_TurnEnd();

			//			TouchInputRecognizer._AnnounceTouch_Start = DelegateResponse_InputStart;
			TouchInputRecognizer._AnnounceTouch_Stay = DelegateResponse_InputStay;
			TouchInputRecognizer._AnnounceTouch_End = DelegateResponse_InputEnd;
			TouchInputRecognizer._AnnounceTouch_Cancel = DelegateResponse_InputCancel;
		}
	}

/*	void DelegateResponse_InputStart(Swipe swipe)
	{
		Vector3 startPosition = Camera.main.ScreenToWorldPoint (swipe._StartPosition);
		Vector3 currentPosition = Camera.main.ScreenToWorldPoint (swipe._StartPosition);
		Vector3 ballPosition = _gameInstance._shooter._turnStartPosition;
		startPosition.z = 0;
		currentPosition.z = 0;
		ballPosition.z = 0;



		float positionLimit_Bottom = 0 - InGameController._CurrentStageWidth * 0.0125f - InGameController._BallRadius * 4;
		float positionLimit_Top = InGameController._CurrentStageWidth;
		if (startPosition.y <= positionLimit_Bottom || positionLimit_Top <= startPosition.y )
			return;
		
		//		if(Vector3.Distance(startPosition, ballPosition) < InGameController._BallSelectCriteria)
		startPosition = ballPosition;
		
		Vector3 dir = currentPosition - startPosition;
		if(dir.magnitude * 20f > InGameController._CurrentStageWidth)
			_gameInstance._guideLine.SetShootInfo (startPosition, currentPosition, ballPosition);
	}
*/
	void DelegateResponse_InputStay(Swipe swipe)
	{
		Vector3 startPosition = Camera.main.ScreenToWorldPoint (swipe._StartPosition);
		Vector3 currentPosition = Camera.main.ScreenToWorldPoint (swipe._CurrentPosition);
		Vector3 ballPosition = _gameInstance._shooter._turnStartPosition;
		startPosition.z = 0;
		currentPosition.z = 0;
		ballPosition.z = 0;

		if (isAvailableInput (startPosition, currentPosition, ballPosition) == false)
			return;

		bool isSnaped = false;
		if(Vector3.Distance(startPosition, ballPosition) < InGameController._BallSelectCriteria)
		{
			isSnaped = true;
			startPosition = ballPosition;
		}

		Vector3 dir = currentPosition - startPosition;
		if(dir.magnitude * 20f > InGameController._CurrentStageWidth)
			_gameInstance._guideLine.SetShootInfo (startPosition, currentPosition, ballPosition, isSnaped);
	}

	void DelegateResponse_InputEnd(Swipe swipe)
	{
		_gameInstance._guideLine.SetOff ();

		Vector3 startPosition = Camera.main.ScreenToWorldPoint (swipe._StartPosition);
		Vector3 currentPosition = Camera.main.ScreenToWorldPoint (swipe._EndPosition);
		Vector3 ballPosition = _gameInstance._shooter._turnStartPosition;
		startPosition.z = 0;
		currentPosition.z = 0;
		ballPosition.z = 0;

		if (isAvailableInput (startPosition, currentPosition, ballPosition) == false)
			return;

		if(Vector3.Distance(startPosition, ballPosition) < InGameController._BallSelectCriteria)
			startPosition = ballPosition;

		Vector3 dir = currentPosition - startPosition;
		float angle = (Static_Calculator.XYMeter2Angle (dir) + 360f) % 360f;
		if(dir.magnitude * 20f > InGameController._CurrentStageWidth)
		{
			if(!(0 <= angle && angle <= 180))
			{
				if(angle <= 270)
					angle = 180;
				else
					angle = 0;
			}

			InGameController.Save_TurnStart(angle);
			
			_gameInstance._shooter.Shoot (Static_Calculator.Vector2To3(Static_Calculator.Vector2XYForce(angle, 1)), _gameInstance.Callback_TurnEnd);
			_game.SetState (InGameController._state_Moving);
		}
	}

	bool isAvailableInput(Vector3 startPosition, Vector3 endPosition, Vector3 ballPosition)
	{
		if (Vector3.Distance (startPosition, endPosition) < InGameController._BallRadius)
			return false;

//		if (Vector3.Distance (startPosition, ballPosition) < InGameController._BallSelectCriteria)
//			return true;
//		else
//		{
//			float positionLimit_Bottom = 0 - InGameController._CurrentStageWidth * 0.0125f;
			float positionLimit_Bottom = ballPosition.y - InGameController._BallSelectCriteria;
			float positionLimit_Top = InGameController._CurrentStageWidth;
			if (positionLimit_Bottom <= startPosition.y && startPosition.y <= positionLimit_Top)
				return true;
			else
				return false;
//		}
	}

	void DelegateResponse_InputCancel(Swipe swipe)
	{
		_gameInstance._guideLine.SetOff ();
	}
	
	public override void FixedMoveNext ()
	{
		_gameInstance._shooter.FixedMoveNext ();
	}
	
	public override void MoveNext()
	{
		TouchInputRecognizer.Recognize ();

		_gameInstance._generator.MoveNext ();
		_gameInstance._shooter.MoveNext ();

//		DebugControl ();
	}

/*	void DebugControl()
	{
		if(Input.GetKeyDown(KeyCode.Menu))
		{
			Ball ball = ObjectPoolHolder.GetObject<Ball> (_gameInstance._shooter._transform);
			ball.Initialize (InGameController._BallRadius);
			_gameInstance._shooter._ball_List.Add (ball);			
			_gameInstance._shooter.AddBall(null);
			
			InGameController._CurrentTurn++;
		}
		
		#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Ball ball = ObjectPoolHolder.GetObject<Ball> (_gameInstance._shooter._transform);
			ball.Initialize (InGameController._BallRadius);
			_gameInstance._shooter._ball_List.Add (ball);
			_gameInstance._shooter.AddBall(null);
			
			InGameController._CurrentTurn++;
		}
		#endif
	}*/
}
