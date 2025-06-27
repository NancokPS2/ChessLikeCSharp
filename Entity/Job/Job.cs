using System.Security.Principal;
using ChessLike.Entity.Action;
using Godot;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

[GlobalClass, Obsolete("Incomplete resourcification")]
public partial class Job : Resource
{
    [Export]
    public string DisplayedName { get => displayedName != "" ? displayedName : Identifier.ToString(); set => displayedName = value; }
    protected string displayedName = "";

    [Export]
    public EJob Identifier = EJob.DEFAULT;
    public Dictionary<EStatName, float> StatMultiplicativeBoostDict = new();
    public List<Ability> Abilities = new();

    [Export]
    public EMovementMode MovementMode = EMovementMode.WALK;



    public override string ToString() => displayedName;
}
