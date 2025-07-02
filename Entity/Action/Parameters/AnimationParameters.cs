using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AnimationParameters : Resource
{
    [Export]
    public float MaxDuration = 2f;

    [Export]
    public Texture2D? FloatingTexture;

    [Export]
    public string FloatingText = "";

    [Export]
    public PackedScene? ParticleScene;

    public List<EActionAnimationFlags> AnimationFlags = new();
    [Export]
    private Godot.Collections.Array<EActionAnimationFlags> animationFlags
    {
        set => AnimationFlags = new(value);
        get => new(AnimationFlags);
    }

}
