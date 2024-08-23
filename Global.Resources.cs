using Godot;

public static partial class Global
{
    public static class Files
    {
        public enum MeshIdent
        {
            DEFAULT,
            CELL,
        }
        static Dictionary<MeshIdent, string> MeshDict = new(){
            {MeshIdent.DEFAULT,"res://assets/test.tres"},
            {MeshIdent.CELL,"res://assets/test.tres"},
        };

        public static Mesh GetMesh(MeshIdent identifier)
        {
            string path = MeshDict[identifier];
            Godot.Mesh output = GD.Load<Mesh>(path);
            return output;
        }
    }
    //public static Directory directory = new();

}



