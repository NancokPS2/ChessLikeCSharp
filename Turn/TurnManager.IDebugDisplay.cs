using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

namespace ChessLike.Turn;

public partial class TurnManager : IDebugDisplay
{
    public string GetName()
    {
        return "TurnManager";
    }

    //TODO: Make sure this still works
    public string GetText()
    {
        string output = ""; 
        foreach (var item in GetParticipants())
        {
            if (item is Mob mob)
            {
                output += mob.DisplayedName + " | " + mob.DelayCurrent + "\n";
            }
            else
            {
                output += item.ToString() + " | " + item.DelayCurrent + "\n";
            }
        }
        return output;
    }
}
