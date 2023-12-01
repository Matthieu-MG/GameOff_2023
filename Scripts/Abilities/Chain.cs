using Godot;
using System;
using System.Threading.Tasks;

public partial class Chain : Sprite2D
{
	[Export] RayCast2D rayCast2D;
	[Export] const float MAX_DISTANCE = 1500;

	[Signal] public delegate void HookedEventHandler(float HookedPosition);

	// Used to Interpolate the Offset and Rect of the Chain Sprite Node
	private void Interpolate(float length, float duration = 0.2f)
	{
		Tween TweenOffset = CreateTween();
		Tween TweenRectH = CreateTween();

		TweenOffset.TweenProperty(this, "offset", new Vector2(0, length/2.0f), duration);
		TweenRectH.TweenProperty(this, "region_rect", new Rect2(0, 0, 125, length), duration);
	}

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("Hook"))
		{
			HookAnim(700, 0.6f);
		}
    }

	// Creates the Hook Animation
	private async void HookAnim(float length, float duration = 0.2f)
	{
		Interpolate(CheckCollision(), duration);
		await ToSignal(GetTree().CreateTimer(duration), "timeout");
		ReverseHook();
	}

	// Retracts Chain
	private void ReverseHook()
	{
		Interpolate(0, 0.75f);
	}

	// Checks for collision of Raycast of Chain
	private float CheckCollision()
	{
		Vector2 collisionPoint;
		float distance;

		// Might remove below line
		Task.Run( async () => await ToSignal(GetTree().CreateTimer(0.1f), "timeout"));

		if (rayCast2D.IsColliding())
		{
			collisionPoint = rayCast2D.GetCollisionPoint();
			distance = (GlobalPosition - collisionPoint).Length();
			EmitSignal("Hooked", collisionPoint);
		}
		else
		{
			distance = MAX_DISTANCE;
		}
		return distance;
	}
}
