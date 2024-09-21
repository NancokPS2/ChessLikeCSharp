using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.DebugDisplay;
using ChessLike.Turn;

namespace Godot;

public partial class BattleController
{
    public void SetupEncounter(EncounterData to_load)
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        CompGrid = to_load.Grid;
        Encounter = to_load;

        SetupComponents();
        
        //Add the mobs after setting their position.
        foreach (var item in Encounter.PresetMobSpawns)
        {
            SetupParticipant(item.Value, item.Key);
        }
    }

    public void SetupComponents()
    {
        //Setup the mob display
        AddChild(CompDisplayMob);
        CompDisplayMob.Name = "MobDisplay";
        //Connect it to the turn manager to handle turn-related display
        CompDisplayMob.ConnectToManager(CompTurnManager);
        //Connect inputs from the action menu.
        CompDisplayMob.MobUINode.ActionPressed += (act) => InputActionSelected = act;
        CompDisplayMob.MobUINode.EndTurnPressed += () => InputEndTurnPressed++;

        AddChild(CompDisplayGrid);
        CompDisplayGrid.SetGrid(CompGrid);
        CompDisplayGrid.Name = "GridDisplay";

        AddChild(CompCamera);
        CompCamera.Name = "Camera";

        DebugDisplay.Instance.Add(CompTurnManager);
    }

    public void SetupParticipant(Mob mob, Vector3i where)
    {
        mob.Position = where;
        CompDisplayMob.Add(mob);
        CompTurnManager.Add(mob);
    }
    
}
