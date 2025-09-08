using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

public abstract partial class BaseButtonMenu<TButton, TAssociatedParam> : Control, ITooltip
	where TButton : BaseButton, new()
{
	public delegate void ButtonInteraction(TButton button, TAssociatedParam param);
	public event ButtonInteraction? ButtonPressed;

	protected (TButton, TAssociatedParam)? TupleHovered;
	protected (TButton, TAssociatedParam)? TupleSelected;

	[Export]
	public Control? ContainerOverride;

	[Export]
	public SizeFlags ButtonHorizontalFlags = SizeFlags.ExpandFill;

	[Export]
	public SizeFlags ButtonVerticalFlags = SizeFlags.ShrinkBegin;

	[Export]
	public Rect2 ButtonAnchors = new Rect2(0, 0, 1, 0);

	[Export]
	public bool ForceDisable;

	[Export]
	public bool AutoUpdateOnEnterTree;


	private List<TAssociatedParam>? LastUpdate;
	protected List<ButtonInstance> ButtonInstances = new();

	public BaseButtonMenu()
	{
	}

	public BaseButtonMenu(Control container) : this()
	{
		ContainerOverride = container;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (AutoUpdateOnEnterTree) Update();
	}

	public void Update()
	{
		if (LastUpdate is null) { GD.PushWarning("Nothing to update with. The last updated value is null or it was never set."); return; }
		Update(LastUpdate);
	}

	public void Update(List<TAssociatedParam> parameterList)
	{
		foreach (var item in ButtonInstances)
		{
			ButtonDelete(item);
		}

		ButtonInstances.Clear();

		foreach (var parameter in parameterList)
		{
			ButtonCreate(parameter);
		}

		_Update(parameterList);
		LastUpdate = parameterList;
	}

	protected virtual void _Update(List<TAssociatedParam> parameterList) { }

	private Control GetCurrentContainer()
		=> ContainerOverride ?? this;

	protected void ButtonCreate(TAssociatedParam parameter)
	{
		ButtonInstance button_instance = ButtonGetNew(parameter);
		_ButtonCreated(button_instance.NodeReference, button_instance.ParameterReference);
		GetCurrentContainer().AddChild(button_instance.NodeReference);
	}

	protected virtual ButtonInstance ButtonGetNew(TAssociatedParam param)
	{
		TButton button = new()
		{
			AnchorLeft = ButtonAnchors.Position.X,
			AnchorTop = ButtonAnchors.Position.Y,
			AnchorRight = ButtonAnchors.Size.X,
			AnchorBottom = ButtonAnchors.Size.Y,
			SizeFlagsHorizontal = ButtonHorizontalFlags,
			SizeFlagsVertical = ButtonVerticalFlags
		};
		if (button is Button butt && ForceDisable) butt.Disabled = true;

		ButtonInstance output = new(button, param, this);
		ButtonInstances.Add(output);
		return output;
	}

	protected virtual void ButtonDelete(ButtonInstance instance)
	{
		instance.NodeReference.QueueFree();
	}

	protected virtual void _ButtonPressed(TButton button, TAssociatedParam param)
	{
		ButtonPressed?.Invoke(button, param);
	}

	/// <summary>
	/// Used to modify the button after it is created.
	/// </summary>
	/// <param name="button">Button just created.</param>
	/// <param name="param">Object associated with it.</param>
	protected virtual void _ButtonCreated(TButton button, TAssociatedParam param)
	{
		
	}

	protected virtual void _ButtonHovered(TButton button, TAssociatedParam param, bool hovered)
	{
		TupleHovered = hovered ? (button, param) : null;
	}

	/* protected TButton? GetHoveredButton()
		=> ButtonInstances.Where(x => x.NodeReference.IsHovered()).FirstOrDefault()?.NodeReference ?? null; */

	protected ButtonInstance? GetInstanceFromButton(TButton button)
		=> ButtonInstances.FirstOrDefault(x => x.NodeReference == button);

	public void SetTooltip(TButton button, string text)
	{
		(GetInstanceFromButton(button) ?? throw new Exception()).Tooltip = text;
	}

	#region ITooltip
	string ITooltip.GetText()
	{
		var tuple = TupleHovered;
		//Make sure something is hovered.
		if (TupleHovered is (TButton, TAssociatedParam) notNull)
		{
			//Get the instance.
			ButtonInstance? instance = GetInstanceFromButton(notNull.Item1);

			//Use its tooltip if available.
			if (instance is not null)
				return instance.Tooltip;
			else
			{
				MsgLog.LogErrorMsg($"Failure in ITooltip.GetText() for type {GetType()}");
				return "";
			}
		}
		else return "";
	}

	bool ITooltip.ShouldShow()
		=> TupleHovered?.Item1?.IsHovered() ?? false;
	#endregion

	#region ButtonInstance
	protected class ButtonInstance
	{
		public TButton NodeReference;
		public TAssociatedParam ParameterReference;
		public BaseButtonMenu<TButton, TAssociatedParam> MenuReference;
		public string Tooltip = "";
		public ButtonInstance(TButton button, TAssociatedParam param, BaseButtonMenu<TButton, TAssociatedParam> menu)
		{
			NodeReference = button;
			ParameterReference = param;
			MenuReference = menu;
			NodeReference.Connect(Button.SignalName.Pressed, Callable.From(OnPressed));
			NodeReference.Connect(Button.SignalName.MouseEntered, Callable.From(OnMouseEntered));
			NodeReference.Connect(Button.SignalName.MouseExited, Callable.From(OnMouseExited));
			NodeReference.Connect(Button.SignalName.FocusEntered, Callable.From(OnFocusEntered));
			NodeReference.Connect(Button.SignalName.FocusExited, Callable.From(OnFocusExited));
		}

		public void OnPressed() => MenuReference._ButtonPressed(NodeReference, ParameterReference);

		public void OnMouseEntered() => MenuReference._ButtonHovered(NodeReference, ParameterReference, true);

		public void OnMouseExited() => MenuReference._ButtonHovered(NodeReference, ParameterReference, false);

		public void OnFocusEntered() => MenuReference._ButtonHovered(NodeReference, ParameterReference, true);

		public void OnFocusExited() => MenuReference._ButtonHovered(NodeReference, ParameterReference, false);
	}
	#endregion
}
