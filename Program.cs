using ChessLike.Shared;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using System.Threading;

namespace ChessLike;

public class TESTING
{
	
	public static void MainOld()
	{
		TestActions();
		TestSerializing();
		TestResources();
		TestFilesystem();
		TestGlobal();
		TestDraw();

	}

	public static void TestActions()
	{
		//Create mob
		Entity.Mob mob = new();
		
		//Create action - TODO


		//Create parameters for action - TODO

		//dam.Use(test_params); - TODO
		Console.WriteLine(mob.Stats.GetValue(StatSet.Name.HEALTH));
		Console.WriteLine(mob.relation_list);

	}

	public static void TestSerializing()
	{

		Entity.Mob mob = new();
		Global.resources.SaveObject(
			"mob",
			mob,
			ResourceDictionary.FileSource.USER_CONTENT);
		Console.WriteLine("Saved file");
		Console.WriteLine(Global.Directory.User);

		Entity.Mob? same_mob = (Entity.Mob?)Global.resources.LoadObject(
			"mob",
			ResourceDictionary.Key.ENTITY,
			ResourceDictionary.FileSource.USER_CONTENT);

		Console.WriteLine(same_mob);
	}

	public static void TestGlobal()
	{
		Global.variables.player_profile = "TEST";
		Console.WriteLine(Global.variables.player_profile);
	}

	public static void TestFilesystem()
	{
		Console.WriteLine(
			Directory.GetFiles(Environment.CurrentDirectory)
		);
		
	}

	public static void TestResources()
	{
		
	}

	public static void TestDraw()
	{
		World.Grid grid = World.Grid.Generator.GenerateFlat(new Vector3i(10));

	}
}
