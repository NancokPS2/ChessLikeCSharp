using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
namespace ChessLike.Turn;

//TODO: add a log of the latest turns, so in case of continuous ties, the same ITurn is not selected repeatedly.
public partial class TurnManager
{
    //public delegate void TurnChangeHandler(ITurn who);
    //public event TurnChangeHandler? TurnEnded;
    //public event TurnChangeHandler? TurnStarted;


    UniqueList<ITurn> Participants = new();

    ITurn? CurrentTaker;

    ITurn? _round_ender;

    public ITurn? RoundEnder { get => _round_ender; set => _round_ender = value; }


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

    private void UpdateRoundEnder()
    {
        //If the ender is null or is no longer in the Participant list. Set a new one.
        if (RoundEnder is null || !Participants.Contains(RoundEnder))
        {
            RoundEnder = GetWithHighestDelay();
        }
    }

    private ITurn GetWithLowestDelay() => GetByDelay(true);
    
    public ITurn GetWithHighestDelay() => GetByDelay(false);
    
    private ITurn GetByDelay(bool lowest)
    {
        if (Participants.Count == 0) {throw new Exception("No participants to iterate over.");}

        ITurn output = Participants.First();

        foreach (var item in Participants)
        {
            if (lowest)
            {
                if (item.DelayCurrent < output.DelayCurrent)
                {
                    output = item;
                }
                
            } else
            {
                if (item.DelayCurrent > output.DelayCurrent)
                {
                    output = item;
                }
            }
        }

        return output;
    }

    public ITurn? GetCurrentTurnTaker()
    {
        return CurrentTaker is not null ? CurrentTaker : null; //throw new Exception("There is not taker at this time, calm down.");
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
        if (CurrentTaker is Mob mob)
        {
            EventBus.MobTurnStarted?.Invoke(mob);
        }

        UpdateRoundEnder();
    }

    public void EndTurn()
    {
        if (CurrentTaker is null) {throw new Exception("No one is taking a turn at this moment.");}

        //Reset the delay, the CurrentTaker should end up with a high delay.
        ResetDelay(CurrentTaker);

        if (CurrentTaker is Mob mob)
        {
            EventBus.MobTurnEnded?.Invoke(mob);
        }

        //If the round ender just finished their turn, count that as the round ending.
        if (CurrentTaker == RoundEnder)
        {
            RoundEnder = null;
            EventBus.RoundEnded?.Invoke();
        }
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
