using Godot;
using System;
using System.Collections.Generic;

public partial class WendigoGoToPointState : State
{
	Tween tween;
	public static Vector2 PointPos;
	CharacterBody2D wendigo;
	[Export] float weight = 0.2f;

	bool Stop = false;
	static RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		Stop = false;
		PointPos = Wendigo.Point_L.Position;
        if(message != null)
		{
			if(message.ContainsKey("Point"))
			{
				if(message["Point"] == 1)
				{
					PointPos = Wendigo.Point_R.Position;
				}
				else
				{
					PointPos = Wendigo.Point_L.Position;
				}
			}
		}

		wendigo = WendigoFSM.wendigoFSM.wendigoBody;
    }

    public override void Update(double delta)
    {
		if(!Stop)
		{
			WendigoFSM.LerpToPoint(wendigo, PointPos, weight, false);
		}
		else
		{
			WendigoFSM.wendigoFSM.LookAtPlayer();
			WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.THROW);
		}

		float diff = Mathf.Abs(wendigo.Position.X - PointPos.X);
		if(diff < 0.001f)
		{
			Stop = true;
		}
    }

	public static int GetPoint()
	{
		return rng.RandiRange(0,1);
	}
}
