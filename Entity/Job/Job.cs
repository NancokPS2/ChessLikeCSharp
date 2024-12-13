using System.Security.Principal;
using ChessLike.Entity.Action;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

public partial class Job
{
    public EJob Identifier = EJob.DEFAULT;
    public Dictionary<StatName, float> StatMultiplicativeBoostDict = new();
    public List<Ability> Abilities = new();
    public EMovementMode MovementMode = EMovementMode.WALK;
}
