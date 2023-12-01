using Godot;
using System;
using System.Collections.Generic;

public partial class WendigoAttackState : State
{
	int AmountOfAtks = 3;
	public const int MAX_ATTACKS = 5;
	int CurrentAttack = 1;

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		WendigoFSM.wendigoFSM.animPlayer.Stop();
		AmountOfAtks = 3;
		CurrentAttack = 0;
		if (message != null)
			if (message.ContainsKey("Attack"))
			{
				if(message["Attack"] > 1 && message["Attack"] <= MAX_ATTACKS)
				{
					AmountOfAtks = message["Attack"];
				}
			}

		WendigoFSM.wendigoFSM.LookAtPlayer();
    }

    public override void Update(double delta)
    {
        if (!WendigoFSM.wendigoFSM.animPlayer.IsPlaying())
		{
			if (CurrentAttack >= AmountOfAtks)
			{
				int[] states = new int[2];
				states[0] = (int) WendigoFSM.WENDIGO_STATES.IDLE;
				states[1] = (int) WendigoFSM.WENDIGO_STATES.FOUR_POINT_DASH;

				WendigoFSM.message["Point"] = WendigoFSM.GetPoint();
				WendigoFSM.wendigoFSM.RandomTransitionState(states, WendigoFSM.message);
			}
			else
			{
				PlayAttackAnim();
			}
		}
    }

    private void PlayAttackAnim()
	{
		CurrentAttack++;
		switch(CurrentAttack)
		{
			case 1:
				WendigoFSM.wendigoFSM.animPlayer.Play("Attack_1");
				return;

			case 2:
				WendigoFSM.wendigoFSM.animPlayer.Play("Attack_2");
				return;

			case 3:
				WendigoFSM.wendigoFSM.animPlayer.Play("Attack_3");
				return;

			default:
				return;
		}
	}
}
