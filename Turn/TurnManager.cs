using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;
namespace ChessLike.Turn;

//TODO: add a log of the latest turns, so in case of continuous ties, the same ITurn is not selected repeatedly.
[GlobalClass]
public partial class TurnManager : Node3D
{

    UniqueList<ITurn> Participants = new();

	List<ITurn> TurnOrder = new();

	ITurn? CurrentTurnOwner;

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
            StartTurn();
        }
        else if (ReadyToEndTurn)
        {
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

	private void SortByDelay(ref List<ITurn> iTurns)
	{
		iTurns.Sort((x, y) => (int)(y.DelayCurrent - x.DelayCurrent));
		if (iTurns.Last().DelayCurrent != iTurns.Min(x => x.DelayCurrent))
			throw new Exception("It was expected that the last element had the lowest delay.");
	}

	public void StartTurn()
    {
        ReadyToStartTurn = false;

		//Make sure there is a turn order.
		if (TurnOrder.IsEmpty())
			TurnOrder = new(Participants);

		SortByDelay(ref TurnOrder);
		
		//Update who is taking the current turn.
		CurrentTurnOwner = TurnOrder.Last();

		//Decrease the delay of all participants.
		float delayToDecrease = CurrentTurnOwner.DelayCurrent;
		foreach (var item in Participants)
		{
			item.DelayCurrent -= delayToDecrease;
		}

		//Emit stuff for
		if (CurrentTurnOwner is Mob mob)
		{
			EventBus.TurnTimePassed?.Invoke(delayToDecrease);
			EventBus.MobTurnStarted?.Invoke(mob);
		}
    }

    public void EndTurn()
    {
        ReadyToEndTurn = false;

		if (CurrentTurnOwner is null)
			throw new Exception($"Who ended the turn if there was no owner? {CurrentTurnOwner}");
		if (TurnOrder.IsEmpty())
			throw new Exception($"The turn order is empty, what is ending their turn!? Current owner {CurrentTurnOwner}");

		TurnOrder.Remove(CurrentTurnOwner);
		ResetDelay(CurrentTurnOwner);

		//Emit stuff.
		if (CurrentTurnOwner is Mob mob)
		{
			EventBus.MobTurnEnded?.Invoke(mob);
		}
        //If the turn order ended up empty. The round ended.
        if (TurnOrder.IsEmpty())
        {
            EventBus.RoundEnded?.Invoke();
        }

    }

	private void ResetDelay(ITurn turn)
	{
		turn.DelayCurrent = turn.GetDelayBase() + turn.DelayToAddOnTurnEnd;
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
        Debug.Assert(CurrentTurnOwner is not null);
        ReadyToEndTurn = true;
    }
    #endregion
}
