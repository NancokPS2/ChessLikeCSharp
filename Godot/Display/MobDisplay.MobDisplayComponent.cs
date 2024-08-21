using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>

public partial class MobDisplay : Godot.Node3D
{
    Dictionary<Mob, MobDisplayComponent> mob_components = new();

    public MobDisplayComponent AddComponents(Mob mob)
    {
        //Components
        MobDisplayComponent components = new();
        mob_components[mob] = components;

        //Set component's values
        components.SetMesh( (Mesh)MeshPreset.MOB.Duplicate() );
        components.SetNameTag(mob.Identity.Name);

        return components;
    }

    public void RemoveComponents(Mob mob)
    {
        RemoveChild(mob_components[mob].mesh_instance);
        RemoveChild(mob_components[mob].name_tag);
        mob_components.Remove(mob);
    }

    public MobDisplayComponent GetComponents(Mob mob)
    {
        return mob_components[mob];
    }

    public void UpdateComponentPositions()
    {
        foreach (Mob mob in mobs)
        {
            MobDisplayComponent components = mob_components[mob];
            components.mesh_instance.Position = mob.Position.ToGVector3();
            components.name_tag.Position = mob.Position.ToGVector3() + Vector3.Up;
        }
    }

    
    public class MobDisplayComponent
    {
        public MeshInstance3D mesh_instance = new();
        public Label3D name_tag = new();

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