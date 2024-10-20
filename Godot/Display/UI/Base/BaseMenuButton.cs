using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public abstract partial class BaseButtonMenu<TButton, TAssociatedParam> : Control
    where TButton : BaseButton, new()
{
    public delegate void ButtonInteraction(TButton button, TAssociatedParam param);
    public event ButtonInteraction? ButtonCreated;
    public event ButtonInteraction? ButtonPressed;

    protected (TButton, TAssociatedParam)? TupleSelected;

    [Export]
    public Control? Container;
    [Export]
    public SizeFlags ButtonHorizontalFlags = SizeFlags.ExpandFill;
    [Export]
    public SizeFlags ButtonVerticalFlags = SizeFlags.ShrinkBegin;
    [Export]
    public Rect2 ButtonAnchors = new Rect2(0,0,1,0);

    protected (TButton, TAssociatedParam)? TupleHovered;

    private List<TAssociatedParam>? _last_update;
    private List<ButtonInstance> ButtonInstances = new();

    public BaseButtonMenu()
    {
        ButtonCreated += OnButtonCreated;
    }
    
    public BaseButtonMenu(Control container) : this()
    {
        Container = container;
    }

    protected void Update()
    {
        if (_last_update is null) {GD.PushWarning("Nothing to update with. The last updated value is null or it was never set."); return;}
        Update(_last_update);
    }

    protected void Update(List<TAssociatedParam> parameter_list)
    {
        Control used_container = Container ?? this;
        foreach (var item in ButtonInstances)
        {
            ButtonDelete(item);
        }

        ButtonInstances.Clear();

        foreach (var parameter in parameter_list)
        {
            ButtonInstance button_instance = ButtonCreate(parameter);
            ButtonCreated?.Invoke(button_instance.NodeReference, button_instance.ParameterReference);
            used_container.AddChild(button_instance.NodeReference);
        }

        _last_update = parameter_list;
    }

    protected virtual ButtonInstance ButtonCreate(TAssociatedParam param)
    {
        TButton button = new(){
            AnchorLeft = ButtonAnchors.Position.X, 
            AnchorTop = ButtonAnchors.Position.Y, 
            AnchorRight = ButtonAnchors.Size.X, 
            AnchorBottom = ButtonAnchors.Size.Y, 
            SizeFlagsHorizontal = ButtonHorizontalFlags,
            SizeFlagsVertical = ButtonVerticalFlags
        };
        ButtonInstance output = new(button, param, this);
        ButtonInstances.Add(output);
        return output;
    }

    public virtual void ButtonDelete(ButtonInstance instance)
    {
        instance.NodeReference.QueueFree();
    }

    protected virtual void OnButtonPressed(TButton button, TAssociatedParam param)
    {
        ButtonPressed?.Invoke(button, param);
    }
    protected abstract void OnButtonCreated(TButton button, TAssociatedParam param);

    protected virtual void OnButtonHovered(TButton button, TAssociatedParam param, bool hovered)
    {
        TupleHovered = hovered ? (button, param) : null;
    }

    public class ButtonInstance
    {
        public TButton NodeReference;
        public TAssociatedParam ParameterReference;
        public BaseButtonMenu<TButton, TAssociatedParam> MenuReference;
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

        public void OnPressed() => MenuReference.OnButtonPressed(NodeReference, ParameterReference);

        public void OnMouseEntered() => MenuReference.OnButtonHovered(NodeReference, ParameterReference, true);

        public void OnMouseExited() => MenuReference.OnButtonHovered(NodeReference, ParameterReference, false);

        public void OnFocusEntered() => MenuReference.OnButtonHovered(NodeReference, ParameterReference, true);

        public void OnFocusExited() => MenuReference.OnButtonHovered(NodeReference, ParameterReference, false);
    }
}
