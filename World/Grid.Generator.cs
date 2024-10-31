using System.Diagnostics;

namespace ChessLike.World;

public partial class Grid
{
    public class Generator
    {
        public static Grid GenerateFlat(Vector3i size)
        {
            Grid output = new();
            output.Boundary = size;

            int[] X = Enumerable.Range(0, (int)size.X).ToArray();
            int[] Y = Enumerable.Range(0, (int)size.Y).ToArray();
            int[] Z = Enumerable.Range(0, (int)size.Z).ToArray();
            foreach (int ind_x in X)
            {
                foreach (int ind_y in Y)
                {
                    foreach (int ind_z in Z)
                    {
                        Vector3i position = new(ind_x,ind_y,ind_z);
                        if (position.Y < 1)
                        {
                            output.SetCell(position, Cell.Preset.Floor);
                        }
                        else if (position.X == 0 && position.Y == 1)
                        {
                            output.SetCell(position, Cell.Preset.Spawnpoint);
                        }
                        else
                        {
                            output.SetCell(position, Cell.Preset.Air);
                        }
                    }
                }

            }
            Debug.Assert(output.CellDictionary.ContainsKey(size-Vector3i.ONE));

            return output;
        }
    }
}
