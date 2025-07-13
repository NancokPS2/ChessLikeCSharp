using ChessLike.Entity;
using ChessLike.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : BaseButtonMenu<Button, Item>
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


	public void Update(MobEquipmentInventory equipmentUI)
	{
		Update(equipmentUI.GetItems());
	}

	protected override void OnButtonCreated(Button button, Item param)
	{
		button.Text = param.Name;
		button.TooltipText = param.ToString();
	}

	#region Event Connection
	private void OnMobSelected(Mob mob)
	{
		Update(mob.EquipmentInventory);
	}
	#endregion
}
