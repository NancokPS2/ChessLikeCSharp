using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared;

public partial class StatSetResource<[MustBeVariant] TEnum> : Godot.Resource where TEnum : notnull, Enum
{
    [Export]
    public Godot.Collections.Dictionary<TEnum, float> Contents;

}
