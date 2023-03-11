using Godot;
using System;

public partial class BombDropOff : Node2D
{
    private Area2D BombDropOffArea;
    private AnimationPlayer AnimationPlayer;
    private Area2D ExplosionArea;

    public override void _Ready()
    {
        ExplosionArea = GetNode<Area2D>("ExplosionArea");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        BombDropOffArea = GetNode<Area2D>("BombDropOffArea");
        // Events
        BombDropOffArea.Connect("body_entered", new Callable(this, "OnBombDropOffAreaEntered"));
        ExplosionArea.Connect("body_entered", new Callable(this, "OnExplosionAreaEntered"));
    }

    private void OnExplosionAreaEntered(Node2D body)
    {
        if (body is Player player)
        {
            SignalBus.Instance.EmitSignal("PlayerHitByExplosion");
        }
    }

    public void OnBombDropOffAreaEntered(Node2D body)
    {
        if (body is Player player)
        {
            SignalBus.Instance.EmitSignal("PlayerDroppedBomb");
            AnimationPlayer.Play("BombDropped");
        }
    }
}
