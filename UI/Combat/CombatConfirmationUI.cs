using Godot;
using System;

public partial class CombatConfirmationUI : Control
{
	[Export]
	public Button? NodeConfirmButton;
	[Export]
	public TextureButton? NodeCancelButton;

    public override void _Ready()
    {
        base._Ready();
		NodeConfirmButton ??= (Button)FindChild("Confirm");
		NodeCancelButton ??= (TextureButton)FindChild("Cancel");
    }

    public void Update(bool show, string confirm_text = "EXECUTE")
	{
		ShowConfirmationButton(show);
		NodeConfirmButton.Text = confirm_text;
	}
    public void ShowConfirmationButton(bool activate)
    {
        NodeConfirmButton.Activate(activate);
        NodeCancelButton.Activate(activate);
    }
}
