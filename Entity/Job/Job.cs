using System.Security.Principal;
using ChessLike.Entity.Action;
using Godot;
using static ChessLike.Entity.Mob;

namespace ChessLike.Entity;

[GlobalClass]
public partial class Job : Resource
{
    [Export]
    public string DisplayedName { get => displayedName != "" ? displayedName : Identifier.ToString(); set => displayedName = value; }
    protected string displayedName = "";

    [Export]
    public EJob Identifier = EJob.DEFAULT;

    [Export]
    private Godot.Collections.Dictionary<EStatName, float> statMultiplicativeBoostDict
    {
        set => StatMultiplicativeBoostDict = new(value);
        get => new(StatMultiplicativeBoostDict);
    }
    public Dictionary<EStatName, float> StatMultiplicativeBoostDict = new();

    [Export]
    private Godot.Collections.Array<Ability> abilities
    {
        set => Abilities = new(value);
        get => new(Abilities);
    }
    public List<Ability> Abilities = new();

    [Export]
    public EMovementMode MovementMode = EMovementMode.WALK;



    public override string ToString() => displayedName;
}
