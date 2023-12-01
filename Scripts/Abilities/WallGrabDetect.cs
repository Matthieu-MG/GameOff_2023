using Godot;
using System;

public partial class WallGrabDetect : Area2D
{
	Godot.Collections.Array<Node2D> bodies;
	bool FoundTiles = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bodies = GetOverlappingBodies();
		foreach(Node2D body in bodies)
		{
			if(body.IsInGroup("Tiles"))
			{
				PlayerMovement.player.IsWalled = true;
				FoundTiles = true;
			}
		}
		if (!FoundTiles)
			PlayerMovement.player.IsWalled = false;
	}
}
