using ChessLike.UI.Base;
using ExtendedXmlSerializer;
using Godot;
using System;

[GlobalClass]
public partial class PreparationUI : Control
{
	[Export]
	protected ConfirmationButton? ConfirmationButtonNode;

	public override void _Ready()
	{
		base._Ready();
		if (ConfirmationButtonNode is null) throw new Exception();
		EventBus.CombatStateChanged += OnCombatStateChanged;
		ConfirmationButtonNode.Confirmed += OnConfirmed;

	}



	#region Event Handling
	private void OnCombatStateChanged(ECombatState obj)
	{
		if (obj == ECombatState.PREPARATION && !IsInsideTree())
		{
			this.ReturnSelf();
		}
		else if (obj != ECombatState.PREPARATION && IsInsideTree())
		{
			this.RemoveSelf();
		}
	}

	private void OnConfirmed()
	{
		//There must be enough player units to start combat.
		if (CombatScene.GetMobsInCombat().Where(x => x.Faction == ChessLike.Entity.EFaction.PLAYER).Count() == 0)
		{
			MessageQueue.AddMessage("No player units in combat.");
			return;
		}

		EventBus.InputPreparationFinished?.Invoke();
	}
	#endregion
}
