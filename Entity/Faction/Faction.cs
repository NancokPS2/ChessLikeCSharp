using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;
using ChessLike.Entity;
using ChessLike.Shared.Serialization;
using ChessLike.Storage;
using ExtendedXmlSerializer;
using ISerializable = ChessLike.Shared.Serialization.ISerializable;
namespace ChessLike.Entity;

//Factions can store groups of Mobs, their inventories and grant allegiance between mobs.
public partial class Faction : IRelation, ISerializable
{
    const string UNALIGNED_IDENTITY = "UNALIGNED";

    public EFaction identifier = EFaction.NEUTRAL;
    UniqueList<Identity> members = new();

    public Dictionary<Identity, float> RelationList { get; set; } = new();

    public Inventory inventory = new(0);
    public Identity Identity { get; set; } = new Identity(Identity.INVALID_IDENTIFIER);


    public Faction(string name = UNALIGNED_IDENTITY)
    {
        Identity = new(name, name);
        members = new();
        RelationList = new();
    }
    public Faction(Identity[] members, string name) : this(name)
    {
        foreach (Identity member in members)
        {
            AddMember(member);
        }
    }

    public List<Identity> GetMembers()
    {
        return members;
    }

    public void AddMember(Identity member)
    {
        members.Add(member);
    }

    public void RemoveMember(Identity member)
    {
        members.Remove(member);
    }

    public static bool operator ==(Faction a, Faction b)
	{
		return a.Identity.Identifier == b.Identity.Identifier;
	}

	public static bool operator !=(Faction a, Faction b)
	{
		return !(a == b);
	}
    public override bool Equals(object? obj)
    {
        if (obj is Faction faction)
        {
            return faction == this;
        }else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
