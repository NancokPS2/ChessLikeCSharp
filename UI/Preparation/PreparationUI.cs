using ChessLike.Entity;
using ChessLike.UI.Base;
using ExtendedXmlSerializer;
using Godot;
using System;

[GlobalClass]
public partial class PreparationUI : Control
{
	[Export]
	protected MobListUI? MobListUINode;

	[Export]
	protected MobUI? MobUINode;

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
		if (obj == ECombatState.PREPARATION)
		{
			if (!IsInsideTree()) this.AddSelf();
			(MobListUINode ?? throw new Exception())
				.Update(Global.ManagerFaction.ResourceGet(EPackIDFaction.Player.ToString(), true, true));
		}
		else if (obj != ECombatState.PREPARATION && IsInsideTree())
		{
			if (IsInsideTree()) this.RemoveSelf(true);
		}
	}

	private void OnConfirmed()
	{
		//There must be enough player units to start combat.
		if (Mob.GetInstancesInState(EMobState.COMBAT).Where(x => x.Faction == ChessLike.Entity.EFaction.PLAYER).Count() == 0)
		{
			MsgLog.AddMessage("No player units in combat.");
			return;
		}

		EventBus.InputPreparationFinished?.Invoke();
	}
	#endregion
}
