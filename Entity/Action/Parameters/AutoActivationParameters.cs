using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

public partial class AutoActivationParameters : Resource
{
    [Export]
    public bool IsTriggeredByPassive = false;

    [Export]
    public bool IsTriggeredByTurnEnd = false;
    
    [Export]
    public Godot.Collections.Array<EActionFlag> FlagsForTrigger = new();


}
