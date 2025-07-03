using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;

public partial class MobActionUI : Control, ISceneDependency
{

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
		foreach (var item in mob.GetAbilities())
		{
			ActionLabel label = new(item){Text = item.Name, SizeFlagsHorizontal = SizeFlags.ExpandFill};
			ActionGrid.AddChild(label);

		}
    }

    private partial class ActionLabel : Label, ITooltip
    {
        ActionEvent Action;

        public ActionLabel(ActionEvent action)
        {
            Action = action;
        }

        string ITooltip.GetText() => Action.GetDescription();
        Godot.Vector2 ITooltip.GetRectSize() => new(200,80);
    }
}
