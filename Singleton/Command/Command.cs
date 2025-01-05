using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;
using Microsoft.VisualBasic;

public partial class Command: Node
{
    public LineEdit TextInput = new(){
        MouseFilter = Control.MouseFilterEnum.Ignore,
        FocusMode = Control.FocusModeEnum.None,
        CustomMinimumSize = new Godot.Vector2(180,48),
        SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
        SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
        AnchorTop = 0.5f,
        AnchorBottom = 0.5f,
        AnchorLeft = 0.5f,
        AnchorRight = 0.5f,
        Visible = false,
        };

    public override void _Ready()
    {
        base._Ready();
        TextInput.TextSubmitted += OnTextSubmitted;
        TextInput.FocusExited += OnFocusExited;
        UI.GetLayer(UI.ELayer.CHEAT_INPUT).AddChild(TextInput);
    }

    private void HideShow(bool show)
    {
        TextInput.Text = "";
        TextInput.Visible = show;
        TextInput.FocusMode = show ? Control.FocusModeEnum.Click : Control.FocusModeEnum.None;
        if (show)
        {
            TextInput.GrabFocus();
        }
        else
        {
            TextInput.ReleaseFocus();
        }
    }

    private void OnFocusExited() => HideShow(false);

    private void OnTextSubmitted(string text)
    {
        var label = MessageQueue.AddMessage("CHEAT ENABLED - ");
        label.Modulate = Godot.Colors.Yellow;
        switch (text)
        {
            case "ultima":
                AllCombatUnitsSetHP(1);
                label.Text += "Sudden death!";
                break;

            case "thesun":
                AllCombatUnitsSetHP(99999);
                label.Text += "Might they not perish.";
                break;

            case "notded":
                if (GetHoveredUnit() is Mob mob_to_heal)
                {
                    mob_to_heal.Stats.SetValue(StatName.HEALTH, 99999);
                }
                label.Text += "Good day.";
                break;

            case "ivefallen":
                if (GetHoveredUnit() is Mob mob_to_hurt)
                {
                    mob_to_hurt.Stats.SetValue(StatName.HEALTH, 1);
                }
                label.Text += "And don't get up!";
                break;

            case "fuckyou":
                BattleController.Instance.FSMSetState(BattleController.State.END_COMBAT);
                label.Text += "Because i say so.";
                break;

            default:
                break;
        }
        HideShow(false);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("command_focus"))
        {
            HideShow( !TextInput.Visible );
        }
    }


    private void AllCombatUnitsSetHP(float amount)
    {
        Global.ManagerMob.GetInCombat().ForEach(
            x => x.Stats.SetValue(ChessLike.Entity.StatName.HEALTH, amount)            
            );
    }

    private Mob? GetHoveredUnit()
    {
        if (Global.ManagerMob.GetInPosition(BattleController.Instance.PositionHovered) is List<Mob> list && list.Count > 0)
        {
            return list.First();
        }
        return null;
    }
}


