namespace ChessLike.Entity;

public partial class Job
{

    public static class Preset
    {
        public static Job Grunt()
        {
            Job output = new();

            output.Stats.SetStat(StatSet.Name.HEALTH, 100);
            output.Stats.SetStat(StatSet.Name.ENERGY, 40);
            output.Stats.SetStat(StatSet.Name.MOVEMENT, 20);
            output.Stats.SetStat(StatSet.Name.DELAY, 100);

            return output;
        }
        public static Job Medic()
        {
            Job output = new();
            
            output.Stats.SetStat(StatSet.Name.HEALTH, 90);
            output.Stats.SetStat(StatSet.Name.ENERGY, 50);
            output.Stats.SetStat(StatSet.Name.MOVEMENT, 20);
            output.Stats.SetStat(StatSet.Name.DELAY, 100);

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
