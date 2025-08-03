using ChessLike.Entity;
using Godot;
using System;
namespace Test;

public partial class ProfileSaveTest : Node3D
{
	public override void _Ready()
	{
		base._Ready();

		SaveProfile.ClearFile("test");
		SaveProfile.SetSaveIdentifier("PlayerNameSave");
		SaveProfile.SetProfileName("Player Name Here");
		Mob testMob = new(){DisplayedName = "Test Mob",};
		MobTemplateJob wizard = (MobTemplateJob)Global.ManagerMobTemplate.ResourceGet("JobWizard");
		MobTemplateBase def = (MobTemplateBase)Global.ManagerMobTemplate.ResourceGet("Default");
		testMob.TemplateSet(wizard);
		testMob.TemplateSet(def);

		var faction = Global.ManagerFaction.ResourceGet(ChessLike.Entity.EFaction.PLAYER, true);
		faction.Mobs.Add(testMob);
		
		SaveProfile.Save();
		SaveProfile.Load("PlayerNameSave");

		SaveProfile.Load("test");
		SaveProfile.Save();

	}

}
