using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity.Action;

public partial class Passive
{
    public class Manager : SerializableManager<Passive, PassiveResource>
    {

        //TODO
        public override List<Passive> CreatePrototypes()
        {
            return new List<Passive>();
            List<Passive> output = new(){
                new PassiveDamageReduction(1), 
                new PassiveDoT(1)
                };
            return output;
        }

        public Passive GetFromEnum(EPassive action)
        {
            return FromResource(
              GetAllResources().First(x => x.Identifier == action)
            );
        }
    }
}
