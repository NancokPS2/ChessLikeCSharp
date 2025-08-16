using ChessLike.World.Encounter;
using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class TravelMapLocation : Resource
{
	[Export]
	public string DisplayName = "???";

	[Export]
	public EncounterData? CombatEncounter;
}