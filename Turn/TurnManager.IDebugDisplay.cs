using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Turn;

public partial class TurnManager : IDebugDisplay
{
    public string GetName()
    {
        return "TurnManager";
    }

    public string GetText()
    {
        string output = ""; 
        foreach (var item in GetParticipants())
        {
            if (item is TurnTakerTest taker)
            {
                output += taker.Name + " | " + taker.DelayCurrent + "\n";
            }
            else
            {
                output += item.ToString() + " | " + item.DelayCurrent + "\n";
            }
        }
        return output;
    }
}
