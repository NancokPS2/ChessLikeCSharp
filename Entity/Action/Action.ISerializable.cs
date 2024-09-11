using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;

public partial class Action : ISerializable
{
    public string GetFileName()
    {
        return Enum.GetName<EAction>(Identifier);
    }
}
