namespace ChessLike.Entity;

public enum EMovementMode
{
	INVALID = -1,
	GROUNDED,
	AMPHIBIOUS,
	FLY,
	/// <summary>
	/// Move within range.
	/// </summary>
	TELEPORT,
	/// <summary>
	/// A form of teleportation without requirements.
	/// </summary>
	PLACE,

}
