using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;

namespace Godot.Display;

public partial class OptionPopupUI<TResultEnum> : CanvasLayer where TResultEnum : notnull
{
    public delegate void OptionSelect(TResultEnum option);
    public event OptionSelect? OptionSelected;

    private SceneTree TreeReference;

    public bool KillOnSelection = true; 

    public string Text = "";

    public Theme? Theme;

    public OptionPopupUI(SceneTree tree_ref)
    {
        TreeReference = tree_ref;
    }

    public OptionPopupUI(SceneTree tree_ref, string text)
    {
        TreeReference = tree_ref;
        Text = text;
    }

    public void AddToScene()
    {
        if (IsInsideTree())
        {
            Reparent(TreeReference.Root);
        }
        else
        {
            TreeReference.Root.AddChild(this);
            if (!IsInsideTree())
            {
                throw new Exception("How?");
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();

        this.FreeChildren();

        Layer = 16;

        float size = TreeReference.Root.GetViewport().GetVisibleRect().Size.Y / 4;

        Panel container = new(){
            AnchorBottom = size, AnchorLeft = -size, AnchorRight = size, AnchorTop = -size,
            CustomMinimumSize = new(20,20)
            };
        container.SetAnchorsPreset(Control.LayoutPreset.Center);
        if (Theme is not null){container.Theme = Theme;}
        AddChild(container);

        HBoxContainer button_container = new();
        button_container.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.RightWide);
        container.AddChild(button_container);

        Label label = new(){Text = Text};
        container.AddChild(label);
        label.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopWide);

        foreach (TResultEnum item in Enum.GetValues(typeof(TResultEnum)))
        {
            Button button = new(){Text = item.ToString()};
            button_container.AddChild(button);
            button.Pressed += () => OnOptionPressed(item);
        }
    }

    public void OnOptionPressed(TResultEnum result)
    {
        OptionSelected?.Invoke(result);
        
        if (KillOnSelection) 
        {
            this.RemoveSelf();
            this.QueueFree();
        }
    }
}
