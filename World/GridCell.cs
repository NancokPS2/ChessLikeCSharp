using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;


public partial struct GridCell : IEquatable<GridCell>
{

    public string Name = "UNNAMED";
    public List<ECellFlag> Flags = new List<ECellFlag>();
    public bool Selectable = false;

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

    public bool Equals(GridCell other)
    {
        return Name == other.Name && Flags == other.Flags && Selectable == other.Selectable;
    }
    public override bool Equals(Object? obj)
    {
        return base.Equals(obj);
        //return GetHashCode() == obj?.GetHashCode();
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
            Flags = new List<ECellFlag>() { ECellFlag.AIR, ECellFlag.PLAYER_SPAWNPOINT },
            Selectable = false,
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
            => new() {Air, Floor, Spawnpoint, Invalid};
    }

}
