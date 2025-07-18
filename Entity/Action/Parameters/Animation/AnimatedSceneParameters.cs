using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AnimatedSceneParameters : Resource
{
	public enum EMotion
	{
		STILL,
		MOVE_TO_TARGET,
		RISE_AND_FALL,
        SPIN,
		FACE_TRAJECTORY,
	}
	public enum ESpawn
	{
		SPAWN_AT_OWNER,
		SPAWN_AT_TARGET,
	}

	[ExportCategory("Scene Spawn")]
	public PackedScene Scene;
	[Export]
	private PackedScene scene { set => Scene = value; get => Scene; }

	public int SpawnCount = 1;
	[Export]
	private int spawnCount { set => SpawnCount = value; get => SpawnCount; }

	public float Duration = 1;
	[Export]
	private float duration { set => Duration = value; get => Duration; }

	public ESpawn SpawnMode = new();
	[Export]
	private ESpawn spawnMode
	{
		set => SpawnMode = value;
		get => SpawnMode;
	}

	public List<EMotion> MotionMode = new();
	[Export]
	private Godot.Collections.Array<EMotion> motionMode
	{
		set => MotionMode = new(value);
		get => new(MotionMode);
	}

	public bool FreeAfterDuration = true;
	[Export]
	private bool freeAfterDuration { set => FreeAfterDuration = value; get => FreeAfterDuration; }
}
