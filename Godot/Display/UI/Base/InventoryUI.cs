using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using ChessLike.Shared.Storage;
using ExtendedXmlSerializer.Core.Sources;
using Godot;

public partial class InventoryUI : BaseButtonMenu<Button, Inventory.Slot>, ISceneDependency, ITooltip
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
		InventorySelected.ClearEmptySlots();
		Update(InventorySelected.GetSlots());
	}

	protected override void OnButtonPressed(Button button, Inventory.Slot slot)
	{
		ButtonSelection(button, slot);

		base.OnButtonPressed(button, slot);
	}

	public void ButtonSelection(Button button, Inventory.Slot slot)
	{
		//Run the button's selection stuff.
		if (slot is null){throw new Exception("ALL buttons should be paired with a slot.");}
		if (!InventorySelected.ContainsSlot(slot)){throw new Exception("This slot is not from this inventory!?");}


 		//Deselect the current one if there is one.
		if (TupleSelected is not null && GodotObject.IsInstanceValid(TupleSelected?.Item1))
		{
			Button? selected_btn = TupleSelected?.Item1;
			if (selected_btn is not null)
			{
				selected_btn.AnimateIntermitentGlowStop();
			}
		}

		//Select the new one.
		button.Modulate = MODULATE_SELECTED;
		TupleSelected = (button, slot);
		button.AnimateIntermitentGlow(1, MODULATE_SELECTED);
	}
	public void ButtonDeselection()
	{
		TupleSelected?.Item1.AnimateIntermitentGlowStop();
		TupleSelected = null;
		return;
	}

	protected override void OnButtonHovered(Button button, Inventory.Slot slot, bool hovered)
	{
		base.OnButtonHovered(button, slot, hovered);
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
	public void OnTransferUIPressed(Button transfer_ui_btn, Inventory.Slot transfer_ui_slot)
	{
		//Must be able to transfer items
		if (!CanTransferItems){return;}
		//There must be an UI set to transfer to and from.
		if (TransferUI is null){throw new Exception("No TransferUI has been set, transfering items should be disabled.");}
		//A button on this side must be selected for the transfer to happen.
		if (TupleSelected is null){TransferUI.ButtonDeselection(); return;}
		//Either of the selected slots must have an item.
		if (TupleSelected?.Item2.Item is null && TransferUI?.TupleSelected?.Item2.Item is null)
		{
			LastError = Inventory.Error.FATAL; 
			MessageQueue.AddMessage("Failed to transfer, there is no item in either of the slots.", 3);
			ButtonDeselection();
			return;
		}

		LastError = Inventory.TransferItem(
			InventorySelected, 
			TupleSelected?.Item2,
			TransferUI.InventorySelected,
			transfer_ui_slot
		);

		if (LastError != Inventory.Error.NONE)
		{
			MessageQueue.AddMessage("Failed to transfer due to " + LastError.ToString(), 3);
		}
		else
		{
			Update();
			TransferUI.Update();
		}
		ButtonDeselection();
		TransferUI.ButtonDeselection();


	}

	public Inventory.Error TransferItemToInventory(Inventory source_inv, Inventory target_inv, Inventory.Slot source_slot, Inventory.Slot target_slot, Item item_to_transfer)
	{

		Inventory.Error remove_err = source_inv.RemoveItem(source_slot);
		if (remove_err != Inventory.Error.NONE)
		{
			return remove_err;
		}

		Inventory.Error add_err = target_inv.AddItem(item_to_transfer, target_slot);
		if (add_err != Inventory.Error.NONE)
		{
			return add_err;
		}

		return Inventory.Error.NONE;
	}

	public string GetText()
	{
		return TupleHovered?.Item2?.Item?.GetDescription() ?? "";
	}

	public Godot.Font GetFont()
	{
		return Global.Readonly.FONT_SMALL;
	}

	public bool IsShown()
	{
		return TupleHovered?.Item2?.Item is not null;
	}

	/* public Godot.Vector2 GetRectSize()
	{
		return TupleSelected?.Item1?.Size * 1.25f ?? new(120,80);
	} */

	public int GetFontSize()
	{
		return 16;
	}
}
