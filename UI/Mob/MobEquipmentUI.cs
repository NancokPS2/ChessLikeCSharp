using ChessLike.Entity;
using ChessLike.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : BaseButtonMenu<Button, (Item?, MobEquipmentInventory.ESlot)>
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
		List<(Item?, MobEquipmentInventory.ESlot)> items = new();
		foreach (var slot in equipInventory.GetSlots())
		{
			items.Add((equipInventory.GetItem(slot), slot));
		}
		Update(items);
		MobEquipmentInventorySelected = equipInventory;
	}

	protected override void OnButtonCreated(Button button, (Item?, MobEquipmentInventory.ESlot) param)
	{
		button.Text = param.Item1?.Name ?? param.Item2.ToString();
		button.TooltipText = param.Item1?.ToString() ?? param.Item2.ToString();
	}

	#region Event Connection
	private void OnMobSelected(Mob mob)
	{
		Update(mob.EquipmentInventory);
	}
	#endregion
}
