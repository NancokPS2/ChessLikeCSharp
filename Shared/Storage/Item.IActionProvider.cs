using ChessLike.Entity;
using ChessLike.Entity.Action;
using static ChessLike.Entity.Mob;

namespace ChessLike.Shared.Storage;
public partial class Item : IActionProvider
{
    protected List<Ability> Abilities = new();
    protected List<Passive> Passives = new();


    public List<Ability> GetAbilities() => Abilities;

    public List<Passive> GetPassives() => Passives; 
}
