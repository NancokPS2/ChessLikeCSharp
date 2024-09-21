using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>

public partial class MobDisplay : Godot.Node3D
{
    Dictionary<Mob, MobDisplayComponent> MobComponents = new();

    public MobDisplayComponent AddComponents(Mob mob)
    {
        //Components
        MobDisplayComponent components = new();
        MobComponents[mob] = components;

        //Set component's values
        components.SetMesh( (Mesh)MeshPreset.MOB.Duplicate() );
        components.SetNameTag(mob.DisplayedName);

        return components;
    }

    public void RemoveComponents(Mob mob)
    {
        RemoveChild(MobComponents[mob].mesh_instance);
        RemoveChild(MobComponents[mob].name_tag);
        MobComponents.Remove(mob);
    }

    public MobDisplayComponent GetComponents(Mob mob)
    {
        return MobComponents[mob];
    }

    public void UpdateComponentPositions()
    {
        foreach (Mob mob in mobs)
        {
            MobDisplayComponent components = MobComponents[mob];
            components.mesh_instance.Position = mob.Position.ToGVector3();
            components.name_tag.Position = mob.Position.ToGVector3() + Vector3.Up;
        }
    }

    
    public class MobDisplayComponent
    {
        public MeshInstance3D mesh_instance = new();
        public Label3D name_tag = new(){Billboard = BaseMaterial3D.BillboardModeEnum.Enabled};

        public void SetMesh(Mesh mesh)
        {
            this.mesh_instance.Mesh = mesh;
        }
        
        public void SetNameTag(string name)
        {
            this.name_tag.Text = name;
        }

        public void AddToDisplay(MobDisplay mob_display)
        {
            mob_display.AddChild(name_tag);
            mob_display.AddChild(mesh_instance);
        }

        public Vector3 GetPositionGlobal()
        {
            return mesh_instance.GlobalPosition;
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
