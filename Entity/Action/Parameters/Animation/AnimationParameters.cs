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

	public List<AnimatedSceneParameters> SceneSpawns = new();
    [Export]
    private Godot.Collections.Array<AnimatedSceneParameters> sceneSpawns
    {
        set => SceneSpawns = new(value);
        get => new(SceneSpawns);
    }

    public List<EActionAnimationFlags> AnimationFlags = new();
    [Export]
    private Godot.Collections.Array<EActionAnimationFlags> animationFlags
    {
        set => AnimationFlags = new(value);
        get => new(AnimationFlags);
    }
}
