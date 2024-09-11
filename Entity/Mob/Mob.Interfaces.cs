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
