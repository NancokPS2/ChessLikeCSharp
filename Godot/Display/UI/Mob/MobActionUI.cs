using ChessLike.Entity;
using Godot;
using Action = ChessLike.Entity.Action;

public partial class MobActionUI : Control, ISceneDependency
{
	public delegate void ActionPress(Action action);
	public event ActionPress? ActionPressed;

    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobEquipmentUI.tscn";

    [Export]
	public Control? ActionGrid;

    public override void _Ready()
    {
        base._Ready();
		ActionGrid ??= (Control)FindChild("ActionGrid");
    }

    public void Update(Mob mob)
    {
		if(ActionGrid is null) {throw new Exception("No ActionList");}

        ActionGrid.FreeChildren();
		foreach (var item in mob.GetActions())
		{
			Label label = new(){Text = item.Name, SizeFlagsHorizontal = SizeFlags.ExpandFill};
			ActionGrid.AddChild(label);

		}
    }
}
