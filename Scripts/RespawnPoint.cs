using Godot;
using System;

public partial class RespawnPoint : Node2D
{
	bool CanAssignPoint = false;
	[Export] TextureRect _ActionIcon;

    public override void _Process(double delta)
    {
        if (CanAssignPoint)
		{
			_ActionIcon.Visible = true;
			if (Input.GetActionStrength("Rest") > 0)
			{
				PlayerMovement.player.spawnPoint = Position;
			}
		}
		else
		{
			_ActionIcon.Visible = false;
		}
    }

    // Signal OnBodyEnterred
    private void PromptRespawnPoint(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			CanAssignPoint = true;
		}
	}

	// Signal OnBodyExited
	private void UnpromptRespawnPoint(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			CanAssignPoint = false;
		}
	}
}
