using Godot;
using System;

public partial class CameraFollowSwitch : Node2D
{
	[Export] Camera2D.Follow EnterFollow;
	[Export] Camera2D.Follow ExitFollow;

	[Export] float y_AxisFollowValue;
	[Export] float x_AxisFollowValue;

    private void OnBodyEnterred(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			Camera2D.camera.IsSwitching = true;
			Camera2D.camera.CameraFollow = EnterFollow;

			Camera2D.camera.x_axisRestraint = x_AxisFollowValue;
			Camera2D.camera.y_axisRestraint = y_AxisFollowValue;
		}
	}
}
