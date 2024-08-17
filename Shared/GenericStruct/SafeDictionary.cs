using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class SafeDictionary<TKey, TValue> where TKey : notnull where TValue : notnull //: Dictionary<TKey, TValue> where TKey : notnull
{
    private Dictionary<TKey, TValue> contents = new();
    public void Add(TKey t1, TValue t2)
    {
        contents.Add(t1, t2);
    }

    public TValue Get(TKey key)
    {
        TValue output;
        contents.TryGetValue(key, out output);
        return output;
    }
    public void Clear()
    {
        contents.Clear();
    }

    public TValue this[TKey index]
    {
        get { return Get(index); }
        set { Add(index, value); }
    }

}
