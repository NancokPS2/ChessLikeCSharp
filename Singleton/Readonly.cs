using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Readonly
{
    public static class Scenes
    {
        public static MobScene SCENE_MOB
        {
            get => GD.Load<PackedScene>("uid://k16lil2fu57n").Instantiate<MobScene>();
        }

        public static PopupText3D SCENE_PARTICLE_POPUP_TEXT
        {
            get => GD.Load<PackedScene>("uid://c6lxdpe373yfb").Instantiate<PopupText3D>();
        }
    }
    
}
