using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

namespace ChessLike.World;

[GlobalClass, Tool]
public partial class GridCell : Resource, IEquatable<GridCell>
{

    [Export]
    public string Name = "UNNAMED";

    public List<ECellFlag> Flags = new List<ECellFlag>();
    [Export]
    private Godot.Collections.Array<ECellFlag> flags
    {
        set => Flags = new(value);
        get => new(Flags);
    }

    [Export]
    public bool Selectable = false;

    [Export]
    public EFaction FactionSpawn = EFaction.INVALID;

    public GridCell()
    {
        Name = "";
        Flags = new List<ECellFlag>();
    }

    public GridCell(string name, List<ECellFlag> flags, bool selectable)
    {
        Name = name;
        Flags = flags;
        Selectable = selectable;
    }

    public bool Equals(GridCell? other)
    {
        return Name == other?.Name && Flags == other.Flags && Selectable == other.Selectable && FactionSpawn == other.FactionSpawn;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(GridCell a, GridCell b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(GridCell a, GridCell b)
    {
        return !a.Equals(b);
    }
    
	public override bool Equals(object? obj)
	{
		return Equals(obj as GridCell);
	}

    public static class Preset
    {
        public static readonly GridCell Air = new()
        {
            Name = "Air",
            Flags = new List<ECellFlag>() { ECellFlag.AIR },
            Selectable = false,
        };
        public static readonly GridCell Floor = new()
        {
            Name = "Floor",
            Flags = new List<ECellFlag>() { ECellFlag.SOLID },
            Selectable = true,
        };
        public static readonly GridCell Spawnpoint = new()
		{
			Name = "Spawnpoint",
			Flags = new List<ECellFlag>() { ECellFlag.AIR },
			Selectable = false,
			FactionSpawn = EFaction.PLAYER
        };
        public static readonly GridCell Invalid = new()
        {
            Name = "INVALID",
            Flags = new List<ECellFlag>() { ECellFlag.UNKNOWN },
            Selectable = false,
        };

        public static GridCell GetByName(string name)
            => name switch
            {
                "Air" => Air,
                "Floor" => Floor,
                "SpawnPoint" => Spawnpoint,
                "INVALID" => Invalid,
                _ => throw new Exception()
            };

        public static List<GridCell> GetAll()
            => new() { Air, Floor, Spawnpoint, Invalid };
    }
}
