using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;

public partial class InventoryUI : BaseButtonMenu<Button, Inventory.Slot>, ISceneDependency
{

	public static readonly Godot.Color MODULATE_SELECTED = new(0.5f,0.5f,0.5f);
	public static readonly Godot.Color MODULATE_NORMAL = new(1,1,1);
	[Export]
	public string SCENE_PATH { get; set; } = "";

	protected Inventory.Slot? SlotSelected;
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
			current.ButtonPressed -= ProcessTransfer;
		}
		if (entering is not null)
		{
			entering.ButtonPressed += ProcessTransfer;
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
		//No slot has been chosen yet.
		if (SlotSelected is null)
		{
			if (!InventorySelected.ContainsSlot(slot)){throw new Exception("This slot is from another inventory!?");}

			SlotSelected = slot;
			button.Modulate = MODULATE_SELECTED;
		}

		//If one was selected and the new one is empty.
		else if (SlotSelected.Item is not null && slot.Item is null)
		{
			if (slot.IsItemValid(SlotSelected.Item))
			{
				Item to_transfer = SlotSelected.Item;
				InventorySelected.RemoveItem(to_transfer);
				InventorySelected.AddItem(to_transfer, slot);

				button.Modulate = MODULATE_NORMAL;
				Update();
			}
		}
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

	public void ProcessTransfer(Button other_btn, Inventory.Slot other_slot)
	{
		if (!CanTransferItems){return;}
		if (SlotSelected is null){return;}

		if (TransferUI is null){throw new Exception("No TransferUI has been set, transfering items should be disabled.");}

		Inventory.Error remove_err = TransferUI.InventorySelected.RemoveItem(other_slot);
		if (remove_err != Inventory.Error.NONE)
		{
			LastError = remove_err; return;
		}

		Inventory.Error add_err = InventorySelected.AddItem(other_slot, SlotSelected);
	}

}
