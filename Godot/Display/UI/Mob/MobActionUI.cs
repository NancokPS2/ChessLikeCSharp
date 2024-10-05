using ChessLike.Entity;
using Godot;
using Action = ChessLike.Entity.Action;

public partial class MobActionUI : Control, ISceneDependency
{
	public delegate void ActionPress(Action action);
	public event ActionPress? ActionPressed;

    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobEquipmentUI.tscn";

    public ItemList<Action>? ActionList;

    [Export]
	public Control? ActionGrid;

    public override void _Ready()
    {
        base._Ready();
		ActionGrid ??= (Control)FindChild("ActionGrid");

		ActionList = new(ActionGrid);
    }

    public void Update(Mob mob)
    {
		if(ActionList is null) {throw new Exception("No ActionList");}

        ActionList.ClearItems();
        foreach (var action in mob.GetActions())
        {
            var menu_item = new ItemList<Action>.MenuItem()
				.ChainSetContained(action)
            	.ChainSetText(action.Name);
            ActionList.AddItem( menu_item );
        }
    }
}
