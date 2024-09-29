using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.Core.Sources;

namespace Godot;

public partial class ItemList<TItemContained> where TItemContained : class
{
    public delegate void ContainedPress(TItemContained contained);
    public event ContainedPress? ContainerPressed;
    private List<MenuItem> MenuItems = new();
    public uint ItemsPerScreen = 6;

    public Control ControlReference;

    public ItemList(Control reference)
    {
        ControlReference = reference;
    }
    private float GetItemHeight()
    {
        var parent = ControlReference.GetParent();
        if (parent is ScrollContainer scroll)
        {
            return scroll.Size.Y / ItemsPerScreen;
        }
        else
        {
            return ControlReference.Size.Y / ItemsPerScreen;
        }
    }

    public void RemoveItem(TItemContained contained)
    {
        MenuItem to_remove = MenuItems.First(x => x.Contained is TItemContained && x.Contained == contained);
        RemoveItem(to_remove);
    }

    private void RemoveItem(MenuItem menu_item)
    {
        MenuItems.Remove(menu_item);
        UpdateMenuItems();
    }

    public void AddItem(MenuItem item)
    {

        if (item is not null && MenuItems.Any(x => x.Contained == item.Contained))
        {
            throw new Exception("A MenuItem with this same contained object is already inside.");
        }

        MenuItems.Add(item);
        UpdateMenuItems();
    }

    public void ClearItems()
    {
        MenuItems.Clear();
        UpdateMenuItems();
    }

    public void UpdateMenuItems()
    {
        ControlReference.FreeChildren();
        ControlReference.Size = ControlReference.GetParent<Control>().Size;

        foreach (var item in MenuItems)
        {
            CreateItem(item);
        }
        if (MenuItems.Count < ItemsPerScreen)
        {
            CreateItem(null);
        }
    }

    private void CreateItem(MenuItem? item)
    {
        Panel panel = new()
        {
            AnchorRight = 1.0f,
            CustomMinimumSize = new(0, GetItemHeight()),
        };
        Label label = new()
        {
            AnchorRight = 1,
            AnchorBottom = 1,
            OffsetLeft = GetItemHeight() + 4,
            LabelSettings = new(),
        };

        TextureRect textureRect = new()
        {
            AnchorBottom = 1,
            CustomMinimumSize = new(GetItemHeight(), GetItemHeight()),
        };
        
        if (item is not null)
        {
            if (item.PanelBackground is StyleBox styleBox){panel.AddThemeStyleboxOverride("Panel", styleBox);}
            panel.Modulate = item.Disabled ? new Color(0.5f,0.5f,0.5f) : new Color(1,1,1);
            ControlReference.AddChild(panel);
        
            label.Text = item.Text;
            //TODO: Find a good algorithm to replace this.
            label.LabelSettings.FontSize = 16;
            panel.AddChild(label);

            textureRect.Texture = item.Texture;
            panel.AddChild(textureRect);

            panel.GuiInput += (InputEvent input) => OnItemInput( (label, textureRect, panel), item, input);
            panel.MouseEntered += () => OnNodeHovered(panel, true);
            panel.MouseExited += () => OnNodeHovered(panel, false);
        }
        else
        {
            panel.Modulate = new Color(0.5f,0.5f,0.5f);
            ControlReference.AddChild(panel);
            panel.AddChild(label);
            panel.AddChild(textureRect);
        }



    }

    public void OnNodeHovered(Control node, bool hovered)
    {
        node.Modulate = hovered ? new(0.5f,0.5f,0.5f) : new(1,1,1);
    }

    public void OnItemInput((Label,TextureRect,Panel) node_refs, MenuItem menu_item, InputEvent action)
    {
        if (menu_item.Disabled){return;}

        if (action.IsActionPressed("accept") && menu_item.Contained is TItemContained)
        {
            ContainerPressed?.Invoke(menu_item.Contained);
        }
    }

    public partial class MenuItem
    {
        public StyleBox PanelBackground = new();
        public TItemContained? Contained;
        public Texture2D? Texture;
        public string Text = "";
        public bool Disabled = false;

        public MenuItem ChainSetContained(TItemContained contained)
        {
            Contained = contained;
            return this;
        }
        public MenuItem ChainSetTexture(Texture2D texture)
        {
            Texture = texture;
            return this;
        }
        public MenuItem ChainSetBackground(StyleBox style)
        {
            PanelBackground = style;
            return this;
        }
        public MenuItem ChainSetText(string text)
        {
            Text = text;
            return this;
        }
    }
}
