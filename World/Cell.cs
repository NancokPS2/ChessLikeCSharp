using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public partial class Grid
{

    public struct Cell : IEquatable<Cell>
    {

        public string name = "UNNAMED";
        public List<CellFlag> flags = new List<CellFlag>();
        public bool selectable = false;

        public Cell()
        {
            name = "";
            flags = new List<CellFlag>();

        }

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
                flags = new List<CellFlag>(){CellFlag.AIR},
                selectable = false,
            };
            public static readonly Cell Floor = new()
            {
                name = "Floor",
                flags = new List<CellFlag>(){CellFlag.SOLID},
                selectable = true,
            };
            public static readonly Cell Invalid = new()
            {
                name = "INVALID",
                flags = new List<CellFlag>(){CellFlag.UNKNOWN},
                selectable = false,
            };
        }

    }
}
