using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public interface ISingleton<TObject> where TObject : GodotObject
{
	public static abstract TObject? Instance { protected set; get; }
}
