using ChessLike.Entity;

namespace Godot;

[GlobalClass]
public partial class MobDisplay : Godot.Node3D
{

    public static readonly Mesh DEFAULT = new SphereMesh()
    { 
        Radius = 0.5f,
        Height = 1.0f,
    };

    private List<Mob> mobs = new();
    Dictionary<Mob, MeshInstance3D> mesh_pool = new();

    public override void _Ready()
    {
        base._Ready();
    }

    public void AddMob(Mob mob)
    {
        if (!HasMob(mob)){
            mobs.Add(mob);
        }

        MeshInstance3D instance = new MeshInstance3D();
        mesh_pool[mob] = instance;
        AddChild(instance);
        SetMobMesh(mob, (Mesh)DEFAULT.Duplicate());
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

    public bool HasMeshInstance(Mob mob)
    {
        MeshInstance3D? output;
        bool exists = mesh_pool.TryGetValue(mob, out output);
        //If it doesn't exist or it is not valid.
        if(!exists || !IsInstanceValid(output))
        {
            return false;
        }
        return true;
    }

    public void EnsureMobAndMesh(Mob mob)
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
        if (failed)
        {
            AddMob(mob);
        }
    }


    public MeshInstance3D GetMobMeshInstance(Mob mob)
    {
        EnsureMobAndMesh(mob);
        return mesh_pool[mob];
    }

    public void SetMobMesh(Mob mob, Mesh mesh)
    {
        EnsureMobAndMesh(mob);

        MeshInstance3D instance = GetMobMeshInstance(mob);
        instance.Mesh = mesh;
    }

    public void UpdateMeshPositions()
    {
        foreach (Mob mob in mobs)
        {
            MeshInstance3D instance = GetMobMeshInstance(mob);
            instance.Position = mob.Position.ToGVector3();
        }
    }

    public class MeshPreset
    {
        public static readonly Mesh CELL = new BoxMesh()
        { 
            Size = new Godot.Vector3(1,1,1)
        };

        public static readonly Mesh MOB = new SphereMesh()
        { 
            Radius = 0.5f
        };
    }

}
