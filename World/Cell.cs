using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public partial struct Cell : IEquatable<Cell>
{

    public string Name = "UNNAMED";
    public List<ECellFlag> Flags = new List<ECellFlag>();
    public bool Selectable = false;

    public Cell()
    {
        Name = "";
        Flags = new List<ECellFlag>();

    }

    public Cell(string name, List<ECellFlag> flags, bool selectable)
    {
        Name = name;
        Flags = flags;
        Selectable = selectable;
    }

    public bool Equals(Cell other)
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

    public static bool operator ==(Cell a, Cell b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Cell a, Cell b)
    {
        return !a.Equals(b);
    }

    public static class Preset
    {
        public static readonly Cell Air = new()
        {
            Name = "Air",
            Flags = new List<ECellFlag>(){ECellFlag.AIR},
            Selectable = false,
        };
        public static readonly Cell Floor = new()
        {
            Name = "Floor",
            Flags = new List<ECellFlag>(){ECellFlag.SOLID},
            Selectable = true,
        };
        public static readonly Cell Spawnpoint = new()
        {
            Name = "Spawnpoint",
            Flags = new List<ECellFlag>(){ECellFlag.AIR, ECellFlag.PLAYER_SPAWNPOINT},
            Selectable = false,
        };
        public static readonly Cell Invalid = new()
        {
            Name = "INVALID",
            Flags = new List<ECellFlag>(){ECellFlag.UNKNOWN},
            Selectable = false,
        };
    }

}
