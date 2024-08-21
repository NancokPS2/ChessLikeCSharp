using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>
[GlobalClass]
public partial class MobDisplay : Godot.Node3D
{
    UniqueList<Mob> mobs = new();
    MobUI mob_ui = new();

    public override void _Ready()
    {
        base._Ready();
        IGSceneAdapter.Setup(mob_ui, this);
    }

    public void Add(Mob mob)
    {
        //Return if it already exists.
        if(!mobs.Add(mob))
        {
            GD.PushError("This mob was already added.");
            return;
        }

        MobDisplayComponent components = AddComponents(mob);
        components.AddToDisplay(this);
    }

    public void Add(List<Mob> mobs)
    {
        foreach (Mob mob in mobs)
        {
            Add(mob);
        }

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (mobs.Count > 0)
        {
            UpdateComponentPositions();
            mob_ui.UpdateStatNodes();
        }
    }

    public void RemoveMob(Mob mob)
    {
        RemoveComponents(mob);
        mobs.Remove(mob);
        
    }

    public List<Mob> GetMobs()
    {
        return mobs;
    }

    public bool HasMob(Mob mob)
    {
        return mobs.Contains(mob);
    }

    public void SetMobInUI(Mob mob)
    {
        mob_ui.SetMob(mob);
    }

}
