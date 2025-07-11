using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Storage;

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
        TurnResourceReset();
    }

	private void OnMobTurnEnded(Mob mob)
	{
        TurnActive = false;
	}

    private void OnActionUsed(UsageParameters parameters)
    {
        if (parameters.OwnerRef != this) return;
        if (TurnActive)
            TurnActionsUsed++;
        else
            TurnReactionsUsed++;
        
	}

	private void OnInventoryChanged(Inventory obj)
    {
        if (obj != MobInventory) return;

        UpdateEquipmentStatBoosts();
        UpdateActions();
    }
}
