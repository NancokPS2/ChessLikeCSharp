using ChessLike.Entity;
using Godot.Display.Interface;

namespace Godot.Display;

public partial class MobDisplay : Godot.Node3D
{
    MobUI mob_ui = new(new Mob());

    public void SetMobInUI(Mob mob)
    {
        mob_ui.SetMob(mob);
    }

    public void UpdateUI()
    {
        mob_ui.UpdateStatNodes();
    }

    public void SetUIAnchors(float left, float top, float right, float bottom)
    {
        mob_ui.SetAnchor(Side.Left, left);
        mob_ui.SetAnchor(Side.Top, top);
        mob_ui.SetAnchor(Side.Right, right);
        mob_ui.SetAnchor(Side.Bottom, bottom);
    }
}