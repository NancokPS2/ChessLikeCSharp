using ChessLike.Entity;
using ChessLike.Extension;
using Godot;
using System;

[GlobalClass]
public partial class MobIdentityUI : BaseMobUI
{
	[Export]
	Label NodeName = null!;

	[Export]
	Label NodeFaction = null!;

	[Export]
	Label NodeRace = null!;

	[Export]
	Label NodeJobs = null!;

	[Export]
	Label NodeIdentity = null!;

	protected override void Update(Mob obj)
	{
		base.Update(obj);
		string raceText = (from template in obj.TemplateGet<MobTemplateRace>() select template.TemplateName).ToList().ToStringList(", ");
		string jobText = (from template in obj.TemplateGet<MobTemplateJob>() select template.TemplateName).ToList().ToStringList(", ");
		string titleText = (from template in obj.TemplateGet<MobTemplateIdentity>() select template.TemplateName).ToList().ToStringList(", ");
		NodeName.Text = $"Name: {obj.DisplayedName}";
		NodeFaction.Text = $"Faction: {obj.Faction.ToString()}";
		NodeRace.Text = $"Race: {raceText}";
		NodeJobs.Text = $"Job: {jobText}";
		NodeIdentity.Text = $"Title: {titleText}";
	}
}
