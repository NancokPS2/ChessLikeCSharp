using ChessLike.Entity;
using ChessLike.World;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    public MobDisplay display_mob = new();
    public GridNode display_grid = new();
    public Camera display_camera = new();

    public void SetupDisplay()
    {
        AddChild(display_mob);
        display_mob.Name = "MobDisplay";
        display_mob.mob_ui.ActionPressed += OnActionPressed;

        AddChild(display_grid);
        display_grid.SetGrid(encounter.Grid);
        display_grid.Name = "GridDisplay";

        AddChild(display_camera);
        display_camera.Name = "Camera";
    }

    public void OnActionPressed(Action action)
    {
        action_selected = action;
    }

}
