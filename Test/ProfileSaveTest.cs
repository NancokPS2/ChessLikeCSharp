using ChessLike.Entity;
using Godot;
using System;
namespace Test;

public partial class ProfileSaveTest : Node3D
{
	public override void _Ready()
	{
		base._Ready();

		SaveFile.DeleteSave("testFirstLoad");
		SaveFile.DeleteSave("testSave");
		SaveFile.DeleteSave("testSaveAndLoad");

		Mob testMob = new(){DisplayedName = "Test Mob",};
		MobTemplateJob wizard = (MobTemplateJob)Global.ManagerMobTemplate.ResourceGet("JobWizard");
		MobTemplateBase def = (MobTemplateBase)Global.ManagerMobTemplate.ResourceGet("Default");
		testMob.TemplateSet(wizard);
		testMob.TemplateSet(def);

		var faction = Global.ManagerFaction.ResourceGet(ChessLike.Entity.EFaction.PLAYER, true);
		faction.Mobs.Add(testMob);

		var newSave = SaveManager.NewSave("testSaveAndLoad", 0);
		newSave.PlayerFaction = faction;
		SaveManager.Save(newSave, 0, true);
		SaveManager.Load("testSaveAndLoad", 0);

		SaveManager.Load("testFirstLoad", 0);

	}

}
