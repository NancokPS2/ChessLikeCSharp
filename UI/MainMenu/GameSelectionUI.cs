using Godot;
using System;

[GlobalClass,Obsolete("Separate stuff into events instead of having so many hard calls")]
public partial class SaveSelectionUI : Control
{
	[Export]
	public LineEdit SaveIdentifierEditNode;

	public override void _Ready()
	{
		base._Ready();
		ChildEnteredTree += OnChildEnteredTree;

		SaveIdentifierEditNode.TextSubmitted += OnSaveTextSubmitted;
	}

	private void OnSaveTextSubmitted(string newText)
	{
		SaveProfile.Load(newText);
	}


	private void OnChildEnteredTree(Node node)
	{
		SaveSlotUI slot;
		if (node is SaveSlotUI converted) slot = converted;
		else return;

		if (slot.EmptySlot)
		{
			SaveIdentifierEditNode.AddSelf();
			return;
		}
	}
}
