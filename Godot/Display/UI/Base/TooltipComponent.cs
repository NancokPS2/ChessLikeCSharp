using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;
using Vector2 = Godot.Vector2;

public partial class TooltipComponent : Node
{
    private Control Parent;

    [Export]
    public string Text = "";
    [Export]
    public Vector2 Size = new(120,80);
    [Export]
    public Godot.Font Font = Global.Readonly.FONT_SMALL;
    [Export]
    public int FontSize = 16;

    public CanvasItem DrawTarget = Global.DrawNode;

    public TooltipComponent(Control parent)
    {
        Parent = parent;
    }
    public TooltipComponent(
        Control parent, 
        Vector2 Size, 
        Godot.Font Font,
        int FontSize
        ) : this(parent)
    {
        Parent = parent;
        this.Size = Size;
        this.Font = Font;
        this.FontSize = FontSize;
    }
    public override void _EnterTree()
    {
        base._EnterTree();
        //Parent = GetParent<Control>();

        //Parent.MouseEntered += ProcessingEnabled;
        //Parent.MouseExited += ProcessingDisabled;
        DrawTarget.Connect(CanvasItem.SignalName.Draw, Callable.From(Draw));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        //Parent.MouseEntered -= ProcessingEnabled;
        //Parent.MouseExited -= ProcessingDisabled;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        DrawTarget.QueueRedraw();
    }
    public void Draw()
    {
        UpdateFromParent(Parent);

        if (Text == ""){return;}

        Vector2 mouse_pos = Parent.GetGlobalMousePosition();
        DrawTarget.DrawRect(
            new(mouse_pos, Size), 
            new(0,0,0,0.3f)
        );
        DrawTarget.DrawMultilineString(
            Font,
            mouse_pos,
            Text, 
            HorizontalAlignment.Left,
            Size.X,
            FontSize,
            -1,
            null,
            TextServer.LineBreakFlag.Mandatory | TextServer.LineBreakFlag.WordBound
        );
    }

    private void ProcessingEnabled()
    {
        SetProcess(true);
    }
    private void ProcessingDisabled()
    {
        SetProcess(false);
    }
    private void UpdateFromParent(Control parent)
    {
        if (parent is ITooltip tool)
        {
            Text = tool.GetText();
            Size = tool.GetRectSize();
            Font = tool.GetFont();
            FontSize = tool.GetFontSize();
        }
        else
        {
            Text = parent.TooltipText;
        }
    }
}

public interface ITooltip
{
    public string GetText();
    public Godot.Font GetFont();
    public Vector2 GetRectSize();
    public int GetFontSize();
}
