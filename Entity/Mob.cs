using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using ChessLike.Entity.Relation;
using ChessLike.Shared;
using ChessLike.Turn;
using ChessLike.World;

namespace ChessLike.Entity;



public partial class Mob : IPosition, IRelation, IStats, ITurn
{
    List<Class> classes = new();
    List<Action> actions = new();

    public Mob()
    {
        Identity = new(Identity.INVALID);
    }
    public Mob(string name, List<Class> classes)
    {
        Identity = new(name);
        this.classes = classes;
    }
}