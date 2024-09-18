using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Time;

public class TurnManager
{
    UniqueList<ITurn> Participants = new();

    public void Add(ITurn participant)
    {
        Participants.Add(participant);
    }

    public void Remove(ITurn participant)
    {
        Participants.Remove(participant);
    }

    public ITurn? GetCurrentTurnTaker()
    {
        if (Participants.Count == 0) {throw new Exception("No participants to iterate over.");}

        ITurn output = Participants.First();

        foreach (var item in Participants)
        {
            if (item.DelayCurrent < output.DelayCurrent)
            {
                output = item;
            }
        }

        return output;
    }

    public void AdvanceTime(float time)
    {
        foreach (var item in Participants)
        {
            item.DelayCurrent += time;
        }
    }


    public void AdvanceTime(ITurn turn, float time)
    {
        if (!Participants.Contains(turn)) {throw new Exception("This is not a participant.");}
        turn.DelayCurrent += time;
    }
}
