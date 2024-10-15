using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
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
        AddChild(CompMobMeshDisplay);
        CompMobMeshDisplay.Name = "MobDisplay";
        //Connect it to the turn manager to handle turn-related display
        CompMobMeshDisplay.ConnectToManager(CompTurnManager);

        AddChild(CompDisplayGrid);
        CompDisplayGrid.SetGrid(CompGrid);
        CompDisplayGrid.Name = "GridDisplay";

        AddChild(CompCamera);
        CompCamera.Name = "Camera";

        AddChild(CompCombatUI);
        CompCombatUI.NodeActionUI.ActionPressed += (act) => InputActionSelected = act;
        CompCombatUI.NodeActionUI.EndTurnPressed += () => InputEndTurnPressed++;

        AddChild(CompCanvas);
        CompCanvas.Layer = Global.Readonly.LAYER_CANVAS_COMP;

        GetTree().ProcessFrame += () => CompActionRunner.Process((float)GetProcessDeltaTime());

        DebugDisplay.Instance.Add(CompTurnManager);
        DebugDisplay.Instance.Add(CompActionRunner);
    }

    public void SetupParticipant(Mob mob, Vector3i where)
    {
        mob.Position = where;
        CompMobMeshDisplay.Add(mob);
        CompTurnManager.Add(mob);
    }
    
}
