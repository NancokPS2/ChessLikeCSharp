using ExtendedXmlSerializer;
using Godot;
using System;

[GlobalClass]
public partial class CombatUI : Control
{
	public override void _Ready()
	{
		base._Ready();
		EventBus.CombatStateChanged += OnCombatStateChanged;
		this.RecordOriginalParent();
	}

	#region Event Handling
	private void OnCombatStateChanged(ECombatState obj)
	{
		if (obj == ECombatState.PREPARATION)
		{
			this.RemoveSelf();
		}
		else if (!IsInsideTree())
		{
			this.ReturnSelf();
		}
	}
	#endregion
}
