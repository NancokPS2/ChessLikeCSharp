using ChessLike.Entity;
using ChessLike.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : BaseButtonMenu<Button, (Item?, EMobEquipmentSlot)>
{
	public MobEquipmentInventory MobEquipmentInventorySelected;
	public MobEquipmentUI() : base()
	{
	}

	public override void _Ready()
	{
		base._Ready();
		EventBus.MobSelected -= OnMobSelected;
		EventBus.MobSelected += OnMobSelected;
	}


	public void Update(MobEquipmentInventory equipInventory)
	{
		List<(Item?, EMobEquipmentSlot)> items = new();
		foreach (var slot in equipInventory.GetSlots())
		{
			items.Add((equipInventory.GetItem(slot), slot));
		}
		Update(items);
		MobEquipmentInventorySelected = equipInventory;
	}

	protected override void _ButtonCreated(Button button, (Item?, EMobEquipmentSlot) param)
	{
		button.Text = param.Item1?.Name ?? param.Item2.ToString();
		//button.TooltipText = param.Item1?.ToString() ?? param.Item2.ToString();
		SetTooltip(button, param.Item1?.GetDescription() ?? "NO ITEM");
	}

	#region Event Handling
	private void OnMobSelected(Mob mob)
	{
		Update(mob.EquipmentInventory);
	}
	#endregion
}
