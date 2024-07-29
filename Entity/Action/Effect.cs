using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using ChessLike.Entity;

namespace ChessLike.Entity.Action;

/// <summary>
/// Usage:
/// Start by creating an Effect
/// 
/// Create an UsageParams object to use with the Effect
/// Set UsageParams.owner and grid. 
/// Use IsTargetingValid() to filter a valid result for UsageParams.location_selected 
/// Pass the result of GetLocationsTargeted() to UsageParams.locations_targeted.
/// Pass the result of GetTargetsAffected() to UsageParams.mob_targets
/// 
/// </summary>
public class Effect
{
    public enum EffectFilterParameter
    {
        IGNORE_ALLY,
        IGNORE_ENEMY,
        AFFECT_MOB,
        AFFECT_ENTITY,
    }

    public enum Error
    {
        NONE,
        MISSING_INTERFACES,//Missing interfaces, and therefore, the required methods for this to work.
        PARAMETER_BLOCK,//The parameters prevented this from working.
        FATAL,
    }
    public EffectFilterParams effect_filter_params;
    public TargetParams target_params;
    public EffectParams effect_params;


    public virtual bool IsTargetingValid(UsageParams usage_params)
    {
        return true;
    }

    public List<Vector3i> GetLocationsTargeted(UsageParams usage_params)
    {
        return new List<Vector3i>();
    }

    //TODO
    public virtual List<Mob> GetTargetsAffected(UsageParams usage_params)
    {
        List<Mob> output = new();
        Mob owner = usage_params.owner;


        foreach (Mob target in usage_params.mob_targets)
        {
            bool valid = false;
            /* 
            if(effect_filter_params)
            {
                valid = false
                goto decide;
            }
            */
            if (effect_filter_params.AffectMob && !(target is Mob))
            {
                valid = false;
                goto decide;
            }

            if (effect_filter_params.IgnoreAlly && (owner.GetLevel(target.Identity) == Relation.Level.GOOD || owner.GetLevel(target.Identity) == Relation.Level.V_GOOD))
            {
                valid = false;
                goto decide;
            }

            if (effect_filter_params.IgnoreEnemy && !(owner.GetLevel(target.Identity) == Relation.Level.GOOD || owner.GetLevel(target.Identity) == Relation.Level.V_GOOD))
            {
                valid = false;
                goto decide;
            }

            if (target.Stats.GetValuePrecent(StatSet.Name.HEALTH) > effect_filter_params.MaximumHealthPercent)
            {
                valid = false;
                goto decide;
            }

        decide:
            if (valid)
            {
                output.Add(target);
            }

        }
        return output;
    }

    public virtual Error Use(UsageParams usage_params)
    {
        foreach (Mob target in usage_params.mob_targets)
        {
            foreach (StatSet.Name stat in effect_params.StatAddition.Keys)
            {
                float value = effect_params.StatAddition[stat];
                target.Stats.ChangeValue(stat, value);
            }
            target.Position += effect_params.PositionChange;
        }

        return Error.NONE;
    }



}
