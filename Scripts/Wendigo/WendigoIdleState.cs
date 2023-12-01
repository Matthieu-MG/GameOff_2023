using Godot;
using System;
using System.Collections.Generic;

public partial class WendigoIdleState : State
{
	float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    
    Vector2 vel;
    bool CalledChangeState = false;

    [Export] Area2D Shockwave;

    public override void Enter(IDictionary<StringName, int> message = null)
    {
        CalledChangeState = false;
        WendigoFSM.wendigoFSM.animPlayer.Play("Idle");
    }

    public override void PhysicsUpdate(double delta)
    {
        // Apply Gravity
        vel = WendigoFSM.wendigoFSM.wendigoBody.Velocity;
        vel.Y += _gravity;

        // If Is Grounded, Switch State
        if (WendigoFSM.wendigoFSM.wendigoBody.IsOnFloor())
        {
            if (!CalledChangeState)
            {
                if(WendigoFSM.GetPreviousState() == (int)WendigoFSM.WENDIGO_STATES.VERTICAL_DASH)
                {
                    SetShockwave(true);
                }
                ChooseState();
            }
            WendigoFSM.wendigoFSM.LookAtPlayer();
        }

        WendigoFSM.wendigoFSM.wendigoBody.Velocity = vel;
        WendigoFSM.wendigoFSM.wendigoBody.MoveAndSlide();
    }

    private async void ChooseState()
    {
        CalledChangeState = true;
        await ToSignal(GetTree().CreateTimer(2), "timeout");

        // Assigns a value to Point key in case GoToPoint State is chosen, choosing a random point to go to
        WendigoFSM.message["Point"] = WendigoFSM.GetPoint();

        // Array of random states to be selected and played
        int[] states = new int[3];
        states[0] = (int) WendigoFSM.WENDIGO_STATES.GO_TO_POINT;
        states[1] = (int) WendigoFSM.WENDIGO_STATES.ATTACK;
        states[2] = (int) WendigoFSM.WENDIGO_STATES.VERTICAL_DASH;

        SetShockwave(false);

        // Transitions to a random state present in the array passed in
        WendigoFSM.wendigoFSM.RandomTransitionState(states, WendigoFSM.message);
    }

    private void SetShockwave(bool state)
    {
        Shockwave.Visible = state;
        Shockwave.Monitoring = state;
        Shockwave.Monitorable = state;
    }
}
