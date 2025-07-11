using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;
using ChessLike.Entity;
using ChessLike.Shared.Serialization;
using ExtendedXmlSerializer;
using ISerializable = ChessLike.Shared.Serialization.ISerializable;
using ChessLike.Storage;
using Godot;
namespace ChessLike.Entity;

//Factions can store groups of Mobs, their inventories and grant allegiance between mobs.
[GlobalClass]
public partial class Faction : Resource, ISerializable
{
    [Export]
    public string DisplayedName = "Unnamed Faction";

    [Export]
    public EFaction Identifier = EFaction.NEUTRAL;

    [Export]
    private Godot.Collections.Dictionary<EFaction, float> relationList
    {
        set => RelationList = new(value);
        get => new(RelationList);
    }
    public Dictionary<EFaction, float> RelationList = new();

    [Export]
    public Inventory Inventory = new(999);

    public Faction() : base() { }

    private float GetRelation(EFaction other)
    {
        float relation;
        RelationList.TryGetValue(other, out relation);
        return relation;
    }

    public bool IsAlly(EFaction other) => GetRelation(other) > 60;
    public bool IsEnemy(EFaction other) => GetRelation(other) <= 0;
    public bool IsNeutral(EFaction other) => !IsAlly(other) && !IsEnemy(other);

    public Faction(EFaction identifer = EFaction.NEUTRAL)
    {
        this.Identifier = identifer;
        RelationList = new();
    }

    public static bool operator ==(Faction a, Faction b)
    {
        return a.Identifier == b.Identifier;
    }

    public static bool operator !=(Faction a, Faction b)
    {
        return !(a == b);
    }
    public override bool Equals(object? obj)
    {
        if (obj is Faction faction)
        {
            return faction.Identifier == this.Identifier;
        }
        else
        {
            return false;
        }
    }


    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
