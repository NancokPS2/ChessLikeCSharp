using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action : IResourceSerialize<Action, ActionResource>
{
    //TODO
        public ActionResource ToResource()
        {
            ActionResource output = new();

            output.Name = Name;

            return output;
        }

        public static Action FromResource(ActionResource resource)
        {
            Action output = new();

            output.Name = resource.Name;

            return output;
        }
}
