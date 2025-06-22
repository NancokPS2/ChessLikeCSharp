using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AnimationParameters : Resource
{
    public float Duration = 1f;

    [Export]
    public Texture2D? FloatingTexture;
}
