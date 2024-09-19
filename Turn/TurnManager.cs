using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.DebugDisplay;

namespace ChessLike.Turn;

public partial class TurnManager
{
    public delegate void TurnChangeHandler(ITurn who);
    public event TurnChangeHandler? TurnEnded;
    public event TurnChangeHandler? TurnStarted;


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
        //Whoever has the lowest delay takes it.
        CurrentTaker = GetWithLowestDelay();

        float initial_delay = CurrentTaker.DelayCurrent;
        //Reduce everyone's delay by until the taker's 0.
        foreach (var item in Participants)
        {
            item.DelayCurrent -= initial_delay;
        }

        //Make sure the taker is at 0.
        if (CurrentTaker.DelayCurrent != 0)
        {
            throw new Exception("Unexpected result.");
        }
        TurnStarted?.Invoke(CurrentTaker);
    }

    public void EndTurn()
    {
        if (CurrentTaker is null) {throw new Exception("No one is taking a turn at this moment.");}

        //Reset the delay, the CurrentTaker should end up with a high delay.
        ResetDelay(CurrentTaker);

        TurnEnded?.Invoke(CurrentTaker);
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

    public void AdvanceDelay(float time)
    {
        foreach (var item in Participants)
        {
            DelayAdd(item, time);
        }
    }

}
