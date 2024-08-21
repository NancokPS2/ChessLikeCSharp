using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared;

namespace ChessLike.Entity;

public static class TurnQueue
{
    public static ITurn GetNext(List<ITurn> participants)
    {
        if (participants.Count == 0)
        {
            throw new NullReferenceException("There's no participants.");
        }

        ITurn output = participants[0];
        foreach (Mob participant in participants)
        {
            if (participant.Delay < output.Delay)
            {
                output = participant;
            }
        }

        return output;
    }

    public static void AdvanceDelay(List<ITurn> participants, float amount)
    {
        
        foreach (ITurn participant in participants)
        {
            participant.Delay -= amount;
        }
    }

}
