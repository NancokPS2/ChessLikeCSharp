using ChessLike.Entity;
using ChessLike.World;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    public void OnActionPressed(Action action)
    {
        TurnActionSelected = action;
    }

}
