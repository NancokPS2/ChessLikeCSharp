using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class SceneButton : Button
{
    public delegate void SceneButtonPress(PackedScene scene);
    public event SceneButtonPress? SceneButtonPressed;

    [Export]
    protected PackedScene Scene;
}
