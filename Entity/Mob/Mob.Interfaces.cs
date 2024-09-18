using ChessLike.Entity;
using ChessLike.Shared;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Mob
{
    //IStats
    public StatSet<StatName> Stats { get; set; } = new();


}
