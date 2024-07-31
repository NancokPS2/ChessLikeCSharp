using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Relation;

 
public interface IRelation
{

    public Dictionary<Identity, float> RelationList {set; get;}
    public Identity Identity {set; get;}
    public void SetRelationWith(Identity other, float val);
    public float GetRelationWith(Identity other);
    public Level GetLevel(Identity other);
    public List<Identity> GetAllWithLevel(Level level);
    public bool IsLevel(Identity other, Level level);
}

public enum Level
{
    V_BAD,
    BAD,
    NEUTRAL,
    GOOD,
    V_GOOD,
}