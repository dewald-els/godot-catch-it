using Godot;
using System;

public partial class LevelManager : Node
{
    public static LevelManager Instance;
    private PackedScene[] Scenes;

    private int CurrentScene = 0;

    public override void _Ready()
    {
        Instance = this;
        SignalBus.Instance.Connect("BombDropOffFull", new Callable(this, "LoadNextLevel"));
    }

    public void SetScenes(PackedScene[] scenes)
    {
        Scenes = scenes;
    }

    public void LoadLevel(int levelIndex)
    {
        CurrentScene = levelIndex;
        GetTree().ChangeSceneToPacked(Scenes[CurrentScene]);
    }

    public void LoadNextLevel()
    {
        LoadLevel(CurrentScene + 1);
    }
}
