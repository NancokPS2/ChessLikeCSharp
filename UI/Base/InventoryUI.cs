using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using ChessLike.Storage;
using ExtendedXmlSerializer.Core.Sources;
using Godot;

public partial class InventoryUI : BaseButtonMenu<Button, ItemFilter>, ISceneDependency, ITooltip
{

	public static readonly Godot.Color MODULATE_SELECTED = new(0.5f,0.5f,0.5f);
	public static readonly Godot.Color MODULATE_NORMAL = new(1,1,1);
	[Export]
	public string SCENE_PATH { get; set; } = "";

	protected MobEquipmentInventory InventorySelected;
	private InventoryUI? _transfer_ui;
	[Export]
	public InventoryUI? TransferUI {get => _transfer_ui; set => SetTransferUI(value);}
	protected Inventory.EInventoryError LastError;
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

	[Obsolete("Does nothing.")]
	public void Update(Mob mob)
	{
		InventorySelected = new();
		//Update(InventorySelected.GetSlots());
	}

	[Obsolete("Does not work.")]
	public void Update(Faction faction)
	{
		/* InventorySelected = faction.Inventory;
		InventorySelected.ClearEmptySlots();
		Update(InventorySelected.GetSlots()); */
	}

	protected override void _ButtonPressed(Button button, ItemFilter slot)
	{
		ButtonSelection(button, slot);

		base._ButtonPressed(button, slot);
	}

	public void ButtonSelection(Button button, ItemFilter slot)
	{
		//Run the button's selection stuff.
		if (slot is null){throw new Exception("ALL buttons should be paired with a slot.");}


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

	protected override void _ButtonHovered(Button button, ItemFilter slot, bool hovered)
	{
		base._ButtonHovered(button, slot, hovered);
		button.Modulate = hovered ? new Godot.Color(0.5f, 0.5f, 0.5f) : new Godot.Color(1, 1, 1);
	}

	protected override void _ButtonCreated(Button button, ItemFilter slot)
	{
		base._ButtonCreated(button, slot);
		//WIP: Missing tooltips
		if (slot.Item is not null)
		{
			button.Text = slot.Item.Name;
			SetTooltip(button, slot.Item.GetDescription());
		}
		else if (slot.FlagWhitelist.Count != 0)
		{
			button.Text = slot.FlagWhitelist[0].ToString();
		}
		else
		{
			button.Text = "EMPTY";
		}
	}

	//TODO: handle deselecting slots once an operation happens.
	//TODO: handle the bi-directional transfer of items (FIRST think of how it will work)
	public void OnTransferUIPressed(Button transfer_ui_btn, ItemFilter transfer_ui_slot)
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
			LastError = Inventory.EInventoryError.UNHANDLED; 
			MsgLog.LogGameMsg("Failed to transfer, there is no item in either of the slots.", new(){Duration = 3});
			ButtonDeselection();
			return;
		}

		/* LastError = Inventory.TransferItem(
			InventorySelected, 
			TupleSelected?.Item2,
			TransferUI.InventorySelected,
			transfer_ui_slot
		);

		if (LastError != Inventory.EInventoryError.NONE)
		{
			MessageQueue.AddMessage("Failed to transfer due to " + LastError.ToString(), 3);
		}
		else
		{
			Update();
			TransferUI.Update();
		}
		ButtonDeselection();
		TransferUI.ButtonDeselection(); */


	}

	public Inventory.EInventoryError TransferItemToInventory(Inventory source_inv, Inventory target_inv, ItemFilter source_slot, ItemFilter target_slot, Item item_to_transfer)
	{

		Inventory.EInventoryError remove_err = source_inv.RemoveItem(source_slot);
		if (remove_err != Inventory.EInventoryError.NONE)
		{
			return remove_err;
		}

		Inventory.EInventoryError add_err = target_inv.AddItem(item_to_transfer, target_slot);
		if (add_err != Inventory.EInventoryError.NONE)
		{
			return add_err;
		}

		return Inventory.EInventoryError.NONE;
	}

	public string GetText()
	{
		return TupleHovered?.Item2?.Item?.GetDescription() ?? "";
	}

	public Godot.Font GetFont()
	{
		return Global.ManagerFont.ResourceGet("Regular");
	}

	public bool ShouldShow()
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
