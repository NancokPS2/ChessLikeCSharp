using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;
using ChessLike.World;
using ChessLike.WorldMap;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;
using Vector3 = Godot.Vector3;

namespace ChessLike.Entity;
//public Navigation navigation = new();

public partial class MobNavigation : Node3D
{
	Grid GridUsed;

	public Dictionary<Mob, MobAStar> AStars = new();

	public override void _Ready()
	{
		base._Ready();
		EventBus.EncounterLoaded += OnEncounterLoaded;
		EventBus.MobStateChanged += OnMobStateChanged;
	}

	#region Event Handling
	private void OnEncounterLoaded(EncounterData obj)
	{
		GridUsed = CombatScene.GetGrid();
	}

	private void OnMobStateChanged(Mob mob, EMobState state)
	{
		if (state != EMobState.COMBAT) return;
		MobAStar newAStar = new();
		newAStar.Generate(GridUsed, mob);
		MobAStars[mob] = newAStar;
	}
	#endregion
}

