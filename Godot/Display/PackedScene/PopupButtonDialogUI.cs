using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PopupButtonDialogUI : CanvasLayer, ISceneDependency
{
	public const int NO_INDEX = -1;
    public string SCENE_PATH { get; } = "res://Godot/Display/PackedScene/PopupButtonDialogUI.tscn";

	public enum EConfirmCancel {CONFIRM, CANCEL}

	public delegate void ActionIndex(int index);
	public event ActionIndex? IndexPressed;

	[Export]
	PackedScene? ButtonScene;

	[Export]
	Control? ButtonContainer;

	[Export]
	Label? MessageLabel; 

	[Export]
	public Godot.Color Modulation = new(1,1,1);
	[Export]
	public Theme? Theme;
	[Export]
	public string Message = "";

	public int IndexLastPressed = NO_INDEX;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Reload();
	}	

	public void Reload()
	{
		IndexLastPressed = NO_INDEX;

		if (IsInsideTree())
		{
			Panel main = (Panel)GetChild(0);
			if (Theme is not null)
			{
				main.Theme = Theme;
			}
			main.Modulate = Modulation;

			ButtonContainer = (Control?)(ButtonContainer ?? FindChild(nameof(ButtonContainer)));

			MessageLabel = (Label?)(MessageLabel ?? FindChild(nameof(MessageLabel)));	

			MessageLabel.Text = Message;
		}
	}

	private void CreateButtons(string[] names)
	{
		ButtonContainer.FreeChildren();

        for (int i = 0; i < names.Length; i++)
		{
            string? item = names[i];

			Button button;
			if (ButtonScene is not null)
			{
				button = ButtonScene.Instantiate<Button>();
			}
			else
			{
				button = new();
			}

            button.Text = item;
			button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

			int another_val = i;
			button.Pressed += () => {IndexPressed?.Invoke(another_val); IndexLastPressed = another_val; this.RemoveSelf();};
			ButtonContainer.AddChild(button);
		}
		var last_node = ButtonContainer.GetChildren().Last();
		if (last_node is Button last_button)
		{
			last_button.GrabFocus();
		}
	}

	public PopupButtonDialogUI SetMessage(string message)
	{
		Message = message;
		return this;
	}
	public PopupButtonDialogUI SetModulation(Godot.Color color)
	{
		Modulation = color;
		return this;
	}
	public PopupButtonDialogUI SetTheme(Theme theme)
	{
		Theme = theme;
		return this;
	}

	public void Setup(SceneTree tree, string[] action_names)
	{
		Reload();
		tree.Root.AddChild(this);
		CreateButtons(action_names);
	}

	public void Setup(Node node_in_tree, string[] action_names)
	{
		Setup(node_in_tree, action_names);
	}


	public void Setup<TEnum>(SceneTree tree) where TEnum : notnull, Enum
	{
		Setup(tree, Enum.GetNames(typeof(TEnum)));
	}

	public void Setup<TEnum>(Node node_in_tree) where TEnum : notnull, Enum
	{
		SceneTree tree = node_in_tree.GetTree();
		if (tree is null){throw new ArgumentException("Node must be in the tree.");}
		Setup(tree, Enum.GetNames(typeof(TEnum)));
	}

	public void Remove()
	{
		this.RemoveSelf();
	}
}
