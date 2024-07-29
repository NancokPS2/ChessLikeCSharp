using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared;

namespace ChessLike.Turn;

public class Queue
{
    UniqueList<ITurn> participants = new();

    public bool AddParticipant(ITurn participant)
    {
        return participants.Add(participant);
    }

    public bool RemoveParticipant(ITurn participant)
    {
        return participants.Remove(participant);
    }

    public ITurn GetNext()
    {
        if (participants.Count == 0)
        {
            throw new NullReferenceException("There's no participants.");
        }

        ITurn output = participants[0];
        foreach (ITurn participant in GetParticipants())
        {
            if (participant.Delay < output.Delay)
            {
                output = participant;
            }
        }

        return output;
    }

    public void AdvanceDelay(float amount)
    {
        
        foreach (ITurn participant in GetParticipants())
        {
            participant.Delay -= amount;
        }
    }

    public UniqueList<ITurn> GetParticipants()
    {
        return participants;
    }
}
