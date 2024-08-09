using System.Security.Cryptography;

namespace ChessLike.Shared.GenericStruct;
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
public struct Flags
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    public static readonly Flags Empty = new();

    private Dictionary<string, bool> Contents = new() { };

    public Flags()
    {
        Contents = new();
    }

    public Flags(string[] flags)
    {
        Contents = new();
        foreach (string flag in flags)
        {
            SetFlag(flag, true);
        }
    }

    public void SetFlag(string flag_name, bool state)
    {
        Contents[flag_name] = state;
    }

    public void SetFlag(int flag_name, bool state)
    {
        SetFlag(flag_name.ToString(), state);
    }

    public bool GetFlag(string flag)
    {
        bool output = false;
        Contents.TryGetValue(flag, out output);

        if (Contents.Keys.Contains(flag) && Contents[flag] != output)
        {
            Console.WriteLine("Error getting flag! output is different from the actual value.");
        }
        return output;
    }

    public static bool AContainsAllInB(Flags a, Flags b)
    {
        string[] set_a = a.GetAllFlags();
        string[] set_b = b.GetAllFlags();
        foreach (string flag_b in set_b)
        {
            if (!set_a.Contains(flag_b))
            {
                return false;
            }
        }
        return true;
    }

    public bool Contains(Flags flags)
    {
        return AContainsAllInB(this, flags);
    }


    public readonly string[] GetAllFlags()
    {
        string[] output = new string[0];
        foreach (string key in Contents.Keys)
        {
            if (Contents[key] == true)
            {
                output.Append(key);
            }
        }
        return output;
    }

    public static bool operator ==(Flags a, Flags b)
    {
        return AContainsAllInB(a, b);
    }

    public static bool operator !=(Flags a, Flags b)
    {
        return !(a == b);
    }

    public override readonly int GetHashCode()
    {
        return GetHashCode();
    }
}

