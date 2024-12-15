using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class UniqueList<T> : List<T>
{
    public bool Safe = true;
    public bool Add(T obj, bool safe)
    {
        if (!Contains(obj))
        {
            base.Add(obj);
            return true;
        } else if (safe)
        {
            throw new Exception("Already in list");
        }else
        {
            return false;
        }
    }

public new bool Add(T obj) => Add(obj, Safe);
}
