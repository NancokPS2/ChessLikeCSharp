using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using ChessLike.World.Encounter;

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
            SetupParticipant(item.Value, item.Key, true);
        }
    }

    public void SetupComponents()
    {
        //Setup the mob display
        AddChild(CompMobMeshDisplay);
        CompMobMeshDisplay.Name = "MobDisplay";
        //Connect it to the turn manager to handle turn-related display

        //Display grid setup
        AddChild(CompDisplayGrid);
        CompDisplayGrid.SetGrid(CompGrid);
        CompDisplayGrid.Name = "GridDisplay";

        //Camera
        AddChild(CompCamera);
        CompCamera.Name = "Camera";

        //UI for combat inputs and display.
        UI.GetLayer(UI.ELayer.BASE_LAYER).AddChild(CompCombatUI);
        EventBus.ActionSelected += (act) => InputActionSelected = act;
        EventBus.TurnEndRequested += () => InputEndTurnPressed++;

        //ActionRunner connections
        EventBus.MobTurnEnded += CompActionRunner.OnTurnEnded;
        GetTree().ProcessFrame += CompActionRunner.Process;

        //Round counter
        EventBus.RoundEnded += () => RoundsPassed ++;

        DebugDisplay.Add(CompTurnManager);
        DebugDisplay.Add(CompActionRunner);
    }

    public void SetupParticipant(Mob mob, Vector3i where, bool add_to_combat)
    {
        //If this mob was already set up, just update its position and state.
        if(CompMobMeshDisplay.HasMob(mob) && CompTurnManager.GetParticipants().Contains(mob))
        {
            mob.Position = where;
            if(add_to_combat){mob.ChainState(EMobState.COMBAT);}
        }
        else
        {
            mob.Position = where;
            CompMobMeshDisplay.Add(mob);
            CompTurnManager.Add(mob);
            if(add_to_combat){mob.ChainState(EMobState.COMBAT);}

        }
    }

    public void RemoveParticipant(Mob mob)
    {
        mob.Position = Vector3i.INVALID;
        CompMobMeshDisplay.Remove(mob);
        CompTurnManager.Remove(mob);
        mob.MobState = EMobState.BENCHED;
    }
    
}
