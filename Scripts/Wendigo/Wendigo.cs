using Godot;
using System;

public partial class Wendigo : CharacterBody2D
{
	[Export] double jumpHeight = 400;
	double jumpTimeToPeak = 0.32f;
	[Export] double jumpTimeToFall = 0.5f;

	//**TODO Assign them in Arena Script Instead
	[Export] Node2D Point_1;
	[Export] Node2D Point_2;
	[Export] Node2D Point_3;
	[Export] Node2D Point_4;
	public static Node2D Point_L;
	public static Node2D Point_R;
	public static Node2D Point_U;
	public static Node2D Point_D;

	[Export] AnimationPlayer _arenaPlayer;
	public static AnimationPlayer ArenaPlayer; 
	[Export] GpuParticles2D _landParticles;
	public static GpuParticles2D LandParticles;

	//** SnowShockwave
	[Export] SnowShockwave snowShockwave;
	public static SnowShockwave snowShockwave_Right;
	[Export] SnowShockwave snowShockwave2;
	public static SnowShockwave snowShockwave_Left;

	double jumpVelocity;
	double jumpGravity;
	double fallGravity;

	bool HasJumped;

    public override void _Ready()
    {
		Point_L = Point_1;
		Point_R = Point_2;
		Point_U = Point_3;
		Point_D = Point_4;

		ArenaPlayer = _arenaPlayer;
		LandParticles = _landParticles;

		snowShockwave_Right = snowShockwave;
		snowShockwave_Left = snowShockwave2;

		HasJumped = false;
		WendigoFSM.wendigoFSM.animPlayer.Stop();
        jumpVelocity = ((2.0 * jumpHeight) / jumpTimeToPeak) * -1.0;
		jumpGravity = ((-2.0 * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0;
		fallGravity = ((-2.0 * jumpHeight) / (jumpTimeToFall * jumpTimeToFall)) * -1.0;
    }

	private double GetGravity()
	{
		if (WendigoFSM.GetWendigoVel().Y < 0.0)
		{
			return jumpGravity;
		}

		return fallGravity;
	}
}
