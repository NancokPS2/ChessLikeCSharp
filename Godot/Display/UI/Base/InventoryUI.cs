using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using ChessLike.Shared.Storage;
using Godot;

public partial class InventoryUI : BaseButtonMenu<Button, Inventory.Slot>, ISceneDependency
{

	public static readonly Godot.Color MODULATE_SELECTED = new(0.5f,0.5f,0.5f);
	public static readonly Godot.Color MODULATE_NORMAL = new(1,1,1);
	[Export]
	public string SCENE_PATH { get; set; } = "";

	protected Inventory InventorySelected;
	private InventoryUI? _transfer_ui;
	[Export]
	public InventoryUI? TransferUI {get => _transfer_ui; set => SetTransferUI(value);}
	protected Inventory.Error LastError;
	[Export]
	public bool CanTransferItems;

	private void SetTransferUI(InventoryUI? ui)
	{
		InventoryUI? current = _transfer_ui;
		InventoryUI? entering = ui;

		if (current is not null)
		{
			current.ButtonPressed -= OnTransferUIPressed;
		}
		if (entering is not null)
		{
			entering.ButtonPressed += OnTransferUIPressed;
			if (entering.TransferUI == this || entering.CanTransferItems){throw new Exception("2-way transfering is not supported.");}
		}
		else
		{
			CanTransferItems = false;
		}
		_transfer_ui = ui;
	}

	public void Update(Mob mob)
	{
		InventorySelected = mob.MobInventory;
		Update(InventorySelected.GetSlots());
	}

	public void Update(Faction faction)
	{
		InventorySelected = faction.Inventory;
		Update(InventorySelected.GetSlots());
	}

	protected override void OnButtonPressed(Button button, Inventory.Slot slot)
	{
		//Run the button's selection stuff.
		if (!InventorySelected.ContainsSlot(slot)){throw new Exception("This slot is not from this inventory!?");}

		//Deselect the current one.
		if (TupleSelected is not null)
		{
			TupleSelected?.Item1.AnimateIntermitentGlowStop();          
		}
		//Select the new one if valid.
		if (TupleSelected is not null)
		{
			button.AnimateIntermitentGlow(1, MODULATE_SELECTED);
		}
		
		TupleSelected = (button, slot);

		base.OnButtonPressed(button, slot);
	}


	protected override void OnButtonHovered(Button button, Inventory.Slot slot, bool hovered)
	{
		button.Modulate = hovered ? new Godot.Color(0.5f, 0.5f, 0.5f) : new Godot.Color(1, 1, 1);
	}

	protected override void OnButtonCreated(Button button, Inventory.Slot slot)
	{
		if(slot.Item is not null) {button.Text = slot.Item.Name;}
		else if (slot.FlagWhitelist.Count != 0) {button.Text = slot.FlagWhitelist[0].ToString();}
		else {button.Text = "EMPTY";}
	}

    //TODO: handle deselecting slots once an operation happens.
    //TODO: handle the bi-directional transfer of items (FIRST think of how it will work)
	public void OnTransferUIPressed(Button other_btn, Inventory.Slot other_slot)
	{
        //Must be able to transfer items
		if (!CanTransferItems){return;}
        //A button on this side must be selected for the transfer to happen.
		if (TupleSelected is null){return;}
        //The selected slot must have an item.
        if (TupleSelected?.Item2.Item is null || TransferUI?.TupleSelected?.Item2.Item is null)
        {
			LastError = Inventory.Error.FATAL; 
            MessageQueue.AddMessage("Failed to transfer, there is no item in either of the slots.", 3);
            return;
        }

		if (TransferUI is null){throw new Exception("No TransferUI has been set, transfering items should be disabled.");}

		Inventory.Error remove_err = TransferUI.InventorySelected.RemoveItem(other_slot);
		if (remove_err != Inventory.Error.NONE)
		{
			LastError = remove_err; 
            MessageQueue.AddMessage("Failed to transfer due to " + LastError.ToString(), 3);
            return;
		}

		Inventory.Error add_err = InventorySelected.AddItem(TupleSelected?.Item2.Item, TupleSelected?.Item2);
		if (add_err != Inventory.Error.NONE)
		{
			LastError = add_err; 
            MessageQueue.AddMessage("Failed to transfer due to " + LastError.ToString(), 3);
            return;
		}

        OnButtonPressed(null, null);
	}

}
