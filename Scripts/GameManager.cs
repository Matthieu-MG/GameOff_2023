using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager gameManager;

    public override void _Ready()
    {
        if (gameManager == null)
		{
			gameManager = this;
		}
    }

    public async void FrameFreeze(float timeScale, float duration)
	{
		Engine.TimeScale = timeScale;
		await ToSignal(GetTree().CreateTimer(duration * timeScale), "timeout");
		Engine.TimeScale = 1;
	}
}
