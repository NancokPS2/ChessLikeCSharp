using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.NativeInterop;

public partial class SceneChanger: Node
{

    public override void _Ready()
    {
        base._Ready();
        Global.ConnectToWindow(GetWindow());
    }

}
