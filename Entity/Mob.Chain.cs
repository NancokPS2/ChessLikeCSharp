namespace ChessLike.Entity;

public partial class Mob
{
/*     public Mob ChainDefaultStats()
    {
        Stats.SetStat(StatSet.Name.HEALTH, 100);
        Stats.SetStat(StatSet.Name.ENERGY, 20);
        Stats.SetStat(StatSet.Name.SPEED, 5);

        actions.Add(new Action().ChainDealDamage(10).ChainPushBack(1));
        
        return this;
    } 
*/

    public Mob ChainCriticalHealth()
    {
        float new_health = Stats.GetMax(StatSet.Name.HEALTH) * 0.1f;
        Stats.SetValue(StatSet.Name.HEALTH, new_health);
        return this;
    }

}
