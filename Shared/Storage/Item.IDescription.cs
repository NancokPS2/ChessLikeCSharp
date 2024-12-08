using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Item : IDescription
{
    public string GetDescription(bool extended = false)
    {
        return string.Format(@"Name: {0}
        Price: {1}
        Flags: {2}", Name, Price, Flags);
    }

    public string GetDescriptiveName()
    {
        return Name;
    }

}
