using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.Identification;


public partial interface IRelation : IIdentify
{

    public Dictionary<Identity, float> RelationList {set; get;}
    public void SetRelationWith(Identity other, float val);
    public float GetRelationWith(Identity other);
    public RelationType GetLevel(Identity other);
    public List<Identity> GetAllWithLevel(RelationType level);
    public bool IsLevel(Identity other, RelationType level);
}
