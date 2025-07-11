using ChessLike.Entity;
using ChessLike.Entity.Action;
using static ChessLike.Entity.Mob;

namespace ChessLike.Storage;
public partial class Item : IActionProvider
{
    protected List<Ability> Abilities = new();


    public List<Ability> GetAbilities() => Abilities;

    public List<Ability> GetPassives() => Abilities; 
}
