using Godot;
using Godot.Collections;
using System;

public partial class World : Node2D
{
    Timer BombSpwawnTimer;
    PackedScene scene = GD.Load<PackedScene>("res://Scenes/Bomb/Bomb.tscn");
    Array<Node> markers;
    RandomNumberGenerator random = new RandomNumberGenerator();
    public override void _Ready()
    {
        markers = GetTree().GetNodesInGroup("BombSpawnMarker");
        GD.Print("Markers: " + markers.Count);
        BombSpwawnTimer = GetNode<Timer>("BombSpawnTimer");
        BombSpwawnTimer.Timeout += OnBombSpwawnTimerTimeout;
        SignalBus.Instance.Connect("BombDropOffFull", new Callable(this, "OnBombDropOffFull"));
    }

    private void OnBombDropOffFull()
    {
        BombSpwawnTimer.Stop();
    }

    private void OnBombSpwawnTimerTimeout()
    {
        var bomb = scene.Instantiate() as Bomb;
        AddChild(bomb);
        var marker = markers[random.RandiRange(0, markers.Count - 1)] as Marker2D;
        bomb.Position = marker.Position;

        BombSpwawnTimer.Stop();
        BombSpwawnTimer.Start();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _ExitTree()
    {
        BombSpwawnTimer.Timeout -= OnBombSpwawnTimerTimeout;
    }
}
