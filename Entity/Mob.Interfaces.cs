using ChessLike.Entity;
using ChessLike.Shared;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Mob
{
    //IStats
    public StatSet<StatName> Stats { get; set; } = new();

    //IPosition
    Vector3i _position = new();
    public List<Vector3i> QueuedGridPositions { get; set; } = new();
    public Vector3i Position { get => _position; set => SetPosition(value); }
    public Vector3 FloatPosition { get; set; }

    public float Speed { get => Stats.GetValue(StatName.MOVEMENT); set => Stats.SetValue(StatName.MOVEMENT, value); }

    void SetPosition(Vector3i value){
        MobToLocationDict.Remove(value);
        _position = value;
        MobToLocationDict[value] = this;
    }

    public float GetCellMovementCost(Grid.Cell cell)
    {
        float output = new();
        if(cell.flags.Contains(CellFlag.SOLID))
        {
            output += float.MaxValue;
        }
        if(cell.flags.Count == 0)
        {
            output += 1;
        }

        return output;
    }

    //IRelation

    public Identity Identity { get; set; } = new(Identity.INVALID_IDENTIFIER);

    public Dictionary<Identity, float> RelationList { get; set; } = new();

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

    /// <summary>
    /// Set max and current, for stats that are not meant to not use the max value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value"></param>

    //ITurn
    public float DelayBase { 
        get => Stats.GetMax(StatName.DELAY); set => Stats.SetMax(StatName.DELAY, value); 
        }
    public float Delay { 
        get => Stats.GetValue(StatName.DELAY); set => Stats.SetValue(StatName.DELAY, value); 
        }
}
