using Godot;
using System;

[GlobalClass, Obsolete("Separate stuff into events instead of having so many hard calls")]
public partial class SaveSelectionUI : Control
{

	[Export]
	Button? ButtonLoad;

	[Export]
	Button? ButtonDelete;

	[Export]
	public Control SaveContainerNode;

	[Export]
	public PackedScene SaveSlotUIScene;

	public override void _EnterTree()
	{
		base._EnterTree();
		PopulateSaveEntries();
	}

	public override void _Ready()
	{
		base._Ready();
		ButtonDelete.Pressed += OnButtonDeletePressed;
		ButtonLoad.Pressed += OnButtonLoadPressed;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (SaveSlotUI.SelectedSlot is not null)
		{
			if (SaveSlotUI.SelectedSlot.EmptySlot)
				ButtonLoad.Text = "NEW";
			else
				ButtonLoad.Text = "LOAD";
		}
	}


	[Obsolete("Unfinished")]
	private void OnSaveTextSubmitted(string newText)
	{
		SaveManager.LoadSave(newText, 0);
	}

	public void PopulateSaveEntries()
	{
		var saves = SaveManager.GetAllSaves();
		foreach (var item in saves)
		{
			SaveSlotUI slotNode = SaveSlotUIScene.Instantiate<SaveSlotUI>();
			slotNode.SetSaveFile(item.Item1, item.Item2);
			SaveContainerNode.AddChild(slotNode);

		}
	}

	private void OnButtonLoadPressed()
	{
		if (SaveSlotUI.SelectedSlot is null) return;

		if (SaveSlotUI.SelectedSlot.EmptySlot)
		{
			string profile = SaveSlotUI.SelectedSlot.ProfileName;
			int slot = SaveSlotUI.SelectedSlot.Slot;

			if (SaveManager.SaveExists(profile, slot)) throw new Exception($"Save already exists {profile} {slot}");

			SaveManager.NewSave(profile, slot);
		}
		else
			SaveManager.LoadSave(SaveSlotUI.SelectedSlot.ProfileName, SaveSlotUI.SelectedSlot.Slot);
	}

	private void OnButtonDeletePressed()
	{
		throw new NotImplementedException();
	}
}
