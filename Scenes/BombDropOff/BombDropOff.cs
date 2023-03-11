using Godot;
using System;

public partial class BombDropOff : Node2D
{
    private Area2D BombDropOffArea;
    private TextureProgressBar CapacityBar;
    private AnimationPlayer AnimationPlayer;
    [Export]
    private int BombCapacity = 1;
    private int BombCount = 0;

    public override void _Ready()
    {
        CapacityBar = GetNode<TextureProgressBar>("CapacityBar");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        BombDropOffArea = GetNode<Area2D>("BombDropOffArea");
        // Events
        BombDropOffArea.Connect("body_entered", new Callable(this, "OnBombDropOffAreaEntered"));
    }

    private Boolean IsFull()
    {
        return BombCount >= BombCapacity;
    }

    public override void _Process(double delta)
    {

    }

    public void OnBombDropOffAreaEntered(Node2D body)
    {
        if (!IsFull() && body is Player player)
        {
            SignalBus.Instance.EmitSignal("PlayerDroppedBomb");
            AnimationPlayer.Play("BombDropped");
            BombCount++;
            CapacityBar.Value = Mathf.Ceil((float)BombCount / BombCapacity * 100);
        }
    }
}
