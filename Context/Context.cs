using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Turn;
using Godot;

namespace ChessLike.Context;

public abstract class Context
{
	public delegate void Step<TEnum>(Context context, TEnum enumerator) where TEnum : notnull, Enum;

	//public abstract void Finish();
}
