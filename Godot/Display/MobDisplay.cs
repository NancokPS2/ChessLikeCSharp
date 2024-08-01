using ChessLike.Entity;

namespace Godot;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>
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
    Dictionary<Mob, Label3D> name_tag_pool = new();

    public override void _Ready()
    {
        base._Ready();
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

        SetMobMesh(mob, (Mesh)DEFAULT.Duplicate());
        SetMobNameTag(mob);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (mobs.Count > 0)
        {
            UpdateNameTagPositions();
            UpdateMeshPositions();
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

    //MeshInstance3D
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

    //Name tag.
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
