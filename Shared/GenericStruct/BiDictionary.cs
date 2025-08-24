using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class BiDictionary<TKey, TValue> where TKey : notnull where TValue : notnull //: Dictionary<TKey, TValue> where TKey : notnull
{
	private Dictionary<TKey, TValue> _forward = new();
	private Dictionary<TValue, TKey> _reverse = new();

	public Indexer<TKey, TValue> Forward { get; private set; }
	public Indexer<TValue, TKey> Reverse { get; private set; }

	public Dictionary<TKey, TValue>.KeyCollection Keys => _forward.Keys;

	public Dictionary<TKey, TValue>.ValueCollection Values => _forward.Values;

	public BiDictionary()
	{
		this.Forward = new(_forward);
		this.Reverse = new(_reverse);
	}

	public int Count() => _forward.Count();

	public Dictionary<TKey, TValue> GetDictForward() => new(_forward);
	public Dictionary<TValue, TKey> GetDictReverse() => new(_reverse);

	protected void AddForward(TKey t1, TValue t2)
	{
		_forward.Add(t1, t2);
		_reverse.Add(t2, t1);
	}
	protected void AddReversed(TValue t1, TKey t2)
	{
		_forward.Add(t2, t1);
		_reverse.Add(t1, t2);
	}

	protected TValue GetForward(TKey key)
		=> Forward[key];

	protected TKey GetReversed(TValue key)
		=> Reverse[key];

	public void Clear()
	{
		_forward.Clear();
		_reverse.Clear();
	}

	public TValue this[TKey index]
	{
		get { return GetForward(index); }
		set { AddForward(index, value); }
	}

	public TKey this[TValue index]
	{
		get { return GetReversed(index); }
		set { AddReversed(index, value); }
	}

	public bool ContainsKey(TKey key) => _forward.ContainsKey(key);
	public bool ContainsValue(TValue value) => _reverse.ContainsKey(value);

	#region Indexer
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
	#endregion
}
