using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Storage;
using Godot;

namespace ChessLike.Extension;

public static class ICollectionExtension
{
    public static bool ContainsAll<T>(this ICollection<T> collection, ICollection<T> target)
    where T : notnull
    {
        foreach (var item in collection)
        {
            if (!target.Contains(item))
            {
                return false;
            }
        }
        return true;
    }
    public static string ToStringList<TKey, TValue>(this Dictionary<TKey, TValue> @this)
    where TKey : notnull
    where TValue : notnull
    {
        string output = "";
        foreach (var item in @this)
        {
            output += item.Key.ToString() + ": " + item.Value.ToString() + "\n";
        }
        return output;
    }

    public static string ToStringList<TKey, TValue>(this Dictionary<TKey, TValue> @this, string prefix = "")
    where TKey : notnull
    where TValue : notnull
    {
        string output = "";
        foreach (var item in @this)
        {
            output += prefix + item.Key.ToString() + ": " + item.Value.ToString() + "\n";
        }
        return output;
    }

    public static string ToStringList<T>(this ICollection<T> collection, string separator = " | ")
    where T : notnull
    {
        string output = "";
        if (collection.Count() == 1)
        {
            output = collection.First().ToString() ?? throw new Exception();
        }
        else
        {
            foreach (var item in collection)
            {
                output += item.ToString() + separator;
            }

        }
        output.TrimSuffix(separator);
        return output;
    }
    public static string ToStringList<T>(this IEnumerable<T> collection, string separator = "|")
    where T : notnull
    {
        string output = "";
        if (collection.Count() == 1)
        {
            output = collection.First().ToString() ?? "";
        }
        else
        {
            foreach (var item in collection)
            {
                output += item.ToString() + separator;
            }

        }
        return output;
    }

    public static TColl? GetRandom<TColl>(this List<TColl> collection, TColl? def)
    {
        if (collection.IsEmpty()) return def;
        else return GetRandom(collection);
    }

    public static TColl? GetRandom<[MustBeVariant] TColl>(this Godot.Collections.Array<TColl> collection, TColl? def)
    {
        if (collection.IsEmpty()) return def;
        else return GetRandom(collection);
    }

    public static TColl GetRandom<[MustBeVariant] TColl>(this Godot.Collections.Array<TColl> collection)
        => GetRandom(new List<TColl>(collection));

    public static TColl GetRandom<TColl>(this List<TColl> collection)
    {
        RandomNumberGenerator rng = new();
        int index = rng.RandiRange(0, collection.Count-1);
        return collection[index];
    }

    public static bool IsEmpty<T>(this IEnumerable<T> coll)
        => coll.Count() == 0;
    public static bool IsEmpty<T>(this ICollection<T> coll)
        => coll.Count == 0;
}
