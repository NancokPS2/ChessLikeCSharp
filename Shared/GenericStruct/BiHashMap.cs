using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class BiHashMap<TKey, TValue> where TKey : notnull where TValue : notnull //: Dictionary<TKey, TValue> where TKey : notnull
{
    private Dictionary<TKey, TValue> _forward = new();
    private Dictionary<TValue, TKey> _reverse = new();

    public Indexer<TKey, TValue> Forward { get; private set; }
    public Indexer<TValue, TKey> Reverse { get; private set; }


    public BiHashMap()
    {
        this.Forward = new(_forward);
        this.Reverse = new(_reverse);
    }

    public class Indexer<TKeyIndexer, TValueIndexer> where TKeyIndexer : notnull where TValueIndexer : notnull
    {
        private Dictionary<TKeyIndexer, TValueIndexer> _dictionary;
        public Indexer(Dictionary<TKeyIndexer, TValueIndexer> dictionary)
        {
            _dictionary = dictionary;
        }
        public TValueIndexer this[TKeyIndexer index]
        {
            get { return _dictionary[index]; }
            set { _dictionary[index] = value; }
        }
    }

    public void Add(TKey t1, TValue t2)
    {
        _forward.Add(t1, t2);
        _reverse.Add(t2, t1);
    }
    public void AddReversed(TValue t1, TKey t2)
    {
        _forward.Add(t2, t1);
        _reverse.Add(t1, t2);
    }

    public TValue Get(TKey key)
    {
        return Forward[key];
    }
    public TKey GetReversed(TValue key)
    {
        return Reverse[key];
    }
    public void Clear()
    {
        _forward.Clear();
        _reverse.Clear();
    }

    public TValue this[TKey index]
    {
        get { return Get(index); }
        set { Add(index, value); }
    }
    public TKey this[TValue index]
    {
        get { return GetReversed(index); }
        set { AddReversed(index, value); }
    }

}
