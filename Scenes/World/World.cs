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

        for (int i = 0; i < markers.Count; i++)
        {
            var marker = markers[i] as BombSpawnPoint;

            if (marker.Disabled == true)
            {
                GD.Print("removing marker " + i);
                markers.Remove(marker);
            }
        }

        GD.Print("Markers", markers);
        GD.Print("found " + markers.Count + " BombSpawnPoints");

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
        var marker = markers.PickRandom() as Marker2D; //markers[random.RandiRange(0, markers.Count - 1)] as Marker2D;
        bomb.Position = marker.Position;

        BombSpwawnTimer.Stop();
        BombSpwawnTimer.Start();
    }

    public override void _ExitTree()
    {
        BombSpwawnTimer.Timeout -= OnBombSpwawnTimerTimeout;
    }
}
