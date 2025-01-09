using System.Security.Principal;
using ChessLike.Entity.Action;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

public partial class Job : IActionProvider
{
    public List<Ability> GetAbilities() => Abilities;

    public List<Passive> GetPassives() => Passives;

}
