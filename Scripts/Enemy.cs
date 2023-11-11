using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] const float SPEED = 100;
	[Export] Patrol _patrol;
	[Export] AnimatedSprite2D _animSprite;
	bool FaceRight = true;

	Vector2 vel;
	float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public bool IsKnockedBack = false;
	public bool WasKnockedBack = false;

	[Export] EnemyType Type = EnemyType.Idle;
	enum EnemyType
	{
		Idle,
		Patrol,
		Jump
	}

    public override void _Process(double delta)
    {
        Flip();
    }
	

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
		vel = Velocity;

		vel.Y += _gravity;

		if (Type == EnemyType.Patrol)
		{
			_animSprite.Play("walking");
			vel.X = SPEED * _patrol.direction.X;
		}

		/*
		if (IsKnockedBack)
		{
			//vel.Y = -1500;
			vel.X = 100 * PlayerMovement.player.GetPlayerTransformScaleX();
			IsKnockedBack = false;
			WasKnockedBack = true;
		}
		else if (WasKnockedBack)
		{
			vel = Vector2.Zero;
			WasKnockedBack = false;
		}
		*/

		Velocity = vel;
		MoveAndSlide();
	}

	private void Flip()
	{
		if (FaceRight && vel.X < 0 || !FaceRight && vel.X > 0)
		{
			FaceRight = !FaceRight;
			Vector2 direction = Scale;
			direction.X *= -1;
			Scale = direction;
		}
	}
}
