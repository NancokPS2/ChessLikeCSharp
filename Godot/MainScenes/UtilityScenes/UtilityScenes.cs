using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class UtilityScenes : Node3D
{
    public Node? CurrentScene;
    [Export]
    public SceneButton GridBuilderButton
    {
        get => gridBuilderButton;
        set
        {
            if (gridBuilderButton is not null) gridBuilderButton.SceneButtonPressed -= OnSceneButtonPressed;
            gridBuilderButton = value;
        }
    }
    [Export]
    public Button UnloadSceneButton;

    private SceneButton gridBuilderButton;

    public override void _Ready()
    {
        base._Ready();
        UnloadSceneButton.Pressed += OnUnloadSceneButtonPressed;
    }


    protected void SwapScene(PackedScene? scene)
    {
        if (CurrentScene is not null)
        {
            CurrentScene.QueueFree();
        }
        if (scene is not null)
        {
            CurrentScene = scene.Instantiate<Node>();
            AddChild(CurrentScene);
        }

    }

    private void OnUnloadSceneButtonPressed()
    {
        SwapScene(null);
    }
    private void OnSceneButtonPressed(PackedScene scene)
    {
        SwapScene(scene);
    }
}
