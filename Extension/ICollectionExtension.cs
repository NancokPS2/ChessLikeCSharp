using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Extension;

public static class ICollectionExtension
{
    public static bool ContainsAll<T>(this ICollection<T> collection, ICollection<T> target)
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
}
