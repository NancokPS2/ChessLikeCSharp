using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Global
{
    public static class Scenes
    {
        public static MobScene MOB_SCENE { get => GD.Load<PackedScene>("res://Godot/Display/MobDisplay/MobScene/MobScene.tscn").Instantiate<MobScene>(); }


        private static TResource LoadSafe<TResource>(string path) where TResource : notnull, Resource
        {
            TResource res = GD.Load<TResource>(path);
            if (res is not Resource)
            {
                throw new Exception("Could not load a readonly resource!");
            }
            return res;
        }
    }
    
}
