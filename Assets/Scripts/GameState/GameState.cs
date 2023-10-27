using UnityEngine;
using System.Collections;

public abstract class GameState
{
	protected InGameController _game;

	public GameState(InGameController game)
	{
		_game = game;
	}

	public abstract void Reset();
	public abstract void FixedMoveNext();
	public abstract void MoveNext();
}
