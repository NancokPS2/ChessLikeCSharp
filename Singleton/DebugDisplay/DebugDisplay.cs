using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Used to show togglable debug information.
/// </summary>
public partial class DebugDisplay : Node
{
    public static DebugDisplay Instance;

    [Export]
    public Godot.Vector2 Offset = new(15,15);
    [Export]
    public Godot.Font FontDraw = new SystemFont(){FontWeight = 700};
    [Export]
    public string ShowMenuAction = "debug_draw";
    [Export]
    public string ShowResourcesAction = "debug_show_res_list";
    private UniqueList<IDebugDisplay> Sources = new();
    private IDebugDisplay? SourceSelected;
    private DebugResourceList MenuResources = new DebugResourceList().GetInstantiatedScene<DebugResourceList>();
    private PopupMenu NodeMenuDebugInfo = new();
    private Node2D NodeDrawTarget = new(){Name = "DebugDisplayDrawTarget"};

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        NodeMenuDebugInfo.IdPressed += OnIdPressed;

        //Setup nodes
        UI.GetLayer(UI.ELayer.DEBUG_DRAW).AddChild(NodeMenuDebugInfo);
        UI.GetLayer(UI.ELayer.DEBUG_DRAW).AddChild(NodeDrawTarget);

        NodeDrawTarget.Draw += DrawTarget;
    }

    public static void Add(IDebugDisplay source)
    {
        Instance.Sources.Add(source);
        Instance.UpdateMenu();
        if (Instance.SourceSelected is null)
        {
            Instance.OnIdPressed(0);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed(ShowMenuAction))
        {
            ShowMenu();
        }
        else if (Input.IsActionJustPressed(ShowResourcesAction))
        {
            ShowResources();
        }
        NodeDrawTarget.QueueRedraw();
        
    }
    public void DrawTarget()
    {
        if (SourceSelected is null) {return;}
        NodeDrawTarget.DrawMultilineStringOutline(
            FontDraw, 
            Offset, 
            SourceSelected.GetText(), 
            HorizontalAlignment.Left,
            -1,
            16,
            -1,
            1, new Godot.Color(0,0,0,1)
        );
        NodeDrawTarget.DrawMultilineString(
            FontDraw, 
            Offset, 
            SourceSelected.GetText(), 
            HorizontalAlignment.Left,
            -1,
            16,
            -1, 
            new Godot.Color(1,1,1,1)
            );
    }

    public void ShowMenu()
    {
        NodeMenuDebugInfo.Popup(new((int)Offset.X, (int)Offset.Y,240,240));
    }

    public void ShowResources()
    {
        if (MenuResources.IsInsideTree())
        {
            MenuResources.RemoveSelf();
        }
        else
        {
            //MenuResources.Position = new(40,40);
            MenuResources.CustomMinimumSize = new(400,400);
            GetTree().Root.AddChild(MenuResources);
            MenuResources.RefreshEntries();
        }
    }

    public void UpdateMenu()
    {
        NodeMenuDebugInfo.Clear(true);
        int id = 0;

        foreach (var item in Sources)
        {
            NodeMenuDebugInfo.AddItem(item.GetName(), id);

            Debug.Assert(Sources[id] == item);

            id ++;
        }
    }

    public void OnIdPressed(long id)
    {
        SourceSelected = Sources[(int)id];
    }

}
