using Godot;
using System;
using System.Collections.Generic;

public partial class WendigoJumpState : State
{
	CharacterBody2D _Wendigo;

	/*
	[Export] double jumpHeight = 800;
	double jumpTimeToPeak = 0.32f;
	[Export] double jumpTimeToFall = 0.5f;

	double jumpVelocity;
	double jumpGravity;
	double fallGravity;
	*/

	[Export] float JumpForce = 30000;
	bool HasJumped;

    public override void _Ready()
    {
        // SetPhysicsProcess(false);
    }

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		// SetPhysicsProcess(true);
		HasJumped = false;
		WendigoFSM.wendigoFSM.animPlayer.Stop();
		/*
        jumpVelocity = ((2.0 * jumpHeight) / jumpTimeToPeak) * -1.0;
		jumpGravity = ((-2.0 * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0;
		fallGravity = ((-2.0 * jumpHeight) / (jumpTimeToFall * jumpTimeToFall)) * -1.0;
		*/
	}
    
    public override void PhysicsUpdate(double delta)
    {
        Vector2 vel = WendigoFSM.GetWendigoVel();
		// vel.Y += (float) (jumpGravity * delta);
		
		if (!HasJumped && WendigoFSM.wendigoFSM.wendigoBody.IsOnFloor())
		{
			vel.Y = -JumpForce;
			HasJumped = true;
		}
		
		else
		{
			vel.X = 0;
			vel.Y = 0;
			WaitBeforeTransition(1);
		}
		

		WendigoFSM.SetWendigoBodyX(vel.X);
		WendigoFSM.SetWendigoBodyY(vel.Y);
		WendigoFSM.CallMoveAndSlide();
	}

	/*
    private double GetGravity()
	{
		if (WendigoFSM.GetWendigoVel().Y < 0.0)
		{
			return jumpGravity;
		}

		return fallGravity;
	}
	*/
	private async void WaitBeforeTransition(float time)
	{
		await ToSignal(GetTree().CreateTimer(time), "timeout");
		WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.HORIZONTAL_DASH);
	}
}
