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

    private List<Vector3i> MovementStored = new();
    private int MovementCurrentIndex;

    private Dictionary<EMobSceneEffect, Node3D> EffectNodes = new();

    public MobScene()
    {
        EventBus.MobMoved += OnMobMoved;
        EventBus.MobStatChanged += OnMobStatChanged;
        EventBus.MobTurnStarted += OnMobTurnStarted;
        EventBus.MobTurnEnded += OnMobTurnEnded;
        EventBus.MobSelected += OnMobSelected;
    }

    public override void _Ready()
    {
        base._Ready();
        if (MobUsing is null) throw new Exception("Lacks a MobUsing");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        MovementProcess();
    }

    #region Particles

    public void AnimatePopupText(string text, Godot.Color? color = null, Godot.Gradient? gradient = null)
    {
        PopupText3D popupText = Readonly.Scenes.SCENE_PARTICLE_POPUP_TEXT;
        popupText.Text = text;
        popupText.Color = color ?? Colors.White;
        popupText.ColorRamp = gradient;
        popupText.Finished += popupText.QueueFree;

        GetTree().Root.AddChild(popupText);

        popupText.GlobalPosition = MarkerOverhead.GlobalPosition;
        popupText.Emitting = true;
    }

    public void ToggleEffect(EMobSceneEffect effect, bool enabled)
    {
        Node3D? effectNode = EffectNodes.ContainsKey(effect) ? EffectNodes[effect] : null;
        Node3D parentNode = MarkerCenterBody;
        if (enabled)
        {
            effectNode?.QueueFree();
            effectNode = effect switch
            {
                EMobSceneEffect.TURN_ACTIVE
                    => Global.ManagerParticle
                    .ResourceGet("HoveringStar")
                    .Instantiate<Node3D>(),

                EMobSceneEffect.TARGETED 
                    => Global.ManagerParticle
                    .ResourceGet("InwardArrows")
                    .Instantiate<Node3D>(),
                    
                _
                    => throw new Exception("Invalid effect.")
            };

            parentNode = effect switch
            {
                EMobSceneEffect.TURN_ACTIVE => MarkerOverhead,
                EMobSceneEffect.TARGETED => MarkerCenterBody,
                _ => MarkerOverhead

            };

            parentNode.AddChild(effectNode);
            EffectNodes[effect] = effectNode;
        }
        else
        {
            effectNode?.QueueFree();
            EffectNodes.Remove(effect);
        }
    }


    #endregion

    #region Model
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

    #endregion

    #region Movement
    public void MovementProcess()
    {
        //If empty, skip.
        if (MovementStored.Count == 0) return;

        //Reached the end of the list, we are done.
        if (MovementCurrentIndex >= MovementStored.Count)
        {
            MovementCurrentIndex = 0;
            MovementStored.Clear();
            MovementVerifyPosition();
            return;
        }

        Vector3i currentGoal = MovementStored[MovementCurrentIndex];

        Godot.Vector3 currentGoalGlobal;
        switch (MobUsing.MovementMode)
        {
            case EMobMovementMode.WALK:
                currentGoalGlobal = CombatScene.GetGridNode().MapToGlobal(currentGoal);
                break;

            default:
                throw new NotImplementedException($"Movement for {MobUsing.MovementMode} not implemented yet.");
        }

        GlobalPosition = GlobalPosition.MoveToward(currentGoalGlobal, MovementGetSpeed());

        //If close enough, advance the index.
        if (Mathf.IsZeroApprox(GlobalPosition.DistanceTo(currentGoalGlobal)))
            MovementCurrentIndex++;
    }

    private float MovementGetSpeed()
    {
        float agility = Mathf.Clamp(MobUsing.Stats.GetStat(EStatName.AGILITY), 0, 200);
        return 2f * (agility / 100);
    }

    public void MovementResetPosition()
    {
        Godot.Vector3 vector = CombatScene.GetGridNode().MapToGlobal(MobUsing.GetPosition());
        GlobalPosition = vector;
    }

    public void MovementVerifyPosition()
    {
        if (!Mathf.IsZeroApprox(CombatScene.GetGridNode().MapToGlobal(MobUsing.GetPosition()).DistanceTo(GlobalPosition)))
        {
            throw new Exception($"Position mismatch. Node:{GlobalPosition} | Mob:{MobUsing.GetPosition()}");
        }
    }
    #endregion

    #region Event Handling
    private void OnMobMoved(Mob mob, Vector3i from, Vector3i to)
    {
        if (mob != MobUsing) return;
        MovementStored.Add(to);
    }

    private void OnMobStatChanged(Mob mob, EStatName stat, float change)
    {
        if (mob != MobUsing) return;
        string text;
        Godot.Color color = Colors.White;
        if (change < 0)
        {
            color = Colors.Red;
        }
        else if (change > 0)
        {
            color = Colors.Green;
        }

        switch (stat)
        {
            case EStatName.HEALTH:
                text = change.ToString();
                break;

            default: break;
        }
        AnimatePopupText($"{stat}: {change}", color);
    }

    private void OnMobTurnStarted(Mob mob)
    {
        if (mob != MobUsing) return;
        AnimatePopupText("READY");
        ToggleEffect(EMobSceneEffect.TURN_ACTIVE, true);
    }

    private void OnMobTurnEnded(Mob mob)
    {
        if (mob != MobUsing) return;
        ToggleEffect(EMobSceneEffect.TURN_ACTIVE, false);
    }

    private void OnMobSelected(Mob mob)
    {
        if (mob != MobUsing) return;
        
    }
    #endregion

}
