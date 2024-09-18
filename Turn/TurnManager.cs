using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Turn;

public class DelayManager
{
    UniqueList<ITurn> Participants = new();

    ITurn? CurrentTaker;

    public void Add(List<ITurn> participants)
    {
        foreach (var item in participants)
        {
            Add(item);        
        }
    }

    public void Add(ITurn participant)
    {
        Participants.Add(participant);
        ResetDelay(participant);
    }

    public void Remove(ITurn participant)
    {
        Participants.Remove(participant);
    }

    public List<ITurn> GetParticipants()
    {
        return Participants;
    }

    private ITurn GetWithLowestDelay()
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
    public ITurn? GetCurrentTurnTaker()
    {
        return CurrentTaker;
    }

    public void StartTurn()
    {
        CurrentTaker = GetWithLowestDelay();

        AdvanceDelay(CurrentTaker.DelayCurrent);

        if (CurrentTaker.DelayCurrent == 0)
        {
            throw new Exception("Unexpected result.");
        }
    }

    public void EndTurn()
    {
        if (CurrentTaker is null) {throw new Exception("No one is taking a turn at this moment.");}

        ResetDelay(CurrentTaker);
    }

    private void ResetDelay(ITurn turn)
    {
        turn.DelayCurrent = turn.GetDelayBase() + turn.DelayToAddOnTurnEnd;
    }

    public void DelayAdd(ITurn turn, float delay)
    {
        if (turn == CurrentTaker)
        {
            turn.DelayToAddOnTurnEnd -= delay;
        }
        else
        {
            turn.DelayCurrent -= delay;
        }
    }

    private void AdvanceDelay(float time)
    {
        foreach (var item in Participants)
        {
            DelayAdd(item, time);
        }
    }

}
