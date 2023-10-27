using UnityEngine;
using System.Collections;

public class GameState_Moving : GameState
{
	InGameController _gameInstance;
	
	public GameState_Moving(InGameController game) : base(game)
	{
		_gameInstance = game as InGameController;
	}

	public override void Reset ()
	{
//		#if UNITY_EDITOR
//		Debug.LogWarning("GameState : Moving");
//		#endif

		TouchInputRecognizer._AnnounceTouch_Start = DelegateResponse_InputStart;
		TouchInputRecognizer._AnnounceTouch_Stay = null;
		TouchInputRecognizer._AnnounceTouch_End = null;
		TouchInputRecognizer._AnnounceTouch_Cancel = null;
	}

	void DelegateResponse_InputStart(Swipe swipe)
	{
		Vector3 position = Camera.main.ScreenToWorldPoint (swipe._StartPosition);
		position.z = 0;

		_gameInstance._shooter.TryToSkip (position);
	}

	public override void FixedMoveNext ()
	{
		_gameInstance._shooter.FixedMoveNext ();
	}
	
	public override void MoveNext ()
	{
		//TouchInputRecognizer.Recognize ();

		_gameInstance._shooter.MoveNext ();
		_gameInstance._generator.MoveNext ();
	}
}