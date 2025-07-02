using Godot;
using System;

namespace Godot;

public partial class PopupText3D : CpuParticles3D
{
	private Godot.Font Font = Global.ManagerFont.GetResource("Regular");
	private string Text = "";

    public override void _Ready()
    {
        base._Ready();
		SetText(Text);
		SetFont(Font);
    }

    private TextMesh GetTextMesh() => (TextMesh)Mesh ?? throw new Exception("No TextMesh assigned.");

    public void SetText(string value)
    {
        GetTextMesh().Text = value;
    }

    public string GetText()
    {
        return GetTextMesh().Text;
    }

    public void SetFont(Godot.Font font)
	{
		GetTextMesh().Font = font;
	}

	public Godot.Font GetFont()
	{
		return GetTextMesh().Font;
	}

}
