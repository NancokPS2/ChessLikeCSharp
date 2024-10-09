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


    [Export]
    public Control? Container;
    [Export]
    public SizeFlags ButtonHorizontalFlags = SizeFlags.ExpandFill;
    [Export]
    public SizeFlags ButtonVerticalFlags = SizeFlags.ShrinkBegin;
    [Export]
    Rect2 ButtonAnchors = new Rect2(0,0,1,0);

    private List<TAssociatedParam>? _last_update;

    public BaseButtonMenu()
    {
        ButtonCreated += OnButtonCreated;
    }
    
    public BaseButtonMenu(Control container) : base()
    {
        Container = container;
    }

    public void Update()
    {
        if (_last_update is null) {GD.PushWarning("Nothing to update with. The last updated value is null or it was never set."); return;}
        Update(_last_update);
    }

    public void Update(List<TAssociatedParam> parameter_list)
    {
        Control used_container = Container ?? this;
        used_container.FreeChildren();

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

    protected virtual void ButtonUpdateConnection(TButton button, TAssociatedParam param)
    {
        button.Pressed += () => OnButtonPressed(button, param);
        
        button.MouseEntered += () => OnButtonHovered(button, param, true);
        button.FocusEntered += () => OnButtonHovered(button, param, true);

        button.MouseExited += () => OnButtonHovered(button, param, false);
        button.FocusExited += () => OnButtonHovered(button, param, false);
    }

    protected abstract void OnButtonCreated(TButton button, TAssociatedParam param);

    protected abstract void OnButtonPressed(TButton button, TAssociatedParam param);

    protected abstract void OnButtonHovered(TButton button, TAssociatedParam param, bool hovered);
}
