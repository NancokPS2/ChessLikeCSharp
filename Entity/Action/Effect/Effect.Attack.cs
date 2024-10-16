namespace ChessLike.Entity.Action;

public class EffectAttack : Effect
{
    public Dictionary<StatName, float> AttackStatBoost = new(){
        {StatName.STRENGTH, 1},
    };
    public float FlatDamage = 0;

    public override void CustomUse(Ability.UsageParams usage_params)
    {
        float damage = FlatDamage;
        foreach (var item in AttackStatBoost)
        {
            damage = usage_params.OwnerRef.Stats.GetValue(item.Key) * item.Value;
        }

        foreach (Mob target in usage_params.MobsTargeted)
        {
            float defense = target.Stats.GetValue(StatName.DEFENSE);
            float total = Math.Clamp(damage - defense, 0, float.MaxValue);
            target.Stats.ChangeValue(StatName.HEALTH, total);
        }
    }

    public EffectAttack SetDamageStat(StatName stat, float modifier)
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

