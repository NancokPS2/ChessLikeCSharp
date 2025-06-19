using System.Security.Principal;
using ChessLike.Entity.Action;
using Godot;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

[GlobalClass]
public partial class Job : Resource
{
    private string displayedName = "";
    public EJob Identifier = EJob.DEFAULT;
    public Dictionary<EStatName, float> StatMultiplicativeBoostDict = new();
    public List<Ability> Abilities = new();
    public EMovementMode MovementMode = EMovementMode.WALK;

    public string DisplayedName { get => displayedName != "" ? displayedName : Identifier.ToString(); set => displayedName = value; }


    public override string ToString() => displayedName;
}
