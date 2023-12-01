using Godot;
using System;

public partial class Patrol : Node2D
{
	[Export] private Node2D[] positions;

	private int currentPositionIndex = 0;
	private Node2D currentPosition;

	public Vector2 direction = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentPosition = positions[0];
		direction = (currentPosition.GlobalPosition - GlobalPosition).Normalized();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GlobalPosition.DistanceTo(currentPosition.Position) < 150)
		{
			currentPositionIndex++;

			if (currentPositionIndex >= positions.Length)
			{
				currentPositionIndex = 0;
			}

			currentPosition = positions[currentPositionIndex];
			direction = (currentPosition.GlobalPosition - GlobalPosition).Normalized();
		}
	}
}
