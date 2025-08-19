using Godot;
using System;

[GlobalClass]
public partial class SaveSlotUI : Control
{
	public static SaveSlotUI? SelectedSlot;

	[Export]
	public bool EmptySlot
	{
		get => emptySlot;
		set
		{
			emptySlot = value;
			if (!IsInsideTree()) return;

			Update();

		}
	}

	public SaveFile? SaveFileSet;

	public string ProfileName
	{
		get
		{
			return LineEditProfileNameNode?.Text ?? "UNKNOWN";
		}

		set
		{
			if (LineEditProfileNameNode is null) throw new NullReferenceException();

			LineEditProfileNameNode.Text = value;
		}

	}
	public long Date;
	public int Slot = 0;

	private bool emptySlot;

	[Export]
	public Label? LabelNewNode;

	[Export]
	public LineEdit? LineEditProfileNameNode;

	[Export]
	public Label? LabelDateNode;

	[Export]
	public Button? ButtonSelect;

	public override void _Ready()
	{
		base._Ready();
		Update();
		ButtonSelect.Pressed += OnButtonSelectPressed;
	}

	public override void _ExitTree()
	{
		SelectedSlot = null;
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		Modulate = SelectedSlot == this ? Colors.Green : Colors.White;
	}

	private void Update()
	{
		LabelNewNode.Visible = EmptySlot;
		if (EmptySlot)
		{
			LineEditProfileNameNode.Text = "";
			LabelDateNode.Text = "Date: ???";
			LineEditProfileNameNode.Editable = true;
		}
		else
		{
			LineEditProfileNameNode.Text = ProfileName + "-" + Slot.ToString();
			LabelDateNode.Text = $"Date: {Time.GetDatetimeStringFromUnixTime(Date).Replace("T", " ")}";
			LineEditProfileNameNode.Editable = false;
		}
	}

	public void SetSaveFile(SaveFile item, int slot)
	{
		EmptySlot = false;
		ProfileName = item.ProfileName;
		Date = item.Date;
		SaveFileSet = item;
		Slot = slot;
	}

	private void OnButtonSelectPressed()
	{
		SelectedSlot = this;
	}
}
