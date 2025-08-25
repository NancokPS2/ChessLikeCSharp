using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class InputManager : Node
{
	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if (@event.IsActionPressed("cancel"))
		{
			EventBus.InputBack?.Invoke();
		}
		else if (@event.IsActionPressed("pause"))
		{
			EventBus.InputPause?.Invoke();
		}
	}
}
