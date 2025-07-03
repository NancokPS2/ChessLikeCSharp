using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobSceneManager : Node3D
{
    public List<MobScene> InstancedMobs = new();
    public MobSceneManager()
    {
        EventBus.MobStateChanged += OnMobStatChanged;
        EventBus.CellSelected += OnCellSelected;
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


    #region Event Connection
    private void OnMobStatChanged(Mob mob, EMobState state)
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
        foreach (var item in InstancedMobs)
        {
            if (item.MobUsing.GetPosition() == cellPos)
            {
                EventBus.MobSelected?.Invoke(item.MobUsing);
                return;
            }            
        }
    }
    #endregion
}
