using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Command;
using ChessLike.Extension;
using Godot;

public partial class MessageQueue: Node
{
    public static MessageQueue Instance;
    private CanvasLayer Canvas;
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
        Canvas = UI.GetLayer(UI.ELayer.MSG_QUEUE);

        Canvas.AddChild(NodeContainer);
        NodeContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);


        EventBus.MobCommandUsed += AddMessageFromCommand;
    }

    private void AddMessageFromCommand(Dictionary<EInfo, string> dictionary)
    {
        string message = MobCommand.ParseInfo(dictionary);
        AddMessage(message, message.Length/6);
    }

    public void SetAnchors(float left, float top, float right, float bottom)
    {
        NodeContainer.AnchorLeft = left;
        NodeContainer.AnchorTop = top;
        NodeContainer.AnchorRight = right;
        NodeContainer.AnchorBottom = bottom;
    }

    public static TemporaryLabel AddMessage(string text, double duration = 5)
    {
        TemporaryLabel new_label = new();
        new_label.Text = text;
        new_label.Duration = duration;
        NodeContainer.AddChild(new_label);
        Queue.Add(new_label);
        return new_label;
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
