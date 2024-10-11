using System.Security.AccessControl;
using System.Security.Cryptography;

namespace ChessLike.Shared.GenericStruct;
public struct Flags<TEnum> where TEnum : notnull, Enum
{
    public static readonly Flags<TEnum> Empty = new();

    private Dictionary<TEnum, bool> Contents = new() { };

    public Flags()
    {
        Contents = new();
    }

    public Flags(TEnum[] flags)
    {
        Contents = new();
        foreach (TEnum flag in flags)
        {
            SetFlag(flag, true);
        }
    }

    public void SetFlag(TEnum flag, bool state)
    {
        Contents[flag] = state;
    }

    public bool GetFlag(TEnum flag)
    {
        bool output = false;
        Contents.TryGetValue(flag, out output);

        if (Contents.Keys.Contains(flag) && Contents[flag] != output)
        {
            Console.WriteLine("Error getting flag! output is different from the actual value.");
        }
        return output;
    }

    public static bool AContainsAllInB(Flags<TEnum> a, Flags<TEnum> b)
    {
        List<TEnum> set_a = a.GetAllFlags();
        List<TEnum> set_b = b.GetAllFlags();
        foreach (var flag_b in set_b)
        {
            if (!set_a.Contains(flag_b))
            {
                return false;
            }
        }
        return true;
    }

    public bool Contains(Flags<TEnum> flags)
    {
        return AContainsAllInB(this, flags);
    }


    public readonly List<TEnum> GetAllFlags()
    {
        List<TEnum> output = new();
        foreach (var pair in Contents)
        {
            if (pair.Value == true)
            {
                output.Append(pair.Key);
            }
        }
        return output;
    }

    public static bool operator ==(Flags<TEnum> a, Flags<TEnum> b)
    {
        return AContainsAllInB(a, b);
    }

    public static bool operator !=(Flags<TEnum> a, Flags<TEnum> b)
    {
        return !(a == b);
    }

    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object? obj) => obj is Flags<TEnum> typed && typed.Contents == Contents;
}

