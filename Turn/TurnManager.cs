using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;
namespace ChessLike.Turn;

//TODO: add a log of the latest turns, so in case of continuous ties, the same ITurn is not selected repeatedly.
[GlobalClass]
public partial class TurnManager : Node3D
{

    UniqueList<ITurn> Participants = new();

    protected ITurn? CurrentTaker
    {
        get => currentTaker;
        set
        {
            currentTaker = value;
            Debug.Assert(currentTaker is not null);
        }
    }
    ITurn? currentTaker;

    ITurn? _round_ender;

    public ITurn? RoundEnder { get => _round_ender; set => _round_ender = value; }


    bool ReadyToStartTurn;
    bool ReadyToEndTurn;

    public TurnManager()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        EventBus.CombatStateChanged -= OnBattleStateChanged;
        EventBus.MobStateChanged -= OnMobStateChanged;
        EventBus.InputTurnEnded -= OnInputTurnEnded;

        EventBus.CombatStateChanged += OnBattleStateChanged;
        EventBus.MobStateChanged += OnMobStateChanged;
        EventBus.InputTurnEnded += OnInputTurnEnded;
	}


    public override void _Process(double delta)
    {
        base._Process(delta);
        if (ReadyToStartTurn)
        {
            if (CurrentTaker is null)
                throw new Exception();
            StartTurn();
        }
        else if (ReadyToEndTurn)
        {
            if (CurrentTaker is null)
                throw new Exception();
            EndTurn();
        }
    }


    #region Handle Participants
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

    #endregion

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
        if (Participants.Count == 0) { throw new Exception("No participants to iterate over."); }

        ITurn output = Participants.First();

        foreach (var item in Participants)
        {
            if (lowest)
            {
                if (item.DelayCurrent < output.DelayCurrent)
                {
                    output = item;
                }

            }
            else
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
        ReadyToStartTurn = false;

        //Whoever has the lowest delay takes it.
        CurrentTaker = GetWithLowestDelay();
        Debug.Assert(CurrentTaker is not null);

        float initial_delay = CurrentTaker.DelayCurrent;

        //Reduce everyone's delay by until the taker's 0.
        foreach (var item in Participants)
        {
            item.DelayCurrent -= initial_delay;
        }

        //Emit that time has passed.
        EventBus.TurnTimePassed?.Invoke(initial_delay);

        //Make sure the taker is at 0.
        if (CurrentTaker.DelayCurrent != 0)
        {
            throw new Exception("Unexpected result.");
        }
        if (CurrentTaker is Mob mob)
            EventBus.MobTurnStarted?.Invoke(mob);

        UpdateRoundEnder();
    }

    public void EndTurn()
    {
        ReadyToEndTurn = false;

        if (CurrentTaker is null) { throw new Exception("No one is taking a turn at this moment."); }

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

    #region Event Handling
    private void OnBattleStateChanged(ECombatState state)
    {
        if (state == ECombatState.TURN_SELECTION)
        {
            ReadyToStartTurn = true;
        }
        else
        {
            ReadyToStartTurn = false;
        }
    }

    private void OnMobStateChanged(Mob mob, EMobState state)
    {
        if (state == EMobState.COMBAT)
        {
            Add(mob);
        }
        else if (state == EMobState.BENCHED)
        {
            Remove(mob);
        }
    }
    
    private void OnInputTurnEnded()
    {
        Debug.Assert(CurrentTaker is not null);
        ReadyToEndTurn = true;
    }
    #endregion
}
