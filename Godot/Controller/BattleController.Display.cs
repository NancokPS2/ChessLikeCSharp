using ChessLike.Entity;
using ChessLike.World;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    //public MobUI display_mob_ui = new( new Mob() );
    public MobDisplay display_mob = new();
    public GridDisplay display_grid = new();
    public Camera camera = new();

    public void SetupDisplay()
    {
        AddChild(display_mob);
        display_mob.Add(encounter.PresetMobSpawns.Values.ToList());
        display_mob.SetMobInUI(encounter.PresetMobSpawns.Values.ToList()[0]);
        display_mob.Name = "MobDisplay";

        AddChild(display_grid);
        display_grid.SetGrid(encounter.Grid);
        display_grid.Name = "GridDisplay";

        //display_mob_ui.ActionPressed += OnActionPressed;
        //AddChild(display_mob_ui);
        //display_mob_ui.Name = "Mob_UI";

        AddChild(camera);
        camera.Name = "Camera";
    }

    public void OnActionPressed(Action action)
    {
        action_selected = action;
    }

}
