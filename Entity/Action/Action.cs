using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using ChessLike.Entity;
using ChessLike.Shared.Identification;

namespace ChessLike.Entity;

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
public partial class Action : IGridReader
{

    public string name = "Undefined Action";
    public EAction Identifier = EAction.PUNCH;

    public EffectFilterParams FilterParams = new();
    public TargetingParams TargetParams = new();
    public List<Effect> EffectParams = new();


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
            if (FilterParams.AffectMob && !(target is Mob))
            {
                valid = false;
                goto decide;
            }
/* 
            if (FilterParams.IgnoreAlly && (owner.GetLevel(target.Identity) == RelationType.GOOD || owner.GetLevel(target.Identity) == RelationType.V_GOOD))
            {
                valid = false;
                goto decide;
            }

            if (FilterParams.IgnoreEnemy && !(owner.GetLevel(target.Identity) == RelationType.GOOD || owner.GetLevel(target.Identity) == RelationType.V_GOOD))
            {
                valid = false;
                goto decide;
            }
*/
            if (target.Stats.GetValuePrecent(StatName.HEALTH) > FilterParams.MaximumHealthPercent)
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

    public virtual void Use(UsageParams usage_params)
    {
        foreach (Effect effect in EffectParams)
        {
            effect.CustomUse(usage_params);
            
        }
    }

/* 
    public string GetEffectDescription(UsageParams usage_params)
    {
        string output = "";

        //Target stat value changes.
        foreach (StatSet.Name stat in effect_params.TargetStatChangeValue.Keys)
        {
            string? stat_string = Enum.GetName(typeof(StatSet.Name), stat);
            if (stat_string == null)
            {
                throw new ArgumentNullException("What?");
            }
            string stat_value = effect_params.TargetStatChangeValue[stat].ToString();

            output = new(output + "Inflict: " + stat_string + stat_value + "\n");
        }

        //Owner stat value changes.
        foreach (StatSet.Name stat in effect_params.TargetStatChangeValue.Keys)
        {
            string? stat_string = Enum.GetName(typeof(StatSet.Name), stat);
            if (stat_string == null)
            {
                throw new ArgumentNullException("What?");
            }
            string stat_value = effect_params.TargetStatChangeValue[stat].ToString();

            output = new(output + "Suffer: " + stat_string + stat_value + "\n");
        }

        if (effect_params.PositionChange != Vector3i.ZERO)
        {
            output = new( output + effect_params.PositionChange.ToString() + "\n" );
        }

        if (output == "")
        {
            output = "Sorry, nothing.";
        }
        return output;
        
            
    } */
}

