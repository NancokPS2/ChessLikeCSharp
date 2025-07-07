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
    TURN_SELECTION,

    /// <summary>
    /// Undefined
    /// </summary>
    TURN_ENDING,

    /// <summary>
    /// Waiting for the player to select an action in the ActionUI
    /// </summary>
    ACTION_INPUT,

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
