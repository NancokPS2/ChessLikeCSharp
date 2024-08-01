using ChessLike.Entity;
using Godot.Display.Interface;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>

public partial class MobDisplay : Godot.Node3D
{
    Dictionary<Mob, MeshInstance3D> mesh_pool = new();

    public MeshInstance3D GetMobMeshInstance(Mob mob)
    {
        EnsureMobValid(mob);
        return mesh_pool[mob];
    }

    public void SetMobMesh(Mob mob, Mesh mesh)
    {
        EnsureMobValid(mob);

        MeshInstance3D instance = GetMobMeshInstance(mob);
        instance.Mesh = mesh;
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
            Radius = 0.5f,
            Height = 1.0f
        };

        public static readonly Mesh DEFAULT = new PrismMesh()
        { 
        };
    }
}
