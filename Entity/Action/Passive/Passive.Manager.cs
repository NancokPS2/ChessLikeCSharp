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

        public override List<Passive> CreatePrototypes()
        {
            throw new NotImplementedException();
            List<Passive> output = new()
            {
            };
            return output;
        }

        public override Passive GetFromResource(PassiveResource resource)
        {
            return FromResource(resource);
        }

        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "passive"
            );

        }

        public Passive GetFromEnum(EPassive action)
        {
            return FromResource(
              GetAllResources().First(x => x.Identifier == action)
            );
        }
    }
}
