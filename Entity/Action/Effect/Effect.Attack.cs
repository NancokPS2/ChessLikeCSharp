namespace ChessLike.Entity.Action;

public class EffectAttack : Effect
{
    public Dictionary<EStatName, float> AttackStatBoost = new(){
        {EStatName.STRENGTH, 1},
    };
    public float FlatDamage = 0;

    public override void CustomUse(UsageParameters usage_params)
    {
        float damage = FlatDamage;
        foreach (var item in AttackStatBoost)
        {
            damage = usage_params.OwnerRef.Stats.GetValue(item.Key) * item.Value;
        }

        foreach (Mob target in usage_params.MobsTargeted)
        {
            float defense = target.Stats.GetValue(EStatName.DEFENSE);
            float total = Math.Clamp(damage - defense, 0, float.MaxValue);
            target.Stats.ChangeValue(EStatName.HEALTH, total);
        }
    }

    public EffectAttack SetDamageStat(EStatName stat, float modifier)
    {
        AttackStatBoost[stat] = modifier;
        return this;
    }
    public EffectAttack SetFlatDamage(float damage)
    {
        FlatDamage = damage;
        return this;
    }
}

