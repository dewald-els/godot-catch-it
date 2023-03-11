using Godot;
using System;

public partial class BombDropOff : Node2D
{
    private Area2D BombDropOffArea;
    private AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        BombDropOffArea = GetNode<Area2D>("BombDropOffArea");
        // Events
        BombDropOffArea.Connect("body_entered", new Callable(this, "OnBombDropOffAreaEntered"));
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
