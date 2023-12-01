using Godot;
using System;
using System.Collections.Generic;

public partial class WendigoVerticalDashState : State
{
	Vector2 DashEndPoint;
	CharacterBody2D wendigo;
	[Export] float dashForce = 1000;

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		// If wendigo is null Get Character Body node
		wendigo ??= WendigoFSM.wendigoFSM.GetWendigoCharacterBody2D();
		
		if(wendigo.IsOnFloorOnly())
		{
			// Calculates and Assigns Position that Wendigo needs to lerp/move to
			DashEndPoint = wendigo.Position;
			DashEndPoint.Y -= dashForce;
		}
		else
		{
			// Transitions to Idle State if Wendigo is not grounded
			WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.IDLE);
		}
    }

    public override void Update(double delta)
    {
		// Dashes
		WendigoFSM.LerpToPoint(wendigo, DashEndPoint, 0.2f, false, WendigoFSM.LerpModes.Y_Only);

		// If Wendigo body reached Dash Point, Transition to Idle
		float distance = Mathf.Abs(wendigo.Position.Y - DashEndPoint.Y);
		if(distance < 0.1f)
		{
			WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.IDLE);
		}
    }
}
