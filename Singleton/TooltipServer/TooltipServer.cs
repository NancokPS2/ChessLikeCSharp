using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Vector2 = Godot.Vector2;

public partial class TooltipServer : Node
{
    public static TooltipServer? Instance;

    public TooltipServer()
    {
        Instance = this;
    }

    private List<ITooltip> TooltipUsers = new();
    private Dictionary<ITooltip, Action> LambdaDict = new();

    public override void _Ready()
    {
        base._Ready();
        GetTree().NodeAdded += OnNodeEntered;

    }

    private void OnNodeEntered(Node node)
    {
        if (node is ITooltip tool)
        {
            AddUser(tool);
        }
    }

    private void AddUser(ITooltip node)
    {
        TooltipUsers.Add(node);
        LambdaDict[node] = () => Draw(node);
        node.GetCanvasItem().Draw += LambdaDict[node];
    }

    private void RemoveUser(ITooltip node)
    {
        TooltipUsers.Remove(node);
        node.GetCanvasItem().Draw -= LambdaDict[node];
        LambdaDict.Remove(node);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        foreach (var item in TooltipUsers)
        {
            if (item.IsDirty())
            {
                item.GetCanvasItem().QueueRedraw();
            };
        }
    }

    public void Draw(ITooltip user)
    {
        if (!user.IsShown()){return;}

        CanvasItem DrawTarget = user.GetCanvasItem();
        Vector2 mouse_pos = DrawTarget.GetLocalMousePosition() + (Vector2.Right * 32);
        DrawTarget.DrawRect(
            new(mouse_pos, user.GetRectSize()), 
            new(0,0,0,0.3f)
        );

        DrawTarget.DrawMultilineString(
            user.GetFont(),
            mouse_pos + (Vector2.Down * user.GetFontSize()),
            user.GetText(), 
            HorizontalAlignment.Left,
            user.GetRectSize().X,
            user.GetFontSize(),
            -1,
            Colors.White,
            TextServer.LineBreakFlag.Mandatory | TextServer.LineBreakFlag.WordBound
        );
    }

}
