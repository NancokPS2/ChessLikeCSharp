using ChessLike.Entity;
using ChessLike.Turn;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>
[GlobalClass]
public partial class MobMeshDisplay : Godot.Node3D
{
    UniqueList<Mob> mobs = new();

    public override void _Ready()
    {
        base._Ready();
    }

    public void Add(Mob mob)
    {
        //Return if it already exists.
        if(!mobs.Add(mob))
        {
            GD.PushError("This mob was already added.");
            return;
        }

        MobDisplayComponent component = AddComponents(mob);
        component.AddToDisplay(this);
        SetupEventsForMob(mob);
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
        }
    }

    public void Remove(Mob mob)
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
