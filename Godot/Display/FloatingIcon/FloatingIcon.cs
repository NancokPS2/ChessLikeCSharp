using ChessLike.Entity.Action;
using Godot;
using System;

[GlobalClass]
public partial class FloatingIcon3D : Node3D
{
    protected Sprite3D Sprite { get => sprite ?? throw new NullReferenceException(); set => sprite = value; }
    [Export]
    private Sprite3D? sprite;

    protected Label3D Label { get => text ?? throw new NullReferenceException(); set => text = value; }
    [Export]
    private Label3D? text;

    public void SetTexture(Texture2D texture) => Sprite.Texture = texture;

    public void SetCount(int count) => Label.Text = count.ToString();
}
