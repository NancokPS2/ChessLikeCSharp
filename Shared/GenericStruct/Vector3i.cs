using System;
using System.Runtime.Intrinsics;

public struct Vector3i : IEquatable<Vector3i>, IComparer<Vector3i>
{
	public static readonly Vector3i INVALID = new Vector3i(int.MinValue,int.MinValue,int.MinValue);
	public static readonly Vector3i ZERO = new Vector3i(0,0,0);
	public static readonly Vector3i ONE = new Vector3i(1, 1,1);
	public static readonly Vector3i UP = new Vector3i(0,1,0);
	public static readonly Vector3i DOWN = new Vector3i(0,-1,0);
	public static readonly Vector3i LEFT = new Vector3i(-1,0,0);
	public static readonly Vector3i RIGHT = new Vector3i(1,0,0);
	public static readonly Vector3i BACK = new Vector3i(0,0,-1);
	public static readonly Vector3i FORWARD = new Vector3i(0,0,1);
	public static readonly Vector3i[] DIRECTIONS = new[]{UP,DOWN,LEFT,RIGHT,BACK,FORWARD};

	private int[] contents = new int[3]{0,0,0};
	public int X {get => contents[0]; set => contents[0] = value;}
	public int Y {get => contents[1]; set => contents[1] = value;}
	public int Z {get => contents[2]; set => contents[2] = value;}

	public Vector3i()
	{
		this.X = 0;
		this.Y = 0;
		this.Z = 0;
		contents = new int[3]{X,Y,Z};
	}
	public Vector3i(int xArg, int yArg, int zArg)
	{
		this.X = xArg;
		this.Y = yArg;
		this.Z = zArg;
		contents = new int[3]{X,Y,Z};
		var a = 1;
	}
	public Vector3i(int all_coordinates) : this(xArg: all_coordinates,all_coordinates,all_coordinates)
	{
	}
	public Vector3i(Vector3 v1) : this((int)v1.X, (int)v1.Y, (int)v1.Z)
	{
	}

	public int DistanceManhattanTo(Vector3i other)
	{
		int output = 0;
		output += Math.Abs(X - other.X);
		output += Math.Abs(Y - other.Y);
		output += Math.Abs(Z - other.Z);
		return output;
	}

	public List<Vector3i> GetStepsToReachVector(Vector3i location)
	{
		List<Vector3i> output = new();

		Vector3i pointing = location - this;

		while (pointing != Vector3i.ZERO)
		{
			//Get the normalized vector reduced to a length of 1, the longest side is kept as 1.
			Vector3i move = Normalized(pointing);

			//Add the move.
			output.Add(move);

			//Reduce the pointing vector by the move.
			pointing -= move;
		}

		return output;
	}

	public readonly int GetLongestIndex()
	{
		int highest_index = -1;
		int highest_value = 0;
		foreach (int index in new int[]{0,1,2})
		{
			int value = contents[index];

			if (Math.Abs(value)> highest_value)
			{
				highest_index = index;
				highest_value = Math.Abs(value);
			}
		}
		if (highest_index == -1)
		{
			throw new Exception("No value was higher than the minimum value.");
		}

		return highest_index;
	}

    public bool Equals(Vector3i other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override string ToString()
    {
		return string.Format("Vector3i({0}, {1}, {2})", this.X, this.Y, this.Z);
    }

	public static Vector3i Normalized(Vector3i vector)
	{
		int longest_index = vector.GetLongestIndex();
		Vector3i output = new Vector3i(0);
		output[longest_index] += Math.Sign(vector[longest_index]);
		return output;
		
	}

	public bool IsNormalized()
	{
		int total = ToInt();
		return total == 1;
	}
/* 
    public int CompareTo(Vector3i other)
    {
		int value = X + Y + Z;
		int other_value = other.X + other.Y + other.Z;
        return value > other_value ? 1 : -1;
    } 
*/

	public int ToInt()
	{
		return X + Y + Z;
	}

	public System.Numerics.Vector3 ToVector3()
	{
		return new Vector3(X,Y,Z);
	}

	public Godot.Vector3 ToGVector3()
	{
		return new Godot.Vector3(X, Y, Z);
	}

/* 	public override string ToString()
	{
		return string.Format("( {0} | {1} | {2} )", X.ToString(),Y.ToString(),Z.ToString());
	} */

    public int Compare(Vector3i a, Vector3i b)
    {
		int value = a.X + a.Y + a.Z;
		int other_value = b.X + b.Y + b.Z;
		return value - other_value;
    }

	public int this[int index]
	{
		get => contents[index];
		set => contents[index] = value;
	}
	
    public static Vector3i operator +(Vector3i v1, Vector3i v2)
	{
		return new Vector3i(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
	}

	public static Vector3i operator +(Vector3 v1, Vector3i v2)
	{
		return new Vector3i((int)(v1.X) + v2.X, (int)(v1.Y) + v2.Y, (int)(v1.Z) + v2.Z);
	}

	public static Vector3i operator +(Vector3i v1, Vector3 v2)
	{
		return new Vector3i(v1.X + (int)(v2.X), v1.Y + (int)(v2.Y), v1.Z + (int)(v2.Z));
	}

    public static Vector3i operator -(Vector3i v1, Vector3i v2)
	{
		return new Vector3i(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
	}

	public static Vector3i operator *(Vector3i v1, int v2 )
	{
		return new Vector3i(v1.X * v2, v1.Y * v2, v1.Z * v2);
	}

	public static bool operator >(Vector3i v1, Vector3i v2 )
	{
		return v1.ToInt() > v2.ToInt();
	}

	public static bool operator <(Vector3i v1, Vector3i v2 )
	{
		return v1.ToInt() < v2.ToInt();
	}

	public static implicit operator Vector3(Vector3i v1)
	{
		return new Vector3(v1.X, v1.Y, v1.Z);
	}

	public static bool operator ==(Vector3i a, Vector3i b)
	{
		return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
	}

	public static bool operator !=(Vector3i a, Vector3i b)
	{
		return !(a == b);
	}


    public override bool Equals(object? obj)
    {
        return obj is Vector3i && Equals((Vector3i)obj);
    }

    public override int GetHashCode()
    {
       return base.GetHashCode();
    }

}

