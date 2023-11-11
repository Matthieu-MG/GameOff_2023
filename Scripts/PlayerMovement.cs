using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	#region Variables

	public static PlayerMovement player;
	public Vector2 spawnPoint = new Vector2(0,-100);

	float action;
	bool FaceRight = true;
	[Export] AnimationPlayer _animPlayer;

	[Export] AnimatedSprite2D _animSprite;

	// Player Hit Variables
	public static bool IsPlayerIFrameActive = false;
	private ColorRect _onHitDarkenScreen;
	[Export] Area2D HitBox;
	[Export] private Area2D LeftHitCheck;
	[Export] private Area2D RightHitCheck;
	[Export] private float KnockbackForce = 400;
	private float KnockbackDirection = -1;
	private bool IsKnockedBack = false;

	// Scale Variables
	float ScaleInput;
	bool isBig = true;
	bool IsTouchingCeiling;
	[Export] double ScaleTime = 0.5f;
	double ScaleTimer = 0;

	// Attack Variables
	[Export] int AtkDamage = 3;
	[Export] double AtkCooldownTime = 0.4f;
	double AtkCooldownTimer = -1;

	enum BonusState
	{
		Empty,
		Full,
		Activated
	}
	BonusState bonusState = BonusState.Empty;
	public static bool BonusActivated = false;
	public static double BonusBar = 0;
	[Export] double BonusAtkPoints = 5;

	[Export] Area2D AtkArea;
	
	// Horizontal Movement Variables
	[Export] float OriginalSpeed = 350f;
	float speed;
	public static float dirX = 1;

	// Dash Variables
	[Export] Dash _dash;
	[Export] float dashSpeed = 4000;
	[Export] float dashDuration = 0.2f;

	// Jump Variables
	public static bool isGrounded;

	[Export] double jumpHeight = 200;
	[Export] double jumpTimeToPeak = 0.4f;
	[Export] double jumpTimeToFall = 0.5f;

	double jumpVelocity;
	double jumpGravity;
	double fallGravity;

	// Jump Buffer And Coyote Timing Variables
	[Export] double jumpBufferTime = 0.1f;
	double jumpBufferTimer = -0.1f;

	[Export] double coyote_time = 0.1f;
	double coyote_timer = -0.1f;

	float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (player == null)
		{
			player = this;
		}

		jumpHeight = 200;
		jumpVelocity = ((2.0 * jumpHeight) / jumpTimeToPeak) * -1.0;
		jumpGravity = ((-2.0 * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) * -1.0;
		fallGravity = ((-2.0 * jumpHeight) / (jumpTimeToFall * jumpTimeToFall)) * -1.0;

		speed = OriginalSpeed;
		_onHitDarkenScreen = (ColorRect) GetParent().GetNode("UserInterface").GetNode("GameplayUI").GetNode("OnPlayerHitDarken");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Attack(delta);
		PlayerActivateBonus();
		CheckBonusState();
		GetScaleInput(ref ScaleInput, delta);
		Flip();
		HandleAnimation();
	}

	#region Movement
	public override void _PhysicsProcess(double delta)
	{
		Vector2 vel = Velocity; 

		isGrounded = IsOnFloor();
		vel.Y += (float) (GetGravity() * delta);

		#region Checks
		if (isGrounded)
		{
			coyote_timer = coyote_time;
		}
		if (Input.IsActionJustPressed("Jump"))
		{
			jumpBufferTimer = jumpBufferTime;
		}
		#endregion

		#region Knockback
		if (IsKnockedBack)
		{
			vel.Y += -KnockbackForce;
			vel.X = KnockbackDirection * (KnockbackForce + 1000);
			IsKnockedBack = false;
		}
		#endregion
		else
		{
			#region Jump
			if(coyote_timer > 0 && jumpBufferTimer > 0)
			{
				vel.Y = (float) jumpVelocity;
			}

			coyote_timer -= delta;
			jumpBufferTimer -= delta;
			#endregion

			#region Run
			// Gets X-axis movement Input -- Sets it to 1, -1 or 0 based on the value returned from GetAxis()
			action = Input.GetAxis("Left", "Right");
			GetDir(ref action);

			vel.X = action * speed;
			#endregion

			#region Dash
			// Starts Dash, if Action was pressed, Player is not dashing already and Cooldown is over
			if ( Input.IsActionJustPressed("Dash") && _dash.CanDash && !_dash.IsDashing())
			{
				_dash.StartDash(dashDuration, GetCurrentSprite());

			}
			// Dash
			if (_dash.IsDashing())
			{
				vel.Y = 0;
				vel.X = dashSpeed * GetPlayerTransformScaleX();
			}
			#endregion
		}

		Velocity = vel;

		MoveAndSlide();
	}

	private double GetGravity()
	{
		if (Velocity.Y < 0.0)
		{
			return jumpGravity;
		}

		return fallGravity;
	}

	// Flips Character Body and children nodes when switching directions
	private void Flip()
	{
		if (FaceRight && action < 0 || !FaceRight && action > 0)
		{
			FaceRight = !FaceRight;
			Vector2 direction = Scale;
			direction.X *= -1;
			Scale = direction;
		}
	}

	// Used to assign a consistent value for movement either 1,-1 or 0
	private void GetDir(ref float MovementInput)
	{
		if(MovementInput > 0)
		{
			MovementInput = 1;
			return;
		}
		else if(MovementInput < 0)
		{
			MovementInput = -1;
			return;
		}

		MovementInput = 0;
	}
	#endregion
	// Returns direction where Player is facing
	public float GetPlayerTransformScaleX()
	{
		if (FaceRight)
		{
			return 1;
		}
		else
		{
			return -1;
		}
	}

	// Handles behaviour when Player wants to scale up or down
	private void GetScaleInput(ref float ScaleInput, double delta)
	{
		ScaleInput = Input.GetActionStrength("Scale");
		if (ScaleInput > 0 && ScaleTimer <= 0)
		{
			if (isBig)
			{
				_animPlayer.Play("ScaleDown");
				isBig = false;
			}
			else if(!IsTouchingCeiling)
			{
				_animPlayer.Play("ScaleUp");
				isBig = true;
			}

			ScaleTimer = ScaleTime;
			// isBig = !isBig;
		}

		ScaleTimer -= delta;
	}

	#region Attack Methods
	// Handles behavior when Player wants to attack
	private void Attack(double delta)
	{
		float IsAttacking = Input.GetActionStrength("Attack");

		if (IsAttacking > 0)
		{
			AtkArea.Monitoring = true;
			AtkCooldownTimer = AtkCooldownTime;
		}
		else
		{
			AtkArea.Monitoring = false;
		}

		AtkCooldownTimer -= delta;
	}

	// Callback method when Area2D for Attack signals AreaEntered Event
	private void OnAttackLanded(Area2D area)
	{
		if (area.IsInGroup("Hit"))
		{
			HealthComponent health = (HealthComponent) area.GetParent().GetNode("Health Component");
			health.OnHit(AtkDamage);

			if (BonusBar < 100)
			{
				BonusBar += BonusAtkPoints;
			}
		}
	}
	#endregion

	// Checks and Assigns correct bonusState depending on the status of the points
	private void CheckBonusState()
	{
		bool ChangeMade = false;

		if (BonusBar < 0.1f)
		{
			BonusActivated = false;
		}

		if (BonusBar < 100 && !BonusActivated && bonusState != BonusState.Empty)
		{
			bonusState = BonusState.Empty;
			ChangeMade = true;
		}
		else if (BonusBar >= 100 && !BonusActivated && bonusState != BonusState.Full)
		{
			bonusState = BonusState.Full;
			ChangeMade = true;
		}
		else if (BonusActivated && bonusState != BonusState.Activated)
		{
			bonusState = BonusState.Activated;
			ChangeMade = true;
		}

		ApplyBonus(ChangeMade);
	}

	// Applies bonus to Player depending on their Bonus State
	private void ApplyBonus(bool ChangeMade)
	{
		if (ChangeMade)
		{
			switch (bonusState)
			{
				case BonusState.Activated:
					speed = OriginalSpeed + 150f;
					break;
				case BonusState.Full:
					speed = OriginalSpeed * 1.2f;
					break;
				case BonusState.Empty:
					speed = OriginalSpeed;
					break;
			}
		}
	}

	#region Animation Methods
	// Handles which Animation the AnimatedSprite2D should play
	private void HandleAnimation()
	{
		if (action == 0)
		{
			_animSprite.Play("idle");
		}
		else
		{
			_animSprite.Play("running");
		}
	}

	// Returns the Sprite of the current frame of the animation being played
	private Sprite2D GetCurrentSprite()
	{
		Sprite2D sprite2D = new Sprite2D();
		sprite2D.Texture = _animSprite.SpriteFrames.GetFrameTexture(_animSprite.Animation, _animSprite.Frame);
		sprite2D.Scale = _animSprite.Scale;
		if (!FaceRight)
		{
			Vector2 newScale = sprite2D.Scale;
			newScale.X = -sprite2D.Scale.X;
			sprite2D.Scale = newScale;
		}
		return sprite2D;
	}
	#endregion

	// Handles behaviour when player is trying to activate bonus
	private void PlayerActivateBonus()
	{
		float ActivateBonus = Input.GetActionStrength("ActivateBonus");
		if (ActivateBonus > 0 && bonusState == BonusState.Full)
		{
			BonusActivated = true;
		}
	}

	#region Hit methods
	// Callback method when Hitbox Component detects an Area Enterred and handles Hit behaviour
	private void OnPlayerHit(Area2D area)
	{
		if (area.IsInGroup("Enemy"))
		{
			IsKnockedBack = true;
			GameManager.gameManager.FrameFreeze(0.7f, 0.4f);
			HealthComponent health = (HealthComponent) GetNode("Health Component");
			Camera2D.camera.InitiateShake();
			_onHitDarkenScreen.Visible = true;
			health.OnHit(2);
		}
	}

	// Detects if Player was hit on its rear
	private void OnLeftHitAreaEntered(Area2D area)
	{
		if (area.IsInGroup("Enemy"))
		{
			KnockbackDirection = 1;
		}
	}

	// Detects if Player was hit on its front
	private void OnRightHitAreaEntered(Area2D area)
	{
		if (area.IsInGroup("Enemy"))
		{
			if (FaceRight)
			{
				KnockbackDirection = -1;
				return;
			}
			KnockbackDirection = 1;
		}
	}

	// Callback method for when Invincible Frames stop and Player can start taking damage again
	private void OnIFrameTimerTimeout()
	{
		GD.Print("IFrame Stops");
		_onHitDarkenScreen.Visible = false;
		IsPlayerIFrameActive = false;
		HitBox.Monitoring = true;
	}
	#endregion

	#region Scale Checks
	// Detects if Player is near ceiling to prevent Player from Scaling Up
	private void OnCeilingTouched(Node2D body)
	{
		if (body.IsInGroup("Tiles"))
		{
			IsTouchingCeiling = true;
		}
	}

	private void OnCeilingExit(Node2D body)
	{
		if (body.IsInGroup("Tiles"))
		{
			IsTouchingCeiling = false;
		}
	}
	#endregion
}
