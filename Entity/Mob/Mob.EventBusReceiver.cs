using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Storage;
using Godot;

namespace ChessLike.Entity;

public partial class Mob : IEventBusMember
{

    public void SetupEventBus()
    {
        EventBus.MobTurnStarted += OnMobTurnStarted;
        EventBus.MobTurnEnded += OnMobTurnEnded;
        EventBus.ActionUsed += OnActionUsed;
        EventBus.InventoryChanged += OnInventoryChanged;
    }

    private void OnMobTurnStarted(Mob mob)
    {
        if (mob != this) return;
        TurnActive = true;
		Stats.RefillValues([EValueName.ACTION, EValueName.REACTION]);
    }

    private void OnMobTurnEnded(Mob mob)
    {
        if (mob != this) return;
        TurnActive = false;
	}

    private void OnActionUsed(UsageParameters parameters)
    {
        if (parameters.OwnerRef != this) return;
        if (TurnActive)
            Stats.ChangeValue(EValueName.ACTION, -parameters.ActionRef.CostParams.Action);
        else
            Stats.ChangeValue(EValueName.REACTION, -parameters.ActionRef.CostParams.Reaction);

		Stats.ChangeValue(EValueName.SUB_ACTION, -parameters.ActionRef.CostParams.Move);

		Stats.ChangeValue(EValueName.MOVE, -parameters.ActionRef.CostParams.Move);
	}

	private void OnInventoryChanged(MobEquipmentInventory obj)
    {
        if (obj != EquipmentInventory) return;

        UpdateEquipmentStatBoosts();
        UpdateActions();
    }
}
