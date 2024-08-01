using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>
[GlobalClass]
public partial class MobDisplay : Godot.Node3D
{
    List<Mob> mobs = new();

    public override void _Ready()
    {
        base._Ready();
        AddChild(mob_ui);
        SetUIAnchors(0,0.7f, 0.4f, 1);
    }

    public void AddMob(Mob mob)
    {
        if (!HasMob(mob)){
            mobs.Add(mob);
        }

        MeshInstance3D mesh_instance = new MeshInstance3D();
        Label3D label = new Label3D() {Billboard = BaseMaterial3D.BillboardModeEnum.Enabled};

        mesh_pool[mob] = mesh_instance;
        name_tag_pool[mob] = label;

        AddChild(mesh_instance);
        AddChild(label);

        SetMobMesh(mob, (Mesh)MeshPreset.MOB.Duplicate());
        SetMobNameTag(mob);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (mobs.Count > 0)
        {
            UpdateNameTagPositions();
            UpdateMeshPositions();
            UpdateUI();
        }
    }

    public void RemoveMob(Mob mob)
    {
        mobs.Remove(mob);
        mesh_pool.Remove(mob);
    }

    public List<Mob> GetMobs()
    {
        return mobs;
    }

    public bool HasMob(Mob mob)
    {
        return mobs.Contains(mob);
    }

    public void EnsureMobValid(Mob mob)
    {
        bool failed = false;
        if (!HasMob(mob))
        {
            GD.PushError("Tried to work on a Mob that has not been added.");
            failed = true;
        }
        if (!HasMeshInstance(mob))
        {
            GD.PushError("No mesh had been set.");
            failed = true;
        }
        if (!HasNameTag(mob))
        {
            GD.PushError("No mesh had been set.");
            failed = true;
        }
        if (failed)
        {
            AddMob(mob);
        }
    }

}
