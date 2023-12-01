using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class WendigoHorizontalDashState : State
{
	bool HasDashed = false;
	[Export] Dash _dash;
	[Export] float dashSpeed;

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		HasDashed = false;
        WendigoFSM.wendigoFSM.animPlayer.Stop();
    }

    public override void PhysicsUpdate(double delta)
    {
		/*
		if(!HasJumped)
		{
			Jump(ref vel);
			WendigoFSM.wendigoFSM.wendigoBody.Velocity = vel;
			WendigoFSM.wendigoFSM.wendigoBody.MoveAndSlide();
		}
		*/
		Vector2 vel = WendigoFSM.wendigoFSM.wendigoBody.Velocity;
		if(!HasDashed)
		{
			_dash.StartDash(0.1f, new Sprite2D());
			HasDashed = true;
		}

		if(_dash.IsDashing())
		{
			vel.X = dashSpeed;
			vel.Y = 0;
		}
		else if(HasDashed)
		{
			// vel = Vector2.Zero;
			WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.IDLE);
		}

		if(HasDashed && !_dash.IsDashing())
		{
			// WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.IDLE);
		}

		WendigoFSM.SetWendigoBodyX(vel.X);
		WendigoFSM.SetWendigoBodyY(vel.Y);
		WendigoFSM.CallMoveAndSlide();
    }

    public override void Exit()
    {
		HasDashed = false;
    }
}
