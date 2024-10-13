using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using Godot;

public partial class MessageQueue : CanvasLayer
{
    static public MessageQueue Instance;
    private static VBoxContainer NodeContainer = new()
    {
        FocusMode = Control.FocusModeEnum.None,
        MouseFilter = Control.MouseFilterEnum.Ignore,
        AnchorRight = 1,
        AnchorBottom = 1,   
    };

    public static List<TemporaryLabel> Queue = new();

    public MessageQueue()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        for (int i = 0; i < Queue.Count; i++)
        {
            TemporaryLabel? label = Queue[i];
            label.Duration -= delta;

            if (label.Duration <= 0)
            {
                Queue.Remove(label);
                label.AnimateFadeAway(0.5f, true);
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        AddChild(NodeContainer);
        NodeContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        Layer = Global.Constants.LAYER_MSG_QUEUE;
    }

    public void SetAnchors(float left, float top, float right, float bottom)
    {
        NodeContainer.AnchorLeft = left;
        NodeContainer.AnchorTop = top;
        NodeContainer.AnchorRight = right;
        NodeContainer.AnchorBottom = bottom;
    }

    public static void AddMessage(string text, double duration)
    {
        TemporaryLabel new_label = new();
        new_label.Text = text;
        new_label.Duration = duration;
        NodeContainer.AddChild(new_label);
        Queue.Add(new_label);
    }

    public partial class TemporaryLabel : Label
    {
        public double Duration;

        public TemporaryLabel()
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill;
            SizeFlagsVertical = SizeFlags.ShrinkEnd;
            FocusMode = Control.FocusModeEnum.None;
            MouseFilter = Control.MouseFilterEnum.Ignore;
            AddThemeStyleboxOverride("normal", new StyleBoxFlat(){BgColor = new(0,0,0,0.1f)});
        }
    }
}
