using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;

[GlobalClass]
public partial class MobActionUI : BaseMobUI, ISceneDependency
{

    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobEquipmentUI.tscn";

    [Export]
    public Control? ActionGrid;

    protected override void Update(Mob mob)
    {
		base.Update(mob);

        if (ActionGrid is null) { throw new Exception("No ActionList"); }

        ActionGrid.FreeChildren();
        foreach (var item in mob.GetAbilities())
        {
            ActionLabel label = new(item) { Text = item.Name, SizeFlagsHorizontal = SizeFlags.ExpandFill };
            ActionGrid.AddChild(label);

        }
    }

    private partial class ActionLabel : Label, ITooltip
    {
        Ability Action;

        public ActionLabel(Ability action)
        {
            Action = action;
        }

        string ITooltip.GetText() => Action.GetDescription();
        Godot.Vector2 ITooltip.GetRectSize() => new(200, 80);
    }
}
