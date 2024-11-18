using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Passive : IResourceSerialize<Passive, PassiveResource>
{
    //TODO
        public PassiveResource ToResource()
        {
            PassiveResource output = new();
            output.Identifier = Identifier;

            output.Name = Name;

            return output;
        }

        public static Passive FromResource(PassiveResource resource)
        {
            Passive output = new();
            output.Identifier = resource.Identifier;

            output.Name = resource.Name;

            return output;
        }
}
