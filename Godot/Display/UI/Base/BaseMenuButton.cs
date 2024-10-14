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
    Rect2 ButtonAnchors = new Rect2(0,0,1,0);

    private List<TAssociatedParam>? _last_update;

    private Dictionary<TButton, List<(Action, Action)>> _button_to_lambda = new();

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
        foreach (var item in used_container.GetChildren<TButton>())
        {
            used_container.FreeChildren();           
        }

        _button_to_lambda.Clear();

        foreach (var parameter in parameter_list)
        {
            TButton button = ButtonCreate(parameter);
            ButtonUpdateConnection(button, parameter);
            ButtonCreated?.Invoke(button, parameter);
            used_container.AddChild(button);
        }

        _last_update = parameter_list;
    }

    protected virtual TButton ButtonCreate(TAssociatedParam param)
    {
        TButton button = new(){
            AnchorLeft = ButtonAnchors.Position.X, 
            AnchorTop = ButtonAnchors.Position.Y, 
            AnchorRight = ButtonAnchors.Size.X, 
            AnchorBottom = ButtonAnchors.Size.Y, 
            SizeFlagsHorizontal = ButtonHorizontalFlags,
            SizeFlagsVertical = ButtonVerticalFlags
        };
        return button;
    }

    //Posible memory leak.
    protected virtual void ButtonUpdateConnection(TButton button, TAssociatedParam param)
    {
        Action on_pressed = () => OnButtonPressed(button, param);
        Action on_mouse_entered = () => OnButtonHovered(button, param, true);
        Action on_focus_entered = () => OnButtonHovered(button, param, true);
        Action on_mouse_exited = () => OnButtonHovered(button, param, false);
        Action on_focus_exited = () => OnButtonHovered(button, param, false);

        button.Pressed += on_pressed;
        
        button.MouseEntered += on_mouse_entered;
        button.FocusEntered += on_focus_entered;

        button.MouseExited += on_mouse_exited;
        button.FocusExited += on_focus_exited;

    }

    protected virtual void OnButtonPressed(TButton button, TAssociatedParam param)
    {
        ButtonPressed?.Invoke(button, param);
    }
    protected abstract void OnButtonCreated(TButton button, TAssociatedParam param);


    protected abstract void OnButtonHovered(TButton button, TAssociatedParam param, bool hovered);
}
