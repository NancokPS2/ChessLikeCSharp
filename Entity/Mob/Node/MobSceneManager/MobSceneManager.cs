using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobSceneManager : Node3D
{
    [Export]
    protected float CursorSpeed = 15;
    protected Node3D NodeSelectionCursor = GD.Load<PackedScene>("uid://4cikkiw1mfd").Instantiate<Node3D>();
    protected Node3D NodeHoveringCursor = GD.Load<PackedScene>("uid://cu1nlfq5x61rn").Instantiate<Node3D>();

    protected List<MobScene> InstancedMobs = new();

    protected MobScene? SelectedMobScene;
    public MobSceneManager()
    {
        EventBus.MobStateChanged += OnMobStateChanged;
        EventBus.CellPositionSelected += OnCellSelected;
        EventBus.CellPositionHovered += OnCellHovered;
        EventBus.MobTurnStarted += OnMobTurnStarted;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Godot.Vector3 target = SelectedMobScene?.GlobalPosition ?? Godot.Vector3.Zero;;

        NodeSelectionCursor.Visible = SelectedMobScene is not null;

        NodeSelectionCursor.GlobalPosition = NodeSelectionCursor.GlobalPosition.MoveToward(
            target, (float)(CursorSpeed * delta)
            );
    }


    public override void _Ready()
    {
        base._Ready();
        AddChild(NodeSelectionCursor);
        AddChild(NodeHoveringCursor);
    }


    private bool HasInstance(Mob mob)
        => InstancedMobs.Any(x => x.MobUsing == mob);

    private MobScene GetInstance(Mob mob)
    {
        List<MobScene> instancesFound = InstancedMobs.FindAll(x => x.MobUsing == mob);
        if (instancesFound.Count > 1)
        {
            throw new Exception($"Only one instance should exist. Found {instancesFound.Count}");
        }
        else if (instancesFound.Count == 0)
        {
            MobScene newInstance = Readonly.Scenes.SCENE_MOB;
            newInstance.MobUsing = mob;
            return newInstance;
        }
        else return instancesFound[0];
    }

    private void AddInstance(Mob mob)
    {
        MobScene instance = GetInstance(mob);

        InstancedMobs.Add(instance);

        AddChild(instance);
        instance.MovementResetPosition();
    }

    public void RemoveInstance(Mob mob)
    {
        if (!HasInstance(mob)) return;
        MobScene instance = GetInstance(mob);

        RemoveChild(instance);
        InstancedMobs.Remove(instance);
    }

    public MobScene? GetInstanceByPosition(Vector3i cellPos)
    {
        foreach (var instance in InstancedMobs)
        {
            if (instance.MobUsing.GetPosition() == cellPos)
            {
                return instance;
            }
        }
        return null;
    }

    protected void SelectMob(MobScene scene)
        => SelectMob(scene.MobUsing);

    protected void SelectMob(Mob mob)
    {
        MobScene? scene = GetInstanceByPosition(mob.GetPosition());
        if (scene is null) return;

        SelectedMobScene = GetInstance(mob);
        EventBus.MobSelected?.Invoke(scene.MobUsing);
    }

    protected void HoverMob(MobScene scene)
        => HoverMob(scene.MobUsing);
    protected void HoverMob(Mob mob)
    {
        MobScene? scene = GetInstanceByPosition(mob.GetPosition());
        if (scene is null) return;

        NodeHoveringCursor.GlobalPosition = scene.MarkerOverhead.GlobalPosition;
        NodeHoveringCursor.Show();
        EventBus.MobHovered?.Invoke(scene.MobUsing);
    }

    private void ThrowOnMissingInstance(Mob mob)
    {
        if (!HasInstance(mob)) throw new Exception();
    }

    #region Event Connection
    private void OnMobStateChanged(Mob mob, EMobState state)
    {
        if (state == EMobState.COMBAT)
        {
            //Has an instance, skip and keep using that.
            if (HasInstance(mob)) return;
            //Add an instance for this mob.
            AddInstance(mob);
        }
        else if (state == EMobState.BENCHED)
        {
            //Does not have an instance already, skip.
            if (!HasInstance(mob)) return;
            //Has an instance, remove it.
            else RemoveInstance(mob);
        }
    }

    private void OnCellSelected(Vector3i cellPos)
    {
        //Do not select anything if in targeting state.
        if (CombatScene.GetState() == EBattleState.TARGETING) return;

        //Try to find an instance.
        MobScene? scene = GetInstanceByPosition(cellPos);
        if (scene is null) return;

        SelectMob(scene);
    }

    private void OnCellHovered(Vector3i cellPos)
    {
        //Try to find an instance.
        MobScene? scene = GetInstanceByPosition(cellPos);
        if (scene is not null)
        {
            HoverMob(scene);
        }
        else
        {
            NodeHoveringCursor.Hide();
        }

    }

    private void OnMobTurnStarted(Mob mob)
    {
        ThrowOnMissingInstance(mob);

        SelectMob(mob);
    }
    #endregion
}
