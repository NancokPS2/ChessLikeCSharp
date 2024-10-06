using System.Security.Principal;

namespace ChessLike.Entity;

public partial class Job : IStats<StatName>
{
    public EJob Identifier = EJob.DEFAULT;
    public StatSet<StatName> Stats { get; set; } = new();
    public List<Action> Actions = new();
}
