using UnityEngine;
using System.Collections;

public class GameState_GameOver : GameState
{
	InGameController _gameInstance;

	public GameState_GameOver(InGameController game) : base(game)
	{
		_gameInstance = game as InGameController;
	}

	public override void Reset ()
	{

	}

	public override void FixedMoveNext ()
	{
		
	}
	
	public override void MoveNext()
	{
		_gameInstance._shooter.MoveNext ();
		_gameInstance._generator.MoveNext ();
	}
}
