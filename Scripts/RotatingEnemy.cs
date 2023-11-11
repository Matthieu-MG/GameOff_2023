using Godot;
using System;
using System.Runtime;

public partial class RotatingEnemy : CharacterBody2D
{
	[Export] float RotationSpeed = 2.0f;
	[Export] float MoveSpeed = 100;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateToTarget(PlayerMovement.player, delta);
	}

    public override void _PhysicsProcess(double delta)
    {
		Vector2 vel = Velocity;
		MoveToPlayer(ref vel);
		Velocity = vel;
        MoveAndSlide();
    }

    private void RotateToTarget(CharacterBody2D target, double delta)
	{
		Vector2 direction = target.GlobalPosition - GlobalPosition;
		float AngleTo = Transform.X.AngleTo(direction);
		Rotate(Mathf.Sign(AngleTo) * Mathf.Min((float) delta * RotationSpeed, Mathf.Abs(AngleTo)));
	}

	private void MoveToPlayer(ref Vector2 vel)
	{
        vel = (PlayerMovement.player.GlobalPosition.Normalized() - GlobalPosition.Normalized()) * MoveSpeed;
	}
}
