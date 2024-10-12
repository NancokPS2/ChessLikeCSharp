using System.Security.Principal;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

public partial class Job
{
    public EJob Identifier = EJob.DEFAULT;
    public MobStatSet Stats = new();
    public List<Action> Actions = new();
    public EMovementMode MovementMode = EMovementMode.WALK;
}
