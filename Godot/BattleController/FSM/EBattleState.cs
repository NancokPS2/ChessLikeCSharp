namespace Godot;

public enum EBattleState
{
    /// <summary>
    /// Invalid value
    /// </summary>
    INVALID,

    /// <summary>
    /// Emitted when the pause menu is brought up.
    /// </summary>
    PAUSED,

    /// <summary>
    /// Waiting for TurnManager to assign a turn.
    /// </summary>
    AWAITING_TURN,

    /// <summary>
    /// Undefined
    /// </summary>
    ENDING_TURN,

    /// <summary>
    /// Waiting for the player to select an action in the ActionUI
    /// </summary>
    AWAITING_ACTION,

    /// <summary>
    /// WIP: Enables targeting for the player.
    /// </summary>
    TARGETING,

    /// <summary>
    /// WIP: Actions are currently being processed.
    /// </summary>
    ACTION_RUNNING,

    /// <summary>
    /// Undefined
    /// </summary>
    PREPARATION,

    /// <summary>
    /// Undefined
    /// </summary>
    END_COMBAT,
}
