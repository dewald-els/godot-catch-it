using Godot;
using System;

public partial class Main : Node2D
{

	[Export] private PackedScene[] Scenes;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LevelManager.Instance.SetScenes(Scenes);
		LevelManager.Instance.LoadLevel(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
