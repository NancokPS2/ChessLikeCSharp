using ChessLike.Entity;
using ChessLike.World;
using Godot.Display.Interface;
using Action = ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    public MobUI display_mob_ui = new( new Mob() );
    public GridDisplay display_grid = new();

    public void SetupDisplay()
    {
        display_mob_ui.ActionPressed += OnActionPressed;
        AddChild(display_mob_ui);
        AddChild(display_grid);
    }

    public void OnActionPressed(Action action)
    {
        if (state_current == State.AWAITING_ACTION)
        {
            selected_action = action;
        }
    }

}
