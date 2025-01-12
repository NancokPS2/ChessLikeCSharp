using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Global
{
    public static class Readonly
    {


        //FONTS
        public static readonly Godot.Font FONT_SMALL = new Godot.SystemFont();
        public static readonly Godot.Font FONT_HEADING = LoadSafe<Godot.Font>("res://assets/Theme/Font/HeadingFont.tres");

        //SHADERS
        public static readonly Godot.ShaderMaterial SHADER_BORDER_CANVAS = LoadSafe<ShaderMaterial>("res://assets/Material/UIBorderColorShader.tres");

        //PARTICLE SCENES
        public static readonly Godot.PackedScene PARTICLE_SCENE_SHINE = LoadSafe<PackedScene>("res://Godot/Particle/ShineHalo.tscn");
        public static readonly Godot.PackedScene PARTICLE_SCENE_EXPLOSION = LoadSafe<PackedScene>("res://Godot/Particle/Explosion.tscn");
        public static readonly Godot.PackedScene PARTICLE_SCENE_POPUP_TEXT = LoadSafe<PackedScene>("res://Godot/Particle/PopupText.tscn");

        //CONTROL SCENES
        


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
