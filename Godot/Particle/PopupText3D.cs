using Godot;

[GlobalClass, Tool]
public partial class PopupText3D : CpuParticles3D
{
    [Export]
    private Godot.Font Font;
    [Export]
    private string text
    {
        set
        {
            Text = value;
            GetTextMesh().Text = Text;
        }

        get => Text;

    }
    private string Text = "";

    public PopupText3D() { }

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
