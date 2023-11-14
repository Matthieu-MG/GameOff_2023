using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public partial class Camera2D : Godot.Camera2D
{
	public static Camera2D camera;

	[Export] PlayerMovement _player;
	public Vector2 pos;

	#region Switching Variables
	public bool IsSwitching = false;
	float _t = 0.0f;

	public float x_axisRestraint;
	public float y_axisRestraint;

	public enum Follow
	{
		X_Only,
		Y_Only,
		Both
	}

	public Follow CameraFollow;

	#endregion
	
	#region Camera Shake Variables
	bool Shake = false;
	[Export] float randomStrength = 30.0f;
	[Export] float shakeFade = 3.0f;

	RandomNumberGenerator rng = new RandomNumberGenerator();
	float shakeStrength = 5.0f;
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (camera == null)
		{
			camera = this;
		}

		CameraFollow = Follow.X_Only;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		pos = this.Position;

		/*GD.Print("Shake: ", Shake);
		GD.Print("Switching: ", IsSwitching);
		GD.Print("Camera Follow: ", CameraFollow);
*/
		if (Shake)
		{
			if(shakeStrength > 0.1)
			{
				shakeStrength = Mathf.Lerp(shakeStrength, 0, shakeFade * (float) delta);
				
				Offset = RandomOffset();
			}
			else
			{
				Shake = false;
			}	
		}

		if (IsSwitching)
		{
			/*
			_t += (float) (0.2f * delta);
			//pos = pos.Lerp(_player.Position, _t);

			// * Problem lies with Distance and Lerping to player position when the latter is moving

			Vector2 lerpPos = PlayerMovement.player.Position;

			if (CameraFollow == Follow.X_Only)
			{
				pos.Y = y_axisRestraint;
				pos.X = Mathf.Lerp(pos.X, lerpPos.X, _t);
			}

			/*
			else if (CameraFollow == Follow.Y_Only)
			{
				lerpPos.X = x_axisRestraint;
			}
			
			else
				pos = pos.Lerp(lerpPos, _t);

			float distance = pos.DistanceTo(_player.Position);
			// GD.Print("Distance: ",distance);
			if(distance < 0.2f)
			{
				IsSwitching = false;
				_t = 0.0f;
			}
			*/
			IsSwitching = false;
		}

		else
		{
			
			switch (CameraFollow)
			{
				case Follow.X_Only:
					pos.X = _player.Position.X;
					break;

				case Follow.Y_Only:
					pos.Y = _player.Position.Y;
					break;

				case Follow.Both:
					pos.X = _player.Position.X;
					pos.Y = _player.Position.Y;
					break;
			}
		}

		Position = pos;
	}

	private void ApplyShake()
	{
		shakeStrength = randomStrength;
	}

	private Vector2 RandomOffset()
	{
		return new Vector2(rng.RandfRange(-shakeStrength,shakeStrength), rng.RandfRange(-shakeStrength,shakeStrength));
	}

	public void InitiateShake()
	{
		ApplyShake();
		Shake = true;
	}
}
