using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;
using ChessLike.Entity;
using ChessLike.Shared.Serialization;
using ChessLike.Entity;
using ExtendedXmlSerializer;
using ISerializable = ChessLike.Shared.Serialization.ISerializable;
namespace ChessLike.Entity;

//Factions can store groups of Mobs, their inventories and grant allegiance between mobs.
public partial class Faction : ISerializable
{

    public EFaction Identifier = EFaction.NEUTRAL;

    public Dictionary<EFaction, float> RelationList { get; set; } = new();

    public Inventory inventory = new(999);


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

    public string GetFileName()
    {
        return Enum.GetName(Identifier) + ".xml";
    }

    public string GetSubDirectory()
    {
        return "factions";
    }
}