using Godot;
using System;
using System.Threading.Tasks;

public partial class TestHookPlayer : CharacterBody2D
{
	private void OnChainHook(Vector2 HookedPosition)
	{
		GD.Print("Signal Received");

		Task.Run( async () => await ToSignal(GetTree().CreateTimer(0.2f), "timeout"));

		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", HookedPosition, 0.75);
	}
}
