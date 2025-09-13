using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLikeCSharp.Shared.GenericStruct.Modifier;

public class ModifierCollection
{
	public enum EModifierType
	{
		CUSTOM = -1,
		INVALID = 0,
		ADDITION,
		MULTIPLICATION,
	}

	protected List<Modifier> Modifiers = new();

	public void AddModifier(EModifierType type, float value, int priority = 0)
	{
		Modifiers.Add( new(type, value, priority) );
	}

	public float Resolve(float value)
	{
		SortModifiers();
		float output = value;
		foreach (var item in Modifiers)
		{
			output = item.ResolveValue(output);
		}
		return output;
	}

	protected void SortModifiers()
	{
		Modifiers.Sort(
			SortCall
		);
	}

	private int SortCall(Modifier x, Modifier y)
	{

		int priorityDiff = x.Priority - y.Priority;

		if (priorityDiff == 0)
			priorityDiff += (int)x.Type - (int)y.Type;

		return priorityDiff;
	}

	protected class Modifier
	{
		public EModifierType Type;
		public float Value;
		public int Priority;
		public string? Expression;

		public Modifier(EModifierType type, float value, int priority)
		{
			Type = type;
			Value = value;
			Priority = priority;

			if (Type == EModifierType.CUSTOM)
				Debug.Assert(Expression is not null);
			else
				Debug.Assert(Expression is null);
		}

		public float ParseExpression(float value)
			=> throw new NotImplementedException();

		public float ResolveValue(float valueToResolve)
			=> Type switch
			{
				EModifierType.ADDITION => valueToResolve += valueToResolve,
				EModifierType.MULTIPLICATION => valueToResolve *= valueToResolve,
				EModifierType.CUSTOM => ParseExpression(valueToResolve),
				_ => throw new Exception(),
			};
	}
}


public interface IModifierOperation
{
	public float ProcessValue(float value);
}
