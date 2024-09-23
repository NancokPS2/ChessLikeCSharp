using ChessLike.Extension;
using Godot;

public static partial class Global
{
    public static class Resources
    {
        public enum MeshIdent
        {
            DEFAULT,
            CELL_FULL,
            PLANE,
            CURSOR,
            TARGETING_TARGETABLE,
            TARGETING_AOE,
            SPAWNPOINT,
        
        }
        static Dictionary<MeshIdent, string> MeshDict = new(){
            {MeshIdent.DEFAULT,"res://assets/test.tres"},
            {MeshIdent.CELL_FULL,"res://assets/Model/terrain/basic_full.res"},
            {MeshIdent.PLANE,"res://assets/Model/terrain/LowPlane.tres"},
            {MeshIdent.CURSOR,"res://assets/Model/Cursor.glb"},
            {MeshIdent.TARGETING_AOE,"res://assets/Model/cell_object/targeting_AoE.res"},
            {MeshIdent.TARGETING_TARGETABLE,"res://assets/Model/cell_object/targeting_Targetable.res"},
            {MeshIdent.SPAWNPOINT,"res://assets/Model/cell_object/SpawnPoint_default.res"},
        };

        public static Mesh GetMesh(MeshIdent identifier)
        {
            string path = MeshDict[identifier];
            Resource output = GD.Load<Resource>(path);
            if (output is PackedScene packed)
            {
                return packed.GetMeshFromModel();
            }
            else if (output is Mesh mesh)
            {
                return mesh;                
            }
            else
            {
                throw new Exception("Could not get a mesh from the given Resource");
            }

        }
    }
    //public static Directory directory = new();

}



