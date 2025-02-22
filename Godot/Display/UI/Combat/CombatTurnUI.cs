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

    public CombatTurnUI()
    {
        EventBus.MobTurnStarted += OnTurnChanged;
        EventBus.MobTurnEnded += OnTurnChanged;
    }

    private void OnTurnChanged(Mob mob, TurnManager manager)
    {
        Update(manager);
    }


    public override void _Ready()
    {
        base._Ready();
        NodeTurnContainer ??= (Control)FindChild("TurnContainer");
    }

    public void Update(TurnManager manager)
    {
        if(NodeTurnContainer is null) {throw new Exception("Null NodeTurnContainer");}

        NodeTurnContainer.FreeChildren();

        List<ITurn> participants = manager.GetParticipants();
        participants.Sort( comparison: (ITurn x, ITurn y) => 
            x.DelayCurrent < y.DelayCurrent ? -1 : 1
        );

        foreach (var item in participants)
        {
            DelayContainer delay_container = new (item);
            NodeTurnContainer.AddChild(delay_container);
            if (item == manager.GetCurrentTurnTaker())
            {
                delay_container.Modulate = new(0.8f,1,0.8f);
            }
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
