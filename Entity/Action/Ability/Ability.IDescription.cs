using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Shared.Serialization;
using Godot;

namespace ChessLike.Entity.Action;

public partial class Ability : IDescription
{
    public string GetDescription(bool extended = false)
    {
        return String.Format("Identifier: {0} \nName: {1} \nFilter Parameters: \n{2} \nTarget Parameters: \n{3} \nFlags: {4}",
            Enum.GetName(Identifier), 
            Name, 
            FilterParams.ToString().Indent(@"    "), 
            TargetParams.ToString().Indent(@"    "), 
            Flags.ToStringList()
        );
    }

    public string GetDescriptiveName() => Name;
}
