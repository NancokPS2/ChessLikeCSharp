using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Used to show togglable debug information.
/// </summary>
public partial class DebugDisplay : Node2D
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
    private PopupMenu MenuDebugInfo = new();
    private DebugResourceList MenuResources = new DebugResourceList().GetInstantiatedScene<DebugResourceList>();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        MenuDebugInfo.IdPressed += OnIdPressed;
        AddChild(MenuDebugInfo);

        ZIndex = (int)RenderingServer.CanvasItemZMax;
        TopLevel = true;
    }

    public void Add(IDebugDisplay source)
    {
        Sources.Add(source);
        UpdateMenu();
        if (SourceSelected is null)
        {
            OnIdPressed(0);
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
        QueueRedraw();
        
    }
    public override void _Draw()
    {
        base._Draw();
        if (SourceSelected is null) {return;}
        DrawMultilineStringOutline(
            FontDraw, 
            Offset, 
            SourceSelected.GetText(), 
            HorizontalAlignment.Left,
            -1,
            16,
            -1,
            1, new Godot.Color(0,0,0,1)
        );
        DrawMultilineString(
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
        MenuDebugInfo.Popup(new((int)Offset.X, (int)Offset.Y,240,240));
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
        MenuDebugInfo.Clear(true);
        int id = 0;

        foreach (var item in Sources)
        {
            MenuDebugInfo.AddItem(item.GetName(), id);

            Debug.Assert(Sources[id] == item);

            id ++;
        }
    }

    public void OnIdPressed(long id)
    {
        SourceSelected = Sources[(int)id];
    }

}
