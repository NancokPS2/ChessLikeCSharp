using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Mob
{

//TODO
    private Ability _movement = new();

    private void UpdateActions()
    {
        ClearAbility();

        foreach (IActionProvider job in Jobs)
        {
            AddAbility(job.GetAbilities());
        }

        foreach (IActionProvider item in MobInventory.GetItems())
        {
            AddAbility(item.GetAbilities());
        }
    }

    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMovementMode.WALK);
        AddAbility(_movement);
        _movement_mode = mode;
    }

    public void AddAbility(List<Ability> abilities) => abilities.ForEach(x => AddAbility(x));
    public void AddAbility(Ability action)
    {
        Actions.Add(action);
        action.Owner = this;
        action.OnAddedToMob();
    }


    public void ClearAbility()
    {
        List<EAbility> identifiers = new(from abil in Actions select abil.Identifier);
        foreach (var item in identifiers)
        {
            RemoveAbility(item);
        }
    }

    public void RemoveAbility(EAbility action_enum)
    {
        IEnumerable<Ability> to_remove = Actions.Where(x => x.Identifier == action_enum);
        foreach (var item in to_remove)
        {
            item.OnRemovedFromMob();
        }
        Actions.RemoveAll(x => x.Identifier == action_enum); 
    }


    public List<Ability> GetAbilities()
    {
        return Actions;
    }

    public void AddPassive(Passive passive)
    {
        Passives.Add(passive);
        passive.Owner = this;
        passive.OnAddedToMob();
    }

    public void RemovePassive(EPassive passive_enum)
    {
        IEnumerable<Passive> to_remove = Passives.Where(x => x.Identifier == passive_enum);
        foreach (var item in to_remove)
        {
            item.OnRemovedFromMob();
        }
        Passives.RemoveAll(x => x.Identifier == passive_enum); 
    }

    public void ClearPassive()
    {
        List<EPassive> identifiers = new(from abil in Passives select abil.Identifier);
        foreach (var item in identifiers)
        {
            RemovePassive(item);
        }
    }

    public List<Passive> GetPassives()
    {
        return Passives;
    }
    
}
