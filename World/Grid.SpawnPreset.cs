using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public partial class Grid
{
    public struct SpawnPreset
    {
        Vector3i SpawnPosition;
        IGridObject Object;
    }
    
}
