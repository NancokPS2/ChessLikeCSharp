using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using static Godot.Vector3;
using static Godot.Vector2;

namespace ChessLike;

public static class Node3DExtension
{
    public static Godot.Vector3 GetFacingHorizontalInteger(this Node3D @this )
    {
        Godot.Vector3 facing_vector = @this.Basis.Z;
        if (facing_vector.Length()== 0)
        {
            throw new Exception("How?");
        }

        Godot.Vector2 vector = new Godot.Vector2(facing_vector.X, facing_vector.Z);
        Godot.Vector2.Axis longest_axis = vector.MaxAxisIndex();

        if (longest_axis == Godot.Vector2.Axis.X)
        {
            if (vector[(int)longest_axis] > 0)
            {
                return Godot.Vector3.Right;
            }
            else
            {
                return Godot.Vector3.Left;
            }
        }
        else
        {
            if (vector[(int)longest_axis] > 0)
            {
                return Godot.Vector3.Back;
            }
            else
            {
                return Godot.Vector3.Forward;
            }
        }
    }

    public static Godot.Vector3 GetFacingGlobalHorizontalInteger(this Node3D @this )
    {
        Godot.Vector3 facing_vector = @this.GlobalBasis.Z;
        if (facing_vector.Length()== 0)
        {
            throw new Exception("How?");
        }

        Godot.Vector2 vector = new Godot.Vector2(facing_vector.X, facing_vector.Z);
        Godot.Vector2.Axis longest_axis = vector.MaxAxisIndex();

        if (longest_axis == Godot.Vector2.Axis.X)
        {
            if (vector[(int)longest_axis] > 0)
            {
                return Godot.Vector3.Right;
            }
            else
            {
                return Godot.Vector3.Left;
            }
        }
        else
        {
            if (vector[(int)longest_axis] > 0)
            {
                return Godot.Vector3.Back;
            }
            else
            {
                return Godot.Vector3.Forward;
            }
        }
    }
}
