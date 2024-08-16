using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>
[GlobalClass]
public partial class MobDisplay : Godot.Node3D
{
    UniqueList<Mob> mobs = new();

    public override void _Ready()
    {
        base._Ready();
        AddChild(mob_ui);
        SetUIAnchors(0, 0.7f, 0.4f, 1);
    }

    public void AddMob(Mob mob)
    {
        //Return if it already exists.
        if(!mobs.Add(mob))
        {
            GD.PushError("This mob was already added.");
            return;
        }

        MobComponents components = AddComponents(mob);
        components.AddToDisplay(this);
    }

    public void AddMob(List<Mob> mobs)
    {
        foreach (Mob mob in mobs)
        {
            AddMob(mob);
        }

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (mobs.Count > 0)
        {
            UpdateComponentPositions();
            UpdateUI();
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

}
