using Godot;

[GlobalClass, Tool]
public partial class PopupText3D : CpuParticles3D
{
    [Export]
    public Godot.Font Font
    {
        set
        {
            font = value;
            GetTextMesh().Font = font;
        }
        get => font;
    }
    private Godot.Font font;

    [Export]
    public string Text
    {
        set
        {
            text = value;
            GetTextMesh().Text = text;
        }

        get => text;

    }
    private string text = "";

    public PopupText3D() { }

    public override void _Ready()
    {
        base._Ready();
    }

    private TextMesh GetTextMesh() => (TextMesh)Mesh ?? throw new Exception("No TextMesh assigned.");

    public string GetText()
    {
        return GetTextMesh().Text;
    }

    public Godot.Font GetFont()
    {
        return GetTextMesh().Font;
    }

}
