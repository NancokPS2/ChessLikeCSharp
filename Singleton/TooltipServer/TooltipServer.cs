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
    //private Dictionary<ITooltip, Action> LambdaDict = new();

    public override void _Ready()
    {
        base._Ready();
        //Don't put this in the NodeInterceptor singleton. Stuff breaks.
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
        //LambdaDict[node] = () => Draw(node);
        node.GetCanvasItem().Draw += () => Draw(node);
    }

    private void RemoveUser(ITooltip node)
    {
        TooltipUsers.Remove(node);
        //node.GetCanvasItem().Draw -= LambdaDict[node];
        //LambdaDict.Remove(node);
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
        Vector2 mouse_pos = DrawTarget.GetLocalMousePosition() + (Vector2.One * 32);

        //Prepare values.
        Vector2 pos = mouse_pos;
        string text = user.GetText();
        HorizontalAlignment alignment = HorizontalAlignment.Left;
        float width = user.GetRectSize().X;
        int font_size = user.GetFontSize();
        int max_lines = -1;
        Godot.Color modulate = Colors.White;
        TextServer.LineBreakFlag break_flags = TextServer.LineBreakFlag.Mandatory | TextServer.LineBreakFlag.WordBound;


        //Get the size of the text.
        Vector2 text_size = user.GetFont().GetMultilineStringSize(
            text,
            alignment,
            width,
            font_size,
            max_lines,
            break_flags
        );

        //Decide which side to draw on.
        Viewport viewport = DrawTarget.GetViewport();
        Rect2 view_rect = viewport.GetVisibleRect();
        if (!view_rect.HasPoint(DrawTarget.GetGlobalMousePosition())){ new Exception("This ain't how it works.");}
        bool mouse_in_right_side = DrawTarget.GetGlobalMousePosition().X > (view_rect.Position.X + (view_rect.Size.X / 2));
        bool mouse_in_bottom_side = DrawTarget.GetGlobalMousePosition().Y > (view_rect.Position.Y + (view_rect.Size.Y / 2));
        if (mouse_in_right_side)
        {
            pos += (Vector2.Left * text_size.X) + (Vector2.Left * 32);
        }
        if (mouse_in_bottom_side)
        {
            pos += (Vector2.Up * text_size.Y) + (Vector2.Up * 32);
        }

        //Draw the background
        DrawTarget.DrawRect(
            new(pos, text_size), 
            new(0,0,0,0.4f)
        );

        //Draw the text
        DrawTarget.DrawMultilineString(
            user.GetFont(),
            pos + (Vector2.Down * user.GetFontSize()),
            text,
            alignment,
            width,
            font_size,
            max_lines,
            modulate,
            break_flags
        );
    }

}
