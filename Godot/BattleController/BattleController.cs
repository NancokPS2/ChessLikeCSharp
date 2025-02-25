

using ChessLike.Entity;
using ChessLike.Shared.Storage;
using ChessLike.Turn;
using ChessLike.World;
using ChessLike.World.Encounter;
using Godot.Display;

using Action = ChessLike.Entity.Action;

namespace Godot;

[GlobalClass]
public partial class BattleController : Node, IDebugDisplay
{
	public static BattleController Instance;
	private bool _ready_for_debug;

	public override void _Ready()
	{
		base._Ready();
		Instance = this;
		DebugDisplay.Add(this);

		FSMSetup();
		SetupEncounter(EncounterData.GetDefault());

		SetDeferred("_ready_for_debug", true);

	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Encounter.EncounterProcess();

		FSMProcess(delta);

		Testing();

	}

	public void Testing()
	{
/* 
		if (Input.IsActionJustPressed("debug_draw"))
		{
			Mob mob = Global.ManagerMob.GetAllPooled()[0];

			mob.Stats.SetValue(StatName.HEALTH, mob.Stats.GetValue(StatName.HEALTH) + 5);
		} */
	}

	public string GetText()
	{
		if (!_ready_for_debug){return "";}

		string output = string.Format(
			"State: {0}" + "\n" +
			"Action selected: {1}" + "\n" +
			"Grid size: {2}" + "\n" +
			"Mob taking turn: {3}" + "\n" +
			"Location selected: {4}" + "\n" + 
			"Camera rotation: {5}" + "\n" +
			"Usage parameters: {6}" + "\n" 
			,
			new object[]{
				StateCurrent is not null ? StateCurrent.StateIdentifier : "null", 
				ActionSelected is not null ? ActionSelected.Name : "null", 
				CompGrid != null ? CompGrid.Boundary : "null",
				CompTurnManager.GetCurrentTurnTaker() as Mob is Mob mob ? mob.DisplayedName : "null",
				PositionHovered,
				CompCamera != null ? CompCamera.Rotation : "???",
				TurnUsageParameters is not null ? TurnUsageParameters.PositionsTargeted.ToArray().ToString()  ?? throw new Exception() : "???",
				}
			
		);
		return output;
	}

	#region Setup
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

        EventBus.EncounterLoaded?.Invoke(to_load);
    }

    public void SetupComponents()
    {
        //Setup the mob display
        AddChild(CompMobMeshDisplay);
        CompMobMeshDisplay.Name = "MobDisplay";
        //Connect it to the turn manager to handle turn-related display

        //Display grid setup
        AddChild(CompDisplayGrid);
        CompDisplayGrid.Name = "GridDisplay";

        //Camera
        AddChild(CompCamera);
        CompCamera.Name = "Camera";

        //UI for combat inputs and display.
        UI.GetLayer(UI.ELayer.BASE_LAYER).AddChild(CompCombatUI);
        GetTree().ProcessFrame += CompActionRunner.Process;

        EventBus.InputActionSelected += (act) => ActionSelected = act;

        DebugDisplay.Add(CompTurnManager);
        DebugDisplay.Add(CompActionRunner);
    }

    public void SetupParticipant(Mob mob, Vector3i where, bool add_to_combat)
    {
        //If this mob was already set up, just update its position and state.
        if(CompMobMeshDisplay.HasMob(mob) && CompTurnManager.GetParticipants().Contains(mob))
        {
            mob.Position = where;
            if(add_to_combat){mob.MobState = EMobState.COMBAT;}
        }
        else
        {
            mob.Position = where;
            CompMobMeshDisplay.Add(mob);
            CompTurnManager.Add(mob);
            if(add_to_combat){mob.MobState = EMobState.COMBAT;}

        }
    }

    public void RemoveParticipant(Mob mob)
    {
        mob.Position = Vector3i.INVALID;
        CompMobMeshDisplay.Remove(mob);
        CompTurnManager.Remove(mob);
        mob.MobState = EMobState.BENCHED;
    }
    #endregion

}
