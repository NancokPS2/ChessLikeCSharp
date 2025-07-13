using ChessLike.Entity;
using ChessLike.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : BaseButtonMenu<Button, Item?>
{
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
		List<Item?> items = new();
		foreach (var slot in equipInventory.GetSlots())
		{
			items.Add(equipInventory.GetItem(slot));
		}
		Update(items);
	}

	protected override void OnButtonCreated(Button button, Item? param)
	{
		button.Text = param?.Name ?? "Empty";
		button.TooltipText = param?.ToString() ?? "Empty";
	}

	#region Event Connection
	private void OnMobSelected(Mob mob)
	{
		Update(mob.EquipmentInventory);
	}
	#endregion
}
