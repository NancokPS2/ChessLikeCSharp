using Godot;
using System;

namespace Godot;

public partial class PopupText : CpuParticles3D
{
	public Godot.Font Font = Global.Readonly.FONT_HEADING;
	public string Text = "";

    public override void _Ready()
    {
        base._Ready();
		SetText(Text);
		SetFont(Font);
    }

    private TextMesh GetTextMesh() => (TextMesh)Mesh ?? throw new Exception("No TextMesh assigned.");

    private void SetText(string value)
    {
        GetTextMesh().Text = value;
    }
    private string GetText()
    {
        return GetTextMesh().Text;
    }

    private void SetFont(Godot.Font font)
	{
		GetTextMesh().Font = font;
	}

	private Godot.Font GetFont()
	{
		return GetTextMesh().Font;
	}

}
