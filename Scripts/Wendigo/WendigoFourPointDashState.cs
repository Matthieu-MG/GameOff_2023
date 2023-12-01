using Godot;
using System;
using System.Collections.Generic;

public partial class WendigoFourPointDashState : State
{
	Vector2 TargetPointPos;
	Vector2 SecondSidePointPos;
	Node2D wendigo;
	bool CalledAsyncFunc;

	AnimationPlayer ArenaPlayer;
	bool StartedPlayingSpikes;

	enum Direction
	{
		Side_1,
		Side_2,
		Up,
		Down
	}
	Direction direction;
	bool Stop;

    public override void Enter(IDictionary<StringName, int> message = null)
    {
		// Sets/Resets Variables Values for upcoming use
		TargetPointPos = Wendigo.Point_L.Position;
		SecondSidePointPos = Wendigo.Point_R.Position;
		direction = Direction.Side_1;

		Stop = false;
        wendigo = WendigoFSM.wendigoFSM.GetWendigoCharacterBody2D();
		CalledAsyncFunc = false;

		ArenaPlayer = Wendigo.ArenaPlayer;
		StartedPlayingSpikes = false;

		// Get Specific Point if passed in
		if(message != null)
		{
			if(message.ContainsKey("Point"))
			{
				if(message["Point"] == 1)
				{
					TargetPointPos = Wendigo.Point_R.Position;
					SecondSidePointPos = Wendigo.Point_L.Position;
				}
				else
				{
					TargetPointPos = Wendigo.Point_L.Position;
					SecondSidePointPos = Wendigo.Point_R.Position;
				}
			}
		}
    }

    public override void Update(double delta)
    {
		// Emits Landing Particles
		if(direction == Direction.Down && WendigoFSM.wendigoFSM.wendigoBody.IsOnFloor())
			Wendigo.LandParticles.Emitting = true;

		// If Wendigo Stopped Moving, Check direction and Assign New Direction and Target Point
		#region Wendigo Stop Behaviour
		if(Stop)
		{
			Stop = false;
			CalledAsyncFunc = false;

			switch(direction)
			{
				case Direction.Side_1:
					direction = Direction.Side_2;
					TargetPointPos = SecondSidePointPos;
					break;

				case Direction.Side_2:
					direction = Direction.Up;
					TargetPointPos = Wendigo.Point_U.Position;
					break;
				
				case Direction.Up:
					direction = Direction.Down;
					TargetPointPos = Wendigo.Point_D.Position;
					break;

				case Direction.Down:
					Stop=true;
					if(!ArenaPlayer.IsPlaying())
					{
						if(!StartedPlayingSpikes)
						{
							StartedPlayingSpikes = true;
							ArenaPlayer.Play("Spikes");
							Wendigo.snowShockwave_Right.Play = true;
							Wendigo.snowShockwave_Left.Play = true;
						}
						else if(!Wendigo.snowShockwave_Right.Play && !Wendigo.snowShockwave_Left.Play)
						{
							Wendigo.LandParticles.Emitting = false;
							WendigoFSM.wendigoFSM.TransitionState(WendigoFSM.WENDIGO_STATES.IDLE);
						}
					}
					break;

				default:
					break;
			}
		}
		#endregion

		float distance;

		// Moves Wendigo according to current direction by lerping Position to TargetPointPos
		#region Wendigo Movement
		if(direction == Direction.Up)
		{
			WendigoFSM.LerpToPoint(wendigo, TargetPointPos, 0.5f, false, WendigoFSM.LerpModes.Both);
			distance = Mathf.Abs(wendigo.Position.DistanceTo(TargetPointPos));
		}
		else if(direction == Direction.Down)
		{
			WendigoFSM.LerpToPoint(wendigo, TargetPointPos, 0.5f, false, WendigoFSM.LerpModes.Y_Only);
			distance = Mathf.Abs(wendigo.Position.Y - TargetPointPos.Y);
		}
		else
		{
			WendigoFSM.LerpToPoint(wendigo, TargetPointPos, 0.5f, false);
			distance = Mathf.Abs(wendigo.Position.X - TargetPointPos.X);
		}
		#endregion

		// Check Wendigo Distance to Target Point Position
		#region Distance Check
		if (distance < 0.1f)
		{
			if(!CalledAsyncFunc)
			{
				StopMoving();
			}
		}
		#endregion
    }

	// Asynchrous method to Stop Wendigo from moving for 0.4 seconds
	private async void StopMoving()
	{
		CalledAsyncFunc = true;
		await ToSignal(GetTree().CreateTimer(0.4f), "timeout");
		Stop = true;
	}
}
