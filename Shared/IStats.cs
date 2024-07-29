using System.Numerics;
using System.Xml;
using ChessLike.Storage;

namespace ChessLike.Shared;
public interface IStats
{

    public StatSet Stats {set; get;}
/*     public enum Stat
    {
        HEALTH,
        ENERGY,
        SPEED,
        DELAY,
    }
    public Dictionary<Stat, FloatResource> Stats {get; set;}
    public void SetStat(Stat stat, float value);
    public void SetStatValue(Stat stat, float value);
    public void SetStatMax(Stat stat, float value);
    public void SetStatMin(Stat stat, float value);
    public float GetStatValue(Stat stat);
    public float GetStatMax(Stat stat);
    public float GetStatMin(Stat stat); */

}

