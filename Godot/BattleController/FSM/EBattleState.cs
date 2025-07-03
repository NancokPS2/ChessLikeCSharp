namespace Godot;

public partial class BattleController
{
    public enum EBattleState
    {
        INVALID,
        PAUSED,
        TAKING_TURN,
        ENDING_TURN,
        AWAITING_ACTION,
        TARGETING,
        ACTION_RUNNING,
        PREPARATION,
        END_COMBAT,
    }
}
