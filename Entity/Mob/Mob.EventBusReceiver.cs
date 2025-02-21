using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Mob : IEventBusMember
{
    public void SetupEventBus()
    {
        EventBus.InventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(Inventory obj)
    {
        if (obj != MobInventory) return;

        Stats.BoostAdd(MobInventory, true);
        UpdateActions();
    }
}
