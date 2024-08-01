using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>

public partial class MobDisplay : Godot.Node3D
{
    Dictionary<Mob, Label3D> name_tag_pool = new();

    public void SetMobNameTag(Mob mob)
    {
        EnsureMobValid(mob);

        string name = mob.Identity.displayed_name;

        Label3D name_tag = GetMobLabel3D(mob);
        name_tag.Text = name;
    }

    public Label3D GetMobLabel3D(Mob mob)
    {
        EnsureMobValid(mob);
        return name_tag_pool[mob];
    }

    public bool HasNameTag(Mob mob)
    {
        Label3D? output;
        bool exists = name_tag_pool.TryGetValue(mob, out output);
        //If it doesn't exist or it is not valid.
        if(!exists || !IsInstanceValid(output))
        {
            return false;
        }
        return true;
    }

    public void UpdateNameTagPositions()
    {
        foreach (Mob mob in mobs)
        {
            Label3D label = GetMobLabel3D(mob);
            label.Position = mob.Position.ToGVector3() + Godot.Vector3.Up;
        }
    }
}
