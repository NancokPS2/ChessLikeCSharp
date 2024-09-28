using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot.Display;

public partial class MobInventoryUIUNUSED : CanvasLayer
{
    const string SLOT_SCENE_PATH = "res://assets/PackedScene/Inventory/ItemSlot.tscn";
    public static readonly List<NodeRequirement> SLOT_REQUIREMENTS = new()
    {
        new("SLOT_CONTAINER", typeof(Container)),
        new("TEXTURE", typeof(TextureRect)),
        new("LABEL", typeof(Label)),
    };

    const string INVENTORY_SCENE_PATH = "res://assets/PackedScene/Inventory/MobInventory.tscn";
    public static readonly List<NodeRequirement> INVENTORY_REQUIREMENTS = new()
    {
        new("EQUIP_LIST", typeof(Container))
    };


    public uint ItemsPerScreen = 6;

    public void UpdateEquipment(Mob mob)
    {
        Container container = this.GetNodeFromRequirement<Container>(INVENTORY_REQUIREMENTS[0]);
        container.FreeChildren();

        float height = container.Size.Y / ItemsPerScreen;

        foreach (var slot in mob.Inventory.GetSlots())
        {
            Container item_slot = this.AddSceneWithDeclarations<Container>(SLOT_SCENE_PATH, SLOT_REQUIREMENTS);
            item_slot.CustomMinimumSize = new(0, height);

            TextureRect rect = item_slot.GetNodeFromRequirement<TextureRect>(SLOT_REQUIREMENTS[1]);
            rect.CustomMinimumSize = new(height, height);

            Label label = item_slot.GetNodeFromRequirement<Label>(SLOT_REQUIREMENTS[2]);
            label.Text = slot.Item.Name;
        }
    }

/*     public Control CreateItemDisplay(Item item, int height = 86)
    {
        TextureButton output = new();

        HSplitContainer container = new();
        container.CustomMinimumSize = new(0,height);

        TextureRect tex = new();
        tex.CustomMinimumSize = new(height, height);

        Label label = new();
        label.Text = item.Name;
        
        container.Ready += () => container.AddChild(tex);
        container.Ready += () => container.AddChild(label);
        container.Ready += () => container.AddChild(label);
        return container;
    } */
}
