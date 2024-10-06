using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;

public partial class Action
{
    public class Manager : SerializableManager<Action, ActionResource>
    {

        public override List<Action> CreatePrototypes()
        {
            List<Action> output = new()
            {
                Action.Create(EAction.HEAL),
                Action.Create(EAction.PUNCH),
                Action.Create(EAction.MOVE),
            };
            return output;
        }

        public override Action ConvertFromResource(ActionResource resource)
        {
            return FromResource(resource);
        }

        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "action"
            );

        }

        public Action GetFromEnum(EAction action)
        {
            return GetAll().First(x => x.Identifier == action);
        }
    }
}
