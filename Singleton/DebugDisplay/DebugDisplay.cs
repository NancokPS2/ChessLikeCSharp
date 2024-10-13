using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class DebugDisplay : Node2D
{
    public static DebugDisplay Instance;

    [Export]
    public Godot.Vector2 Offset = new(15,15);
    [Export]
    public Godot.Font FontDraw = new SystemFont(){FontWeight = 700};
    [Export]
    public string ShowMenuAction = "debug_draw";
    private UniqueList<IDebugDisplay> Sources = new();
    private IDebugDisplay? SourceSelected;
    private PopupMenu Menu = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        Menu.IdPressed += OnIdPressed;
        AddChild(Menu);

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
        Menu.Popup(new((int)Offset.X, (int)Offset.Y,240,240));

    }

    public void UpdateMenu()
    {
        Menu.Clear(true);
        int id = 0;

        foreach (var item in Sources)
        {
            Menu.AddItem(item.GetName(), id);

            Debug.Assert(Sources[id] == item);

            id ++;
        }
    }

    public void OnIdPressed(long id)
    {
        SourceSelected = Sources[(int)id];
    }

}
