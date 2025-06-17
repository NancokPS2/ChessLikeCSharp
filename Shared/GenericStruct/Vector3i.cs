using System;
using System.Diagnostics;
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

	public int X;
	public int Y;
	public int Z;

	public enum Axis
	{
		X,Y,Z
	}

	public enum Rotation
	{
		UNROTATED,
		Z_90_CW, Z_90_CCW, Z_180,
		X_90_CW, X_90_CCW, X_180,
		Y_90_CW, Y_90_CCW, Y_180
		
	}

	public Vector3i()
	{
		this.X = 0;
		this.Y = 0;
		this.Z = 0;

		//Debug.Assert(this.contents[0] == X);
		//Debug.Assert(X != null);
	}
	public Vector3i(int xArg, int yArg, int zArg)
	{
		this.X = xArg;
		this.Y = yArg;
		this.Z = zArg;
	}
	public Vector3i(int all_coordinates) : this(xArg: all_coordinates,all_coordinates,all_coordinates)
	{

	}
	public Vector3i(Vector3 v1) : this((int)v1.X, (int)v1.Y, (int)v1.Z)
	{
	}

	public Vector3i(Godot.Vector3 v1) : this((int)v1.X, (int)v1.Y, (int)v1.Z)
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

	public int DistanceManhattanWithToleranceTo(Vector3i other, Vector3i tolerance)
	{
		int output = 0;
		int dist_X = Math.Abs(X - other.X) - tolerance.X;
		int dist_Y = Math.Abs(Y - other.Y) - tolerance.Y;
		int dist_Z = Math.Abs(Z - other.Z) - tolerance.Z;
		output += 
			Math.Clamp(dist_X, 0, int.MaxValue) 
			+ Math.Clamp(dist_Y, 0, int.MaxValue) 
			+ Math.Clamp(dist_Z, 0, int.MaxValue);
		return output;
	}


	public Vector3i GetDirectionNormalizedTo(Vector3i target)
	{
		Vector3i pointing = target - this;
		return pointing;
	}

	public Vector3i GetDirectionNormalizedTo(Vector3i target, Vector3i[] allowed_dirs)
	{
		//Must have at least 1 direction and they must be one.
		if (allowed_dirs.Count() < 1 || allowed_dirs.Any(x => x.GetLength() != 1))
		{
			throw new Exception("All elements must be directions.");
		}

		Vector3i closest_pos = allowed_dirs[0] + this;
		foreach (Vector3i direction in allowed_dirs)
		{
			//Change the closest position if it is closer.
			if (
				(this + direction).DistanceManhattanTo(target) <= closest_pos.DistanceManhattanTo(target)
			)
			{
				closest_pos = this + direction;
			}
		}

		return closest_pos;
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

	public Axis? GetLongestAxis()
	{
		Axis? longestAxis = null;
		int highest_value = 0;
		foreach (Axis axis in new[]{Axis.X,Axis.Y,Axis.Z})
		{
			int value = this[axis];

			if (Math.Abs(value)> highest_value)
			{
				longestAxis = axis;
				highest_value = Math.Abs(value);
			}
		}
		if (longestAxis is null)
		{
			throw new Exception("No value was higher than the minimum value.");
		}
		else return longestAxis;
	}

	public int GetLength()
	{
		return X + Y + Z;
	}

	[Obsolete("WIP")]
	public Rotation NormalizedToRotation(Vector3i target, Axis[]? ignored = null)
	{
		Vector3i normalized = GetDirectionNormalizedTo(target);

		if (ignored?.Contains(Axis.Y) ?? false) goto xAxis;

		if (normalized == RIGHT) return Rotation.Y_90_CW;

		else if (normalized == LEFT) return Rotation.Y_90_CCW;

		else if (normalized == BACK) return Rotation.Y_180;


		xAxis:
		if (ignored?.Contains(Axis.X) ?? false) goto zAxis;


		zAxis:
		if (ignored?.Contains(Axis.Z) ?? false) goto end;

		end:
		throw new Exception();
	}

    public bool Equals(Vector3i other)
	{
		return X == other.X && Y == other.Y && Z == other.Z;
	}

    public override string ToString()
    {
		return string.Format("Vector3i({0}, {1}, {2})", this.X, this.Y, this.Z);
    }

	public Vector3i Normalized()
	{
		return Normalized(this);
	}

	public static Vector3i Normalized(Vector3i vector)
	{
		Vector3i output = new Vector3i(0);
		Axis? longestAxis =  vector.GetLongestAxis();

		if (longestAxis is Axis axis)
		{
			int axisLength = vector[axis];
			output[axis] += Math.Sign(axisLength);
		}

		return output;		
	}

	public Vector3i Rotated(Rotation rotation)
	{
		switch (rotation)
		{
			case Rotation.UNROTATED: return this;

			//Z Axis
			case Rotation.Z_90_CW: return new(Y, -X, Z);
			case Rotation.Z_90_CCW: return new(-Y, X, Z);
			case Rotation.Z_180: return new(-X, -Y, Z);

			//X Axis
			case Rotation.X_90_CW: return new(X, -Z, Y);
			case Rotation.X_90_CCW: return new(X, Z, -Y);
			case Rotation.X_180: return new(X, -Y, -Z);

			//Y Axis
			case Rotation.Y_90_CW: return new(Z, Y, -X);
			case Rotation.Y_90_CCW: return new(-Z, Y, X);
			case Rotation.Y_180: return new(-X, Y, -Z);

			default: throw new Exception("Invalid");
		}
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
		return GetLength();
	}

	public System.Numerics.Vector3 ToVector3()
	{
		return new Vector3(X,Y,Z);
	}

	public Godot.Vector3 ToGVector3()
	{
		return new Godot.Vector3(X, Y, Z);
	}

	public Godot.Vector3I ToGVector3I()
	{
		return new Godot.Vector3I(X, Y, Z);
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

	public int this[Axis index]
	{
		get => index switch{ Axis.X => X, Axis.Y => Y, Axis.Z => Z, _ => throw new Exception()};
		set => AxisSet(index, value);
	}

	private void AxisSet(Axis axis, int value)
	{
		switch(axis)
		{
			case Axis.X: 
				X = value;
				break;
			case Axis.Y: 
				Y = value;
				break;
			case Axis.Z: 
				Z = value;
				break;
			default: 
				throw new Exception();
		}
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

	public bool IsValid()
	{
		return !(
		X < -2147483630 || X > 2147483630 
		|| Y < -2147483630 || Y > 2147483630 
		|| Z < -2147483630 || Z > 2147483630);
	}

}

