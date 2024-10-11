using ChessLike.Entity;
using Godot;
using System;

public partial class PartyMobListUI : BaseButtonMenu<Button, Mob>
{
	public Mob? MobSelected;

    public override void _Ready()
    {
        base._Ready();
		Container ??= (Control)FindChild("PortraitContainer");
    }

    public void Update(EFaction faction)
	{
		var list = Global.ManagerMob.GetFromFaction(faction);
		Update(list);
	}

    protected override void OnButtonCreated(Button button, Mob param)
    {
		button.Text = param.DisplayedName;
    }

    protected override void OnButtonPressed(Button button, Mob param)
    {
        MobSelected = param;
    }

    protected override void OnButtonHovered(Button button, Mob param, bool hovered)
    {
        if (hovered)
		{
			button.Modulate = Godot.Colors.White * 0.5f;
		}
		else
		{
			button.Modulate = Godot.Colors.White;
		}
    }

}
