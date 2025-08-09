using ChessLike.Entity;
using Godot;
using System;
namespace Test;

public partial class ProfileSaveTest : Node3D
{
	private const string ProfileNameLoadFirst = "testFirstLoad";
	private const string ProfileNameRegularSaveLoad = "testSave";


	public override void _Ready()
	{
		base._Ready();

		SaveManager.DeleteSave(ProfileNameLoadFirst, 0);
		SaveManager.DeleteSave(ProfileNameRegularSaveLoad, 0);

		SaveManager.NewSave(ProfileNameLoadFirst, 0);

		//Create a new Mob
		Mob testMob = new() { DisplayedName = "Test Mob", };
		MobTemplateJob wizard = (MobTemplateJob)Global.ManagerMobTemplate.ResourceGet("JobWizard");
		MobTemplateBase def = (MobTemplateBase)Global.ManagerMobTemplate.ResourceGet("Default");
		testMob.TemplateSet(wizard);
		testMob.TemplateSet(def);

		//Take the player faction and store a version that has this new Mob
		var faction = Global.ManagerFaction.ResourceGet(ChessLike.Entity.EFaction.PLAYER, true);
		faction.Mobs.Add(testMob);

		//Create the new save file.
		var newSave = new SaveFile(ProfileNameRegularSaveLoad);
		newSave.PlayerFaction = faction;

		//Save it.
		SaveManager.SetCurrentSave(newSave, 0);
		SaveManager.Save(true);
		SaveManager.ReloadSave();


	}

}
