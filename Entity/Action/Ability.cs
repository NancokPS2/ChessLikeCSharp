using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using ChessLike.Entity;
using ChessLike.Shared.Identification;
using ChessLike.World;

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
public partial class Ability
{

    //References
    public Mob Owner;

    public string Name = "Undefined Action";
    public EAbility Identifier = EAbility.PUNCH;
    public int PriorityDefault = 0;
    public List<EPassiveTrigger> PassiveTriggers = new();
    public float EnergyCost = 0;
    public float HealthCost = 0;

    public MobFilterParameters FilterParams = new();
    public TargetingParameters TargetParams = new();
    public AnimationParameters AnimationParams = new();
    public PassiveParameters PassiveParams = new();
    public List<Effect> Effects = new();


    public virtual void Use(UsageParams usage_params)
    {
        usage_params.OwnerRef.Stats.ChangeValue(StatName.HEALTH, HealthCost);
        usage_params.OwnerRef.Stats.ChangeValue(StatName.ENERGY, EnergyCost);

        foreach (Effect effect in Effects)
        {
            effect.Use(usage_params);
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

