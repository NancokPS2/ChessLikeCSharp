using System.Data.Common;
using System.Reflection;
using ChessLike.Entity.Relation;
using ChessLike.Storage;
namespace ChessLike.Entity;

//Factions can store groups of Mobs, their inventories and grant allegiance between mobs.
public struct Faction : IRelation
{
    const string UNALIGNED_IDENTITY = "UNALIGNED";

    UniqueList<Identity> members = new();

    public Dictionary<Identity, float> RelationList { get; set; } = new();

    public Inventory inventory = new(0);


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
		return a.Identity.unique_identifier == b.Identity.unique_identifier;
	}

	public static bool operator !=(Faction a, Faction b)
	{
		return !(a == b);
	}

    //IRelation
    public Identity Identity { get; set; } = new Identity(Identity.INVALID_IDENTIFIER);


    public List<Identity> GetAllWithLevel(Level level)
    {
        List<Identity> output = new();
        foreach (Identity identity in RelationList.Keys)
        {
            if(IsLevel(identity, level))
            {
                output.Add(identity);
            }
        }
        return output;
    }

    public Level GetLevel(Identity other)
    {
        int relation = (int)GetRelationWith(other);

        if (relation < -40)
        {
            return Level.V_BAD;
        }
        if(Enumerable.Range(-40, -10).Contains(relation))
        {
            return Level.BAD;
        }
        if(Enumerable.Range(-10, 30).Contains(relation))
        {
            return Level.NEUTRAL;
        }
        if(Enumerable.Range(30, 60).Contains(relation))
        {
            return Level.GOOD;
        }
        if (relation > 60)
        {
            return Level.V_GOOD;
        }
        throw new ArgumentNullException("Could not return, issue with range coverage.");
    }

    public bool IsLevel(Identity other, Level level)
    {
        int relation = (int)GetRelationWith(other);
        return GetLevel(other) == level;
    }

    public float GetRelationWith(Identity other)
    {
        float pass = 0.0f;
        RelationList.TryGetValue(other, out pass);
        return pass;
    }

    public void SetRelationWith(Identity other, float val)
    {
        RelationList[other] = val;
    }

    
}
