using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobModel : Resource
{
	[Export]
	protected PackedScene Model = null!;

	[Export]
	protected NodePath TorsoBone = "Armature/Skeleton3D/TORSO";

	[Export]
	protected NodePath HandBoneL = "Armature/Skeleton3D/HAND_L";

	[Export]
	protected NodePath HandBoneR = "Armature/Skeleton3D/HAND_R";

	[Export]
	protected NodePath HeadBone = "Armature/Skeleton3D/HEAD";

	public Node3D GetModel()
		=> Model.Instantiate<Node3D>();

	public Node3D[] GetBones(Node modelRoot, EMobModelBone bone)
		=> bone switch
		{
			EMobModelBone.HEAD => [modelRoot.GetNode<Node3D>(HeadBone)],
			EMobModelBone.HAND => [modelRoot.GetNode<Node3D>(HandBoneL), modelRoot.GetNode<Node3D>(HandBoneR)],
			EMobModelBone.TORSO => [modelRoot.GetNode<Node3D>(TorsoBone)],
			_ => throw new NotImplementedException(),
		};
}
