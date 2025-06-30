using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;
using System;

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

    public void SetBodyModel(EMobBodyModel body)
    {
        MarkerCenterBody.FreeChildren();

        Node3D modelScene;
        switch (body)
        {
            case EMobBodyModel.HUMAN:
                modelScene = Global.ManagerModel.GetInstance("BodyHuman");
                break;

            default: throw new Exception();
        }

        ModelScene = modelScene;
        MarkerCenterBody.AddChild(ModelScene);
    }
}
