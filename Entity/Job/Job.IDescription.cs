using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Extension;

namespace ChessLike.Entity;

public partial class Job : IDescription
{
    public string GetDescription(bool extended = false)
    {
        return String.Format(
            "Identifier: {0} \nStats: {1} \nAbilities: {2} \nMovement Mode: {3}",
            GetDescriptiveName(),
            StatMultiplicativeBoostDict.ToString(),
            Abilities.ToStringList(),
            MovementMode.ToString()
        );
    }

    public string GetDescriptiveName()
    {
        return Enum.GetName(Identifier) ?? throw new Exception();
    }

}
