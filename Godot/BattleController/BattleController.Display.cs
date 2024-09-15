using ChessLike.Entity;
using ChessLike.World;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    public MobDisplay DisplayMob = new();
    public GridNode display_grid = new();
    public Camera display_camera = new(){mode = Camera.Mode.DELEGATED_PIVOT};

    public void SetupDisplay()
    {
        AddChild(DisplayMob);
        DisplayMob.Name = "MobDisplay";
        DisplayMob.MobUINode.ActionPressed += OnActionPressed;

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
