using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;
using ChessLike.Entity;
using ChessLike.Entity;
using ExtendedXmlSerializer;
namespace ChessLike.Entity;

//Factions can store groups of Mobs, their inventories and grant allegiance between mobs.
public partial class Faction
{
    //IRelation
    public List<Identity> GetAllWithLevel(RelationType level)
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

    public RelationType GetLevel(Identity other)
    {
        int relation = (int)GetRelationWith(other);

        if (relation < -40)
        {
            return RelationType.V_BAD;
        }
        if(Enumerable.Range(-40, -10).Contains(relation))
        {
            return RelationType.BAD;
        }
        if(Enumerable.Range(-10, 30).Contains(relation))
        {
            return RelationType.NEUTRAL;
        }
        if(Enumerable.Range(30, 60).Contains(relation))
        {
            return RelationType.GOOD;
        }
        if (relation > 60)
        {
            return RelationType.V_GOOD;
        }
        throw new ArgumentNullException("Could not return, issue with range coverage.");
    }

    public bool IsLevel(Identity other, RelationType level)
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
