using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public partial struct Cell : IEquatable<Cell>
{
    public enum Flag
    {
        UNKNOWN,
        SOLID,
        LIQUID,
        AIR,
    }

    public string name;
    public Shared.IGridPosition occupant;
    public List<Flag> flags;
    public bool selectable;

    public bool Equals(Cell other)
    {
        return name == other.name && flags == other.flags && selectable == other.selectable;
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
            name = "Air",
            flags = new List<Flag>(){Flag.AIR},
            selectable = false,
        };
        public static readonly Cell Floor = new()
        {
            name = "Floor",
            flags = new List<Flag>(){Flag.SOLID},
            selectable = true,
        };
        public static readonly Cell Invalid = new()
        {
            name = "INVALID",
            flags = new List<Flag>(){Flag.UNKNOWN},
            selectable = false,
        };
    }

}
