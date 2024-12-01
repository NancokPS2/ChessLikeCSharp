using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;

namespace ChessLike.Entity;

public partial class Mob
{

//TODO
    private Ability _movement = new();
    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMovementMode.WALK);
        AddAbility(_movement);
        _movement_mode = mode;
    }

    public void AddAbility(Ability action)
    {
        Actions.Add(action);
        action.Owner = this;
    }

    public void RemoveAbility(EAbility action_enum, bool all = true)
    {
        if (all)
        {
            Actions.RemoveAll(x => x.Identifier == action_enum);
        }
        else
        {
            Actions.Remove( Actions.First(x => x.Identifier == action_enum) );
        }

    }

    public List<Ability> GetAbilities()
    {
        return Actions;
    }

    public void AddPassive(Passive passive)
    {
        Passives.Add(passive);
        passive.Owner = this;
    }

    public void RemovePassive(EPassive passive_enum, bool all = true)
    {
        if (all)
        {
            Passives.RemoveAll(x => x.Identifier == passive_enum);
        }
        else
        {
            Passives.Remove( Passives.First(x => x.Identifier == passive_enum) );
        }

    }

    public List<Passive> GetPassives()
    {
        return Passives;
    }
    
}
