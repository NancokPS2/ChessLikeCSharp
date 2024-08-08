using System.Security.Principal;

namespace ChessLike.Entity;

public partial class Job : IStats
{
    public Identity Identity = new("UndefinedMobJob");
    public StatSet Stats { get; set; } = new();
    public List<Action> actions = new();
}
