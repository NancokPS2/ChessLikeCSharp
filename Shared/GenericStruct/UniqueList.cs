using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class UniqueList<T> : List<T>
{
    public new bool Add(T obj)
    {
        if (!Contains(obj))
        {
            base.Add(obj);
            return true;
        }
        return false;
    }
}
