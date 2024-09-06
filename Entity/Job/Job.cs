using System.Security.Principal;

namespace ChessLike.Entity;

public partial class Job : IStats<StatName>
{
    public EJob Identifier = EJob.CIVILIAN;
    public StatSet<StatName> Stats { get; set; } = new();
    public List<Action> actions = new();
}
