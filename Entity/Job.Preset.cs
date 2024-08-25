namespace ChessLike.Entity;

public partial class Job
{

    public static class Preset
    {
        public static Job Basic()
        {
            Job output = new();

            output.ChainDefaultStats();
            output.actions.Add(Action.Preset.BasicAttack());

            return output;
        }
        public static Job Wizard()
        {
            Job output = Basic().ChainWizard();
            return output;
        }

        public static Job Warrior()
        {
            Job output = Basic().ChainWarrior();


            return output;
        }

        public static Job Ranger()
        {
            Job output = new();

            return output;
        }
    }

}
