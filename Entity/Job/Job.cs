using System.Security.Principal;
using ChessLike.Entity.Action;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

public partial class Job
{
    private string displayedName = "";
    public EJob Identifier = EJob.DEFAULT;
    public Dictionary<StatName, float> StatMultiplicativeBoostDict = new();
    public List<Ability> Abilities = new();
    public List<Passive> Passives = new();
    public EMovementMode MovementMode = EMovementMode.WALK;

    public string DisplayedName { get => displayedName != "" ? displayedName: Identifier.ToString(); set => displayedName = value; }


    public override string ToString() => displayedName;
}
