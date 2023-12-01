using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class WendigoThrowState : State
{
	[Export] Area2D ThrowObject;
	bool HasStopped = false;
	Vector2 TargetPos;
	Vector2 InitialPos;
	float weight = 0.05f;

	enum ThrowState
	{
		Throwing,
		Returning
	}

	ThrowState state;

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		SetThrowObject(true);
		HasStopped = false;
		InitialPos = ThrowObject.GlobalPosition;
		state = ThrowState.Throwing;

		// If Opposite Point Is On The Left Throw To Left
		if (WendigoGoToPointState.PointPos.X - Wendigo.Point_R.Position.X >= 0)
		{
			TargetPos = Wendigo.Point_L.Position;
		}
		// Else Throw to the Right
		else
		{
			TargetPos = Wendigo.Point_R.Position;
		}
    }

    public override void Update(double delta)
    {
		switch(state)
		{
			case ThrowState.Throwing:

				// If Object is still being throwned, lerp its position to the target point
				if(!HasStopped)
				{
					WendigoFSM.LerpToPoint(ThrowObject, TargetPos, weight);
				}

				float diff = Mathf.Abs(ThrowObject.GlobalPosition.X - TargetPos.X);
				if(diff < 1) // 0.001f
				{
					HasStopped = true;
					state = ThrowState.Returning;
				}
				break;

			case ThrowState.Returning:

				// If Object reached target point, returns to its original Position, back to Wendigo Position
				if(HasStopped)
				{
					WendigoFSM.LerpToPoint(ThrowObject, InitialPos, weight);
				}

				float _diff = Mathf.Abs(ThrowObject.GlobalPosition.X - InitialPos.X);
				if(_diff < 1)
				{
					HasStopped = false;
					WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.IDLE);
				}
				break;
		}
    }

    public override void Exit()
    {
		SetThrowObject(false);
    }

	private void SetThrowObject(bool state)
	{
		ThrowObject.Visible = state;
		ThrowObject.Monitoring = state;
		ThrowObject.Monitorable = state;
	}
}
