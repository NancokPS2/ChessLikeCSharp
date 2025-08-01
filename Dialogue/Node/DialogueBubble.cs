using Godot;
using System;

public partial class DialogueBubble : Node3D
{
	[Export]
	protected Label LabelNode;

	[Export]
	protected float CharacterDelay = 0.1f;

	[Export(PropertyHint.MultilineText)]
	protected string DefaultText = "";

	protected bool DirtyLabel;

	protected string NewText = "";
	protected int NewTextIndex;
	protected double NewTextTimeSince;

	public override void _Ready()
	{
		base._Ready();
		if (DefaultText != "") SetText(DefaultText);
	}


	public void SetText(string text)
	{
		LabelNode.Text = "";
		DirtyLabel = true;
		NewTextIndex = 0;
		NewText = text;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		//Position = new(Position.X, LabelNode.GetRect().Size.Y/2, Position.Z);

		if (!DirtyLabel)
			return;

		//Finished going trough the text.
		if (NewTextIndex >= NewText.Count())
		{
			DirtyLabel = false;
		}

		while (NewTextTimeSince > CharacterDelay)
		{
			char newChar = NewText[NewTextIndex];
			LabelNode.Text += newChar;
			
			NewTextTimeSince -= CharacterDelay;
			NewTextIndex++;
		}

		NewTextTimeSince += delta;
	}

}
