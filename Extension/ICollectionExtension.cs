using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Storage;

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

    public static string ToStringList<T>(this ICollection<T> collection, string separator = "|")
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
}
