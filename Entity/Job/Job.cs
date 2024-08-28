using System.Security.Principal;

namespace ChessLike.Entity;

public partial class Job : IStats<StatName>
{
    public Identity Identity = new("UndefinedMobJob");
    public StatSet<StatName> Stats { get; set; } = new();
    public List<Action> actions = new();
}
