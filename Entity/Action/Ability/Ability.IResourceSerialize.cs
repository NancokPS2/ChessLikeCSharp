using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Ability : IResourceSerialize<Ability, AbilityResource>
{
    //TODO
        public AbilityResource ToResource()
        {
            AbilityResource output = new();
            output.Identifier = Identifier;

            output.Name = Name;

            return output;
        }

        public static Ability FromResource(AbilityResource resource)
        {
            Ability output = new();
            output.Identifier = resource.Identifier;

            output.Name = resource.Name;

            return output;
        }
}
