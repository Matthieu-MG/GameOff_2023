using Godot;
using System;

public partial class SnowShockwave : CharacterBody2D
{
	public bool Play = false;
	Vector2 OriginalPosition;
	[Export] float ShockwaveSpeed = 60;
	[Export] bool IsRight = true;
	float direction = 1;

	[Export] Area2D Hitbox;
	[Export] Sprite2D sprite;

    public override void _Ready()
    {
        OriginalPosition = Position;
		SetHitboxProperties(false);
    }

    public override void _PhysicsProcess(double delta)
    {
		// If SnowShockwave is Playing
        if(Play)
		{
			// Enable Hitbox and Visibility
			SetHitboxProperties(true);
			// Update the direction of Shockwave movement depending on where Wendigo is facing
			UpdateDirection();

			// Moves Shockwave
			Vector2 vel = Velocity;
			vel.X += ShockwaveSpeed * direction;

			if(IsOnWall())
			{
				// Disable Shockwave Hitbox and Visibility
				SetHitboxProperties(false);
				// Resets Position and Play variables values
				Position = OriginalPosition;
				Play = false;
			}

			Velocity = vel;
			MoveAndSlide();
		}
    }

	// Sets Hitbox and Sprite properties for the Shockwave scene
	private void SetHitboxProperties(bool state)
	{
		Hitbox.Visible = state;
		Hitbox.Monitoring = state;
		Hitbox.Monitorable = state;
		sprite.Visible = state;
	}

	// Sets direction based on where the Wendigo is Facing
	// And whether Shockwave is on the right or left of the Wendigo
	private void UpdateDirection()
	{
		if(IsRight)
			direction = WendigoFSM.wendigoFSM.GetFacingDirection();
		else
			direction = WendigoFSM.wendigoFSM.GetFacingDirection()*-1;
	}
}
