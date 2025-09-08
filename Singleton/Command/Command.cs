using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;
using Microsoft.VisualBasic;

public partial class Command: Node
{
	public enum ECheat { ALL_HP_TO_ONE, ALL_HP_FULL, FUCKYOU,
		TEST_DIALOGUE_BUBBLES
	}
	public LineEdit TextInput = new()
	{
		MouseFilter = Control.MouseFilterEnum.Ignore,
		FocusMode = Control.FocusModeEnum.None,
		CustomMinimumSize = new Godot.Vector2(180, 48),
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
        UIManager.GetLayer(UIManager.ELayer.CHEAT_INPUT).AddChild(TextInput);
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
		MsgLog.TemporaryLabel? label = MsgLog.AddMessage("CHEAT ENABLED - ");
		label.Modulate = Godot.Colors.Yellow;
		ECheat cheat;
		switch (text)
		{
			case "ultima":
				label.Text += "Sudden death!";
				cheat = ECheat.ALL_HP_TO_ONE;
				break;

			case "thesun":
				label.Text += "Might they not perish.";
				cheat = ECheat.ALL_HP_FULL;
				break;

			case "merrykrismas":
				label.Text = "Sing swears!";
				cheat = ECheat.TEST_DIALOGUE_BUBBLES;
				break;

			case "fuckyou":
				label.Text += "Because i say so.";
				cheat = ECheat.FUCKYOU;
				break;

			default:
				HideShow(false);
				return;
		}

		HideShow(false);
		EventBus.InputCheatEntered?.Invoke(cheat);
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
        Mob.GetInstancesInCombat().ForEach(
            x => x.Stats.SetValue(ChessLike.Entity.EValueName.HEALTH, amount)            
            );
    }

    private Mob? GetHoveredUnit()
    {
        throw new NotImplementedException();
        return null;
    }
}


