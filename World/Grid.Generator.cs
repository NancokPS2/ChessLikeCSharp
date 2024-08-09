namespace ChessLike.World;

public partial class Grid
{
    public class Generator
    {
        public static Grid GenerateFlat(Vector3i size)
        {
            Grid output = new();
            int[] X = Enumerable.Range(0, (int)size.X).ToArray();
            int[] Y = Enumerable.Range(0, (int)size.Y).ToArray();
            int[] Z = Enumerable.Range(0, (int)size.Z).ToArray();
            foreach (int ind_x in X)
            {
                foreach (int ind_y in X)
                {
                    foreach (int ind_z in X)
                    {
                        if (ind_y < 1)
                        {
                            output.SetCell(new Vector3i(ind_x,ind_y,ind_z), Cell.Preset.Floor);
                        }else
                        {
                            output.SetCell(new Vector3i(ind_x,ind_y,ind_z), Cell.Preset.Air);
                        }
                    }
                }

            }

            return output;
        }
    }
}
