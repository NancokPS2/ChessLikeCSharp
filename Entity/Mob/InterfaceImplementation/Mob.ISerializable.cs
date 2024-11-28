using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;

public partial class Mob : ISerializable
{
    public string GetFileName()
    {
        return DisplayedName;// + "_" + Godot.Time.GetTicksMsec().ToString();
    }

    public string GetSubDirectory()
    {
        return "mobs";
    }
}
