namespace ChessLike.Entity;

public partial class Mob
{
    public Mob PresetDefaultStats()
    {
        Stats.SetStat(StatSet.Name.HEALTH, 100);
        Stats.SetStat(StatSet.Name.ENERGY, 20);
        Stats.SetStat(StatSet.Name.SPEED, 5);

        actions.Add(new Action().PresetDealDamage(10));
        
        return this;
    }

    public Mob PresetCriticalHealth()
    {
        float new_health = Stats.GetMax(StatSet.Name.HEALTH) * 0.1f;
        Stats.SetValue(StatSet.Name.HEALTH, new_health);
        return this;
    }

}
