using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;

public partial class Action
{
    public class Manager : Manager<Action>
    {

        public override List<Action> CreatePrototypes()
        {
            return new()
            {
                Action.Create(EAction.HEAL),
                Action.Create(EAction.PUNCH),
                Action.Create(EAction.WALK),
            };
        }

        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "action"
            );

        }
    }
}
