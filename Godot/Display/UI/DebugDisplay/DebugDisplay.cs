using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.DebugDisplay;

[GlobalClass]
public partial class DebugDisplay : Node2D
{
    public static DebugDisplay Instance;

    [Export]
    public Godot.Vector2 Offset = new(15,15);
    [Export]
    public Godot.Font FontDraw = new SystemFont ();
    [Export]
    public string ShowMenuAction = "debug_draw";
    private UniqueList<IDebugDisplay> Sources = new();
    private IDebugDisplay? SourceSelected;
    private PopupMenu Menu = new();

    public override void _Ready()
    {
        base._Ready();
        Menu.IdPressed += OnIdPressed;
        Instance = this;
        AddChild(Menu);
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

        DrawMultilineString(FontDraw, Offset, SourceSelected.GetText());
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
