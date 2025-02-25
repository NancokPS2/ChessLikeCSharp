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
        public static readonly PackedScene SCENE_UI_PAUSE = GD.Load<PackedScene>("res://Godot/Display/UI/Pause.tscn");

        //public static readonly PackedScene SCENE_UI_MOB_GENERAL = GD.Load<PackedScene>();
        public static readonly PackedScene SCENE_UI_MOB_ACTION = GD.Load<PackedScene>("res://Godot/Display/UI/Mob/MobEquipmentUI.tscn");
        //public static readonly PackedScene SCENE_UI_MOB_STATS = GD.Load<PackedScene>();

        public static readonly PackedScene SCENE_UI_PARTY_GENERAL = GD.Load<PackedScene>("res://Godot/Display/UI/Party/PartyGeneralUI.tscn");
        public static readonly PackedScene SCENE_UI_PARTY_JOB_CHANGE = GD.Load<PackedScene>("res://Godot/Display/UI/Party/PartyJobChangeUI.tscn");
        public static readonly PackedScene SCENE_UI_PARTY_MOB_LIST = GD.Load<PackedScene>("res://Godot/Display/UI/Party/PartyMobListUI.tscn");

        public static readonly PackedScene SCENE_UI_COMBAT_GENERAL = GD.Load<PackedScene>("res://Godot/Display/UI/Combat/CombatGeneralUI.tscn");
        public static readonly PackedScene SCENE_UI_COMBAT_ACTION = GD.Load<PackedScene>("res://Godot/Display/UI/Combat/CombatActionUI.tscn");
        public static readonly PackedScene SCENE_UI_COMBAT_TURN = GD.Load<PackedScene>("res://Godot/Display/UI/Combat/CombatTurnUI.tscn");

        public static readonly PackedScene SCENE_POPUP_BUTTON_DIALOG = GD.Load<PackedScene>("res://Godot/Display/PackedScene/PopupButtonDialogUI.tscn");


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
