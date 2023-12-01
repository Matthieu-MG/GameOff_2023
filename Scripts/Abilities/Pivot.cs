using Godot;
using System;

public partial class Pivot : Node2D
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Hook"))
		{
			LookAt(GetGlobalMousePosition());
		}
    }
}
