using Godot;

public partial class UIManager
{
    public partial class CanvasWithTarget: CanvasLayer
    {
        public Node2D DrawTarget = new();
        public override void _Ready()
        {
            base._Ready();
            AddChild(DrawTarget);
        }
    }
}
