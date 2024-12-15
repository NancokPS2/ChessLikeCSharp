using ChessLike.Entity;

namespace Godot.Display;


/// <summary>
/// Add Mobs with AddMob() to make them appear in the scene.
/// </summary>

public partial class MobMeshDisplay : Godot.Node3D
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
        MobComponents[mob].FreeNodes();
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
            MobDisplayComponent component = MobComponents[mob];
            //Invisible if not in combat.
            component.MeshInstance.Visible = mob.MobState == EMobState.COMBAT;
            component.MeshInstance.Position = mob.Position.ToGVector3();
            component.NameTag.Position = mob.Position.ToGVector3() + Vector3.Up;
        }
    }

    
    public class MobDisplayComponent
    {
        public MeshInstance3D MeshInstance = new();
        public Label3D NameTag = new(){Billboard = BaseMaterial3D.BillboardModeEnum.Enabled};

        public void FreeNodes()
        {
            NameTag.QueueFree();
            MeshInstance.QueueFree();
        }

        public void SetMesh(Mesh mesh)
        {
            this.MeshInstance.Mesh = mesh;
        }
        
        public void SetNameTag(string name)
        {
            this.NameTag.Text = name;
        }

        public void AddToDisplay(MobMeshDisplay mob_display)
        {
            mob_display.AddChild(NameTag);
            mob_display.AddChild(MeshInstance);
        }

        public Vector3 GetPositionGlobal()
        {
            return MeshInstance.GlobalPosition;
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
