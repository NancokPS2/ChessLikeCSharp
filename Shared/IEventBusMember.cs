using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IEventBusMember
{
    public void SetupEventBus();
}
