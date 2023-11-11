using Godot;
using System;

public partial class EnemyJumpSideways : Node2D
{
	CharacterBody2D enemy;
	[Export] static float JumpForceY = 600;
	[Export] static float JumpForceX = 200;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemy = (CharacterBody2D) GetParent();
	}

	public static void EnemyJump(ref Vector2 enemyVel)
	{
		enemyVel.Y = -JumpForceY;
		enemyVel.X = JumpForceX;
	}
}
