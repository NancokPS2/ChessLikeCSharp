using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.World;
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
        EventBus.MobStatChanged += OnMobStatChanged;
        EventBus.MobTurnStarted += OnMobTurnStarted;
    }

    public override void _Ready()
    {
        base._Ready();
        if (MobUsing is null) throw new Exception("Lacks a MobUsing");
    }

    public void AnimatePopupText(string text, Godot.Color? color = null, Godot.Gradient? gradient = null)
    {
        PopupText3D popupText = Readonly.Scenes.SCENE_PARTICLE_POPUP_TEXT;
        popupText.SetText(text);
        popupText.Color = color ?? Colors.White;
        popupText.ColorRamp = gradient;
        popupText.Emitting = true;
        popupText.Finished += popupText.QueueFree;

        GetTree().Root.AddChild(popupText);

        popupText.GlobalPosition = GlobalPosition;
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
                    tween.TweenProperty(
                        this,
                        "position",
                        point.ToGVector3() * GetGridCellSize(),
                        GetMovementDuration()
                        );
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

    public void UpdatePosition()
    {
        Godot.Vector3 vector = Grid.MapToReal(MobUsing.GetPosition());
        GlobalPosition = vector;
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

    private void OnMobStatChanged(Mob mob, EStatName stat, float new_value)
    {
        if (mob != MobUsing) return;
        AnimatePopupText($"{stat}: {new_value}", Colors.Red);
    }

    private void OnMobTurnStarted(Mob mob)
    {
        if (mob != MobUsing) return;
        AnimatePopupText("READY");
    }
    #endregion

}
