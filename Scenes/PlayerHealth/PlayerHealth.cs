using Godot;
using System;

public partial class PlayerHealth : Node2D
{
    private int Health = 3;
    public override void _Ready()
    {
        SignalBus.Instance.Connect("PlayerHitByExplosion", new Callable(this, "OnHitByExplosion"));
        SignalBus.Instance.Connect("PlayerRespawned", new Callable(this, "OnRespawned"));
    }

    private void OnRespawned()
    {
        var hearts = GetTree().GetNodesInGroup("PlayerLifeBarHeart");
        foreach (Node2D heart in hearts)
        {
            heart.Visible = true;
        }
        Health = 3;
    }

    private void OnHitByExplosion()
    {
        var hearts = GetTree().GetNodesInGroup("PlayerLifeBarHeart");

        if (hearts.Count > 0)
        {
            var heart = hearts[Health - 1] as Node2D;
            heart.Visible = false;
            Health--;
        }

    }
}
