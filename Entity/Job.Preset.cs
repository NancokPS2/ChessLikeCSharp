namespace ChessLike.Entity;

public partial class Job
{

    public static class Preset
    {
        public static Job Grunt()
        {
            Job output = new();

            output.stats.SetStat(StatSet.Name.HEALTH, 100);
            output.stats.SetStat(StatSet.Name.ENERGY, 40);
            output.stats.SetStat(StatSet.Name.MOVEMENT, 20);
            output.stats.SetStat(StatSet.Name.DELAY, 100);

            return output;
        }
        public static Job Medic()
        {
            Job output = new();
            
            output.stats.SetStat(StatSet.Name.HEALTH, 90);
            output.stats.SetStat(StatSet.Name.ENERGY, 50);
            output.stats.SetStat(StatSet.Name.MOVEMENT, 20);
            output.stats.SetStat(StatSet.Name.DELAY, 100);

            return output;
        }

        public static Job Warrior()
        {
            Job output = new();

            return output;
        }

        public static Job Ranger()
        {
            Job output = new();

            return output;
        }
    }

}
