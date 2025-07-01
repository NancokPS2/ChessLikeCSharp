using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class MobScene : Node3D
{
    public Mob MobUsing;

    protected PackedScene FloatingIconScene = GD.Load<PackedScene>("uid://bmm3h2202bdkq");

    public StatusEffectIcon StatusEffectIcon { get => statusEffectIcon ?? throw new Exception(); set => statusEffectIcon = value; }
    [Export]
    private StatusEffectIcon? statusEffectIcon;

    public Node3D MarkerOverhead { get => markerOverhead ?? throw new Exception(); set => markerOverhead = value; }
    [Export]
    private Node3D? markerOverhead;

    public Node3D MarkerCenterBody { get => markerCenterBody ?? throw new Exception(); set => markerCenterBody = value; }
    [Export]
    private Node3D? markerCenterBody;

    public Node3D MarkerBase { get => markerBase ?? throw new Exception(); set => markerBase = value; }
    [Export]
    private Node3D? markerBase;

    private Node3D? ModelScene;
    private AnimationPlayer? AnimationPlayer;

    public MobScene()
    {
        EventBus.MobFinishedPathMove += OnMobFinishedPathMove;
    }
    public override void _Ready()
    {
        base._Ready();
        if (MobUsing is null) throw new Exception("Lacks a MobUsing");
    }
    public void SetBodyModel(EMobSceneBodyModel body)
    {
        MarkerCenterBody.FreeChildren();

        Node3D modelScene;
        switch (body)
        {
            case EMobSceneBodyModel.HUMAN:
                modelScene = Global.ManagerModel.GetInstance("BodyHuman");
                break;

            default: throw new Exception();
        }

        ModelScene = modelScene;
        MarkerCenterBody.AddChild(ModelScene);
    }

    public void AnimateMovement(List<Vector3i> path, EMobMovementMode movementType)
    {
        Tween tween = CreateTween().Chain();
        switch (movementType)
        {
            case EMobMovementMode.WALK:
                AnimationPlayer?.Play("Walk");

                foreach (var point in path)
                {
                    tween.TweenProperty(this, "position", point.ToGVector3() * GetGridCellSize(), GetMovementDuration());
                }
                break;

            default: throw new Exception();
        }
    }

    #region Private
    private float GetMovementDuration()
    {
        float agility = Mathf.Clamp(MobUsing.Stats.GetValue(EStatName.AGILITY), 0, 200);
        float agilityReduction = agility / 500;
        Debug.Assert(agilityReduction < 0.4 && agilityReduction > 0);
        return 0.5f - agilityReduction;
    }
    
    [Obsolete("get actual measurements")]
    private Godot.Vector3 GetGridCellSize() => new(1, 1, 1);
    #endregion

    #region Event Connection
    private void OnMobFinishedPathMove(Mob mob, List<Vector3i> path)
    {
        if (mob != MobUsing) return;

        AnimateMovement(path, MobUsing.MovementMode);
    }
    #endregion

}
