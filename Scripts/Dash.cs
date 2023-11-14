using Godot;
using System;

public partial class Dash : Node2D
{
	const float DASH_DELAY = 0.4f;

	PackedScene GhostScene;
	ShaderMaterial _ghostShader;
	[Export] Timer _durationTimer;
	[Export] Timer _ghostTimer;
	public bool CanDash = true;

	Sprite2D sprite;

	public override void _Ready()
	{
		// Preloads Ghost effect and Shader
		GhostScene = (PackedScene)GD.Load("res://Scenes/DashGhost.tscn");
		_ghostShader = GD.Load<ShaderMaterial>("res://Scenes/DashGhost.tscn::ShaderMaterial_6wegj");
	}

	// Called to initiate Timer and Dash
	public void StartDash(float duration, Sprite2D sprite)
	{
		PlayerMovement.player.IsListeningToInput = false;
		this.sprite = sprite;
		_durationTimer.WaitTime = duration;
		_durationTimer.Start();

		_ghostTimer.Start();
	}

	// Creates 1 Instance of Ghost Sprite
	private void InstanceGhost()
	{
		Sprite2D ghost = (Sprite2D) GhostScene.Instantiate();

		ghost.GlobalPosition = GlobalPosition;
		ghost.Texture = sprite.Texture;
		ghost.Scale = sprite.Scale;
		ghost.Material = _ghostShader;

		GetParent().GetParent().AddChild(ghost);
	}
	
	// Checks if Timer has stopped or not to determine if Node Is Dashing
	public bool IsDashing()
	{
		return !_durationTimer.IsStopped();
	}

	// Async method to delay Dash after use
	private async void EndDash()
	{
		_ghostTimer.Stop();
		CanDash = false;
		await ToSignal(GetTree().CreateTimer(DASH_DELAY), "timeout");
		CanDash = true;
	}

	// Callback method upon Timeout event of Duration Timer
	private void OnDurationTimerTimeout()
	{
		EndDash();
		PlayerMovement.player.IsListeningToInput = true;
	}

	// Callback method to create multiple ghost instance in 1 dash
	private void OnGhostTimerTimeout()
	{
		InstanceGhost();
		GameManager.gameManager.FrameFreeze(0.5f, 0.4f);
	}
}
