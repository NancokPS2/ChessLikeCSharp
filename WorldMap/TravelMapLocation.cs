using ChessLike.Extension;
using ChessLike.WorldMap;
using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class TravelMapLocation : Resource
{
	[Export]
	public string DisplayName = "???";

	[Export]
	public EncounterData? CombatEncounter;

	[Export]
	public Godot.Collections.Array<EStoryFlag> FlagWhitelist = new();

	[Export]
	public Godot.Collections.Array<EStoryFlag> FlagBlacklist = new();

}