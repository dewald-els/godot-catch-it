using Godot;
using System;

public partial class SignalBus : Node
{
    public static SignalBus Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    [Signal]
    public delegate void PlayerPickedUpBombEventHandler(Node2D bomb);

    [Signal]
    public delegate void PlayerDroppedBombEventHandler(Node2D bomb);

    [Signal]
    public delegate void PlayerHitByExplosionEventHandler();

    [Signal]
    public delegate void PlayerRespawnedEventHandler();

    [Signal]
    public delegate void BombDropOffFullEventHandler();

}
