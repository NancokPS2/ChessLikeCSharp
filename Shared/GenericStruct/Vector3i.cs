using System;

public struct Vector3i : IEquatable<Vector3i>, IFormattable, IComparer<Vector3i>
{
	public static readonly Vector3i INVALID = new Vector3i(int.MinValue,int.MinValue,int.MinValue);
	public static readonly Vector3i ZERO = new Vector3i(0,0,0);
	public static readonly Vector3i UP = new Vector3i(0,1,0);
	public static readonly Vector3i DOWN = new Vector3i(0,-1,0);
	public static readonly Vector3i LEFT = new Vector3i(-1,0,0);
	public static readonly Vector3i RIGHT = new Vector3i(1,0,0);
	public static readonly Vector3i BACK = new Vector3i(0,0,-1);
	public static readonly Vector3i FORWARD = new Vector3i(0,0,1);
	public static readonly Vector3i[] DIRECTIONS = new[]{UP,DOWN,LEFT,RIGHT,BACK,FORWARD};

	public int X;
	public int Y;
	public int Z;

	public Vector3i(int xArg, int yArg, int zArg)
	{
		this.X = xArg;
		this.Y = yArg;
		this.Z = zArg;
	}
	public Vector3i(int all_coordinates)
	{
		X = all_coordinates;
		Y = all_coordinates;
		Z = all_coordinates;
	}
	public Vector3i(Vector3 v1)
	{
		this.X = (int)(v1.X);
		this.Y = (int)(v1.Y);
		this.Z = (int)(v1.Z);
	}

	public int DistanceManhattanTo(Vector3i other)
	{
		int output = 0;
		output += Math.Abs(X - other.X);
		output += Math.Abs(Y - other.Y);
		output += Math.Abs(Z - other.Z);
		return output;
	}

    public bool Equals(Vector3i other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
		return string.Format("Vector3i({0}, {1}, {2})", this.X, this.Y, this.Z);
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

    public int Compare(Vector3i a, Vector3i b)
    {
		int value = a.X + a.Y + a.Z;
		int other_value = b.X + b.Y + b.Z;
		return value - other_value;
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

	public static Vector3i operator *(Vector3i v1, int v2 )
	{
		return new Vector3i(v1.X * v2, v1.Y * v2, v1.Z * v2);
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

