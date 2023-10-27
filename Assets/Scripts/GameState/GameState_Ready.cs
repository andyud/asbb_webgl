using UnityEngine;
using System.Collections;

public class GameState_Ready : GameState
{
	InGameController _gameInstance;

	public GameState_Ready(InGameController game) : base(game)
	{
		_gameInstance = game as InGameController;
	}

	public override void Reset ()
	{
		_gameInstance._stage.Reset ();
		
		_gameInstance._cameras.SetStageWidth (InGameController._CurrentStageWidth);
		_gameInstance._stage.SetStageWidth (InGameController._CurrentStageWidth);
		_gameInstance._shooter.SetStageWidth(InGameController._CurrentStageWidth);
		
		_gameInstance._generator.Reset ();
		_gameInstance._shooter.Reset ();
	}

	public override void FixedMoveNext ()
	{

	}

	public override void MoveNext()
	{

	}
}
