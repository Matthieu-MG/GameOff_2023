using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] private int MaxHealth = 10;
	private int health;

	private Node parentCharacter;

	[Export] AnimationPlayer _hitFlash;
	[Export] GpuParticles2D _particles;
	[Export] Timer _timer;
	[Export] AudioStreamPlayer _audio;

	[Export] Timer IFrameTimer;
	Area2D playerHitbox;

	public override void _Ready()
	{
		if (this.IsInGroup("Player"))
		{
			playerHitbox = (Area2D) GetParent().GetNode("Hitbox Component");
		}

		parentCharacter = GetParent();
		health = MaxHealth;
	}

	public override void _Process(double delta)
	{
		if (health <= 0)
		{
			if (!parentCharacter.IsInGroup("Player"))
				parentCharacter.QueueFree();

			else
			{
				CharacterBody2D player = (CharacterBody2D) parentCharacter;
				player.Position = PlayerMovement.player.spawnPoint;
				health = MaxHealth;
			}
		}
	}

	public void OnTimeout()
	{
		_particles.Emitting = false;
	}

	// Decreases Health when Node is Hit
	public void OnHit(int damage)
	{
		if (_particles != null)
		{
			_timer.Start();
			_particles.Emitting = true;
		}

		if (this.IsInGroup("Player") && playerHitbox.Monitoring)
		{
			playerHitbox.SetDeferred("monitoring", false);
			IFrameTimer.Start();
		}

		else if(IsInGroup("Enemy"))
		{
			// GameManager.gameManager.FrameFreeze(0.7f, 0.1f);
			if(parentCharacter is Enemy)
			{
				Enemy enemy = (Enemy) parentCharacter;
				enemy.IsKnockedBack = true;
			}
		}
		
		// Flash Sprite White
		_hitFlash.Play("hitFlash");
		
		// _audio.Play();
		// parentCharacter.knockback = true;
		
		// Decrease Health
		health -= damage;

	}
}
