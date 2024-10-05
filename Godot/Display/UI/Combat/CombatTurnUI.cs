using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;

namespace Godot.Display;

public partial class CombatTurnUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Combat/CombatTurnUI.tscn";

    [Export]
    public Control? NodeTurnContainer;

    public override void _Ready()
    {
        base._Ready();
        NodeTurnContainer ??= (Control)FindChild("TurnContainer");
    }

    public void Update(TurnManager manager)
    {
        if(NodeTurnContainer is null) {throw new Exception("Null NodeTurnContainer");}

        NodeTurnContainer.FreeChildren();

        foreach (var item in manager.GetParticipants())
        {
            NodeTurnContainer.AddChild(new DelayContainer(item));
        }
    }

    private partial class DelayContainer : TextureRect
    {
        private readonly ITurn User;
        public DelayContainer(ITurn turn)
        {
            User = turn;
        }

        public override void _Ready()
        {
            base._Ready();
            SetAnchorsPreset(LayoutPreset.FullRect);
            SizeFlagsHorizontal = SizeFlags.ExpandFill;

            //Label and name
            string name = "";
            string delay = "0?";
            if (User is Mob mob)
            {
                name = mob.DisplayedName;
                delay = mob.DelayCurrent.ToString();
            }

            Label label_name = new();
            label_name.Text = name;
            AddChild(label_name);
            label_name.SetAnchorsPreset(LayoutPreset.TopWide);

            Label label_delay = new();
            label_delay.Text = delay;
            AddChild(label_delay);
            label_delay.SetAnchorsPreset(LayoutPreset.BottomWide);
        }

    }
}
