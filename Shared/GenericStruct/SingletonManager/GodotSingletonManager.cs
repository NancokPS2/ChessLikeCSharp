using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using Godot;
using LightInject;

public static class GodotSingletonManager
{
	private static Dictionary<Type, GodotObject> Instances = new();

	public static void AddSingleton<TObject>(TObject obj) where TObject : GodotObject
	{
		if (!GodotObject.IsInstanceValid(obj))
			throw new Exception($"Tried to add invalid instance of type {obj.GetType()}.");



		SanitizeInstances();
		Instances[obj.GetType()] = obj;
	}

	public static TObject? GetSingleton<TObject>(this TObject obj) where TObject : GodotObject
	{
		if (!HasSingleton(obj)) return null;
		return (TObject?)Instances[obj.GetType()];
	}

	private static void SanitizeInstances()
		=> Instances = (
			from kvp
			in Instances
			where GodotObject.IsInstanceValid(kvp.Value)
			select kvp
			).ToDictionary();

	public static bool HasSingleton(this GodotObject obj)
		=>	Instances.TryGetValue(obj.GetType(), out GodotObject? val) && GodotObject.IsInstanceValid(val);
}

public class Singleton
{
	public Singleton(GodotObject obj)
	{
		GodotSingletonManager.AddSingleton(obj);
	}
}
