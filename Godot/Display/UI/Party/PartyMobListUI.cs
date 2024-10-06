using ChessLike.Entity;
using Godot;
using System;

public partial class PartyMobListUI : Control
{
	[Export]
	Control? MobContainer;

    public override void _Ready()
    {
        base._Ready();
		MobContainer ??= (Control)FindChild("PortraitContainer");
    }

    public void Update(EFaction faction)
	{
		var list = Global.ManagerMob.GetInFaction(faction);
	}
}
