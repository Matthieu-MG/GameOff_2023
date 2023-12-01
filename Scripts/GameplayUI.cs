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
		if(!PlayerMovement.BonusActivated)
			_BonusProgressBar.Value = PlayerMovement.BonusBar;

		if (PlayerMovement.BonusActivated)
		{
			GD.Print("Lerping Bar");
			_t += (float) (0.001f * delta);
			PlayerMovement.BonusBar = Mathf.Lerp(PlayerMovement.BonusBar, 0, _t);
			_BonusProgressBar.Value = PlayerMovement.BonusBar;
			GD.Print("Bonus Bar Value",_BonusProgressBar.Value);
		}
		else if (_BonusProgressBar.Value >= 100)
		{
			_BonusProgressBar.TintProgress = Color.Color8(255,102,0);
		}
	}
}
