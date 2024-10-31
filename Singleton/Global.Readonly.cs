using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Global
{
    public static class Readonly
    {
        public static readonly int LAYER_CANVAS_COMP = 10;
        public static readonly int LAYER_MSG_QUEUE = 11;
        public static readonly int LAYER_GLOBAL_DRAW = 32;

        public static readonly Godot.Font FONT_SMALL = new Godot.SystemFont();

        public static readonly Godot.ShaderMaterial SHADER_BORDER_CANVAS = GD.Load<ShaderMaterial>("res://assets/Material/UIBorderColorShader.tres");
    }
    
}
