using Godot;
using System;
namespace Test;

public partial class ProfileSaveTest : Node3D
{
	public override void _Ready()
	{
		base._Ready();

		SaveProfile.ClearFile("test");
		SaveProfile.SetProfile("test");
		SaveProfile.Save();

		SaveProfile.ClearFile("test");
		SaveProfile.Load("test");
		SaveProfile.Save();

	}

}
