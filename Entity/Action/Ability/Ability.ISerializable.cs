using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity.Action;

public partial class Ability : ISerializable
{
    public string GetFileName()
    {
        return Enum.GetName<EAbility>(Identifier);
    }
}
