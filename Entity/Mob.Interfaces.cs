using ChessLike.Entity.Relation;
using ChessLike.Shared;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Mob
{
    //IStats
    public StatSet Stats { get; set; } = new();

    //IMobility
    Vector3i _position;
    public Vector3i Position { get => _position; set => _position = value; }

    public float Speed { get => Stats.GetValue(StatSet.Name.SPEED); set => Stats.SetValue(StatSet.Name.SPEED, value); }

    public float GetCellMovementCost(Cell cell)
    {
        float output = new();
        if(cell.flags.Contains(Cell.Flag.SOLID))
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
    Identity _identity = new();
    public Identity Identity { get => _identity; set => _identity = value; }

    Dictionary<Identity, float> _relation_list = new();
    public Dictionary<Identity, float> relation_list { get => _relation_list; set => _relation_list = value; }

    public List<Identity> GetAllWithLevel(Level level)
    {
        List<Identity> output = new();
        foreach (Identity identity in relation_list.Keys)
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
        relation_list.TryGetValue(other, out pass);
        return pass;
    }

    public void SetRelationWith(Identity other, float val)
    {
        relation_list[other] = val;
    }

    /// <summary>
    /// Set max and current, for stats that are not meant to not use the max value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value"></param>

    //ITurn
    public float DelayBase { get => Stats.GetMax(StatSet.Name.DELAY); set => Stats.SetMax(StatSet.Name.DELAY, value); }
    public float Delay { get => Stats.GetValue(StatSet.Name.DELAY); set => Stats.SetValue(StatSet.Name.DELAY, value); }
}
