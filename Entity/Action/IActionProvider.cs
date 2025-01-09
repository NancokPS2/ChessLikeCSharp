using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public interface IActionProvider
{
    public List<Ability> GetAbilities();
    public List<Passive> GetPassives();
}
