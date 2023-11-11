using Godot;
using System;

public partial class DashGhost : Sprite2D
{
	Tween _tween;

	// Creates and plays tween whose opacity goes from 1 to 0 in 1 second
    public override void _Ready()
    {
		_tween = CreateTween();
		_tween.TweenProperty(this, "self_modulate", new Color(1, 1, 1, 0), 1).SetEase(Tween.EaseType.Out)
		.SetTrans(Tween.TransitionType.Quart);
		_tween.Play();
    }

    public override void _Process(double delta)
    {
		// When tween has finished playing, free it from memory
        if (!_tween.IsRunning())
		{
			QueueFree();
		}
    }
}
