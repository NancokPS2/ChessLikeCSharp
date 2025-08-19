using ChessLike.Entity;
using Godot;
using System;

[GlobalClass]
public partial class MobIdentityUI : BaseMobUI
{
	[Export]
	Label? NodeName;

	[Export]
	Label? NodeFaction;

	protected override void Update(Mob obj)
	{
		base.Update(obj);
		NodeName.Text = $"Name: {obj.DisplayedName}";
		NodeFaction.Text = $"Faction: {obj.Faction.ToString()}";
	}
}
