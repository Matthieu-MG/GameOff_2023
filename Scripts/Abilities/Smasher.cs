using Godot;
using System;

public partial class Smasher : Area2D
{
	public bool CanSmash = true;
	[Export] private float SmashDuration = 1;
	[Export] private Timer SmashTimer;

	[Export] private float SmashCooldownDuration = 10;
	[Export] private Timer SmashCooldownTimer;

	public void StartSmash()
	{
		// Enables Monitoring to detect hittable or breakable nodes, and prevent Smash to be spammed
		Monitoring = true;
		CanSmash = false;

		// Starts Cooldown before Smash Ability can be used again
		SmashCooldownTimer.WaitTime = SmashCooldownDuration;
		SmashCooldownTimer.Start();

		// Starts Timer that determines how much time would the Area2D Smasher would be monitoring
		SmashTimer.WaitTime = SmashDuration;
		SmashTimer.Start();
	}

	// Used by PlayerMovement to check whether Smash is on cooldown and cannot be used or not
	public bool IsSmashOnCooldown()
	{
		return !SmashCooldownTimer.IsStopped();
	}

	// Signal Method To allow Player to use Smash Ability Again 
	private void OnCooldownTimerTimeout()
	{
		CanSmash = true;
	}

	// Disables Monitoring of Smash Ability to prevent any nodes to trigger AreaEntered Signal on PlayerMovement script
	private void OnSmashEnd()
	{
		Monitoring = false;
	}
}
