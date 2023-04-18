using Godot;
using System;

public partial class Main : Node2D
{

    [Export] private PackedScene[] Scenes;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RenderingServer.SetDefaultClearColor(new Color(63.0f, 56.0f, 81.0f, 1.0f));
        LevelManager.Instance.SetScenes(Scenes);
        LevelManager.Instance.LoadLevel(0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
