using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AnimationParameters : Resource
{
    public enum ESceneAnimationMode
    {
        SPAWN_AT_OWNER,
        SPAWN_AT_TARGET,
        MOVE_TO_TARGET
    }
    [Export]
    public float MaxDuration = 2f;

    public Dictionary<ESceneAnimationMode, PackedScene> ScenesToSpawn = new();
    [Export]
    private Godot.Collections.Dictionary<ESceneAnimationMode, PackedScene> scenesToSpawn
    {
        set => ScenesToSpawn = new(value);
        get => new(ScenesToSpawn);
    }

    public List<EActionAnimationFlags> AnimationFlags = new();
    [Export]
    private Godot.Collections.Array<EActionAnimationFlags> animationFlags
    {
        set => AnimationFlags = new(value);
        get => new(AnimationFlags);
    }

}
