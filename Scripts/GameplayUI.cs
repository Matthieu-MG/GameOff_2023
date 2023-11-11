using Godot;
using System;

public partial class GameplayUI : Control
{
	[Export] private TextureProgressBar _BonusProgressBar;
	float _t = 0.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_BonusProgressBar.Value = PlayerMovement.BonusBar;

		if (PlayerMovement.BonusActivated)
		{
			_t += (float) (0.001f * delta);
			PlayerMovement.BonusBar = Mathf.Lerp(PlayerMovement.BonusBar, 0.0f, _t);
		}
		else if (_BonusProgressBar.Value >= 100)
		{
			_BonusProgressBar.TintProgress = Color.Color8(255,102,0);
		}
	}
}
