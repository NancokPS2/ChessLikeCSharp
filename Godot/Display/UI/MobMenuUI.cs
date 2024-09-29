using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot;

[GlobalClass]
public partial class MobMenuUI : Control
{
    public static readonly List<NodeRequirement> INVENTORY_REQUIREMENTS = new()
    {
        new("EQUIP_LIST", typeof(Container)),
    };
    private bool shown = true;

    public ItemList<Item> EquipList;

    public MobMenuUI()
    {
        EquipList = new(this);
        AnchorBottom = 1; 
        AnchorRight = 1; 
        Name = "MobMenu";
    }

    public override void _Ready()
    {
        base._Ready();
        Control equip_list_container = this.GetNodeFromRequirement<Control>(INVENTORY_REQUIREMENTS[0]);
        EquipList.ControlReference = equip_list_container;

        DisplayDummy();
    }

    private void DisplayDummy()
    {
        Mob mob = Mob.CreatePrototype(EMobPrototype.HUMAN);
        mob.Inventory.AddItem(new Coin());
        UpdateEquipment(mob);
    }

    public void UpdateEquipment(Mob mob)
    {
        EquipList.ClearItems();
        GD.Print(mob.Inventory.GetSlots().ToArray());
        foreach (var slot in mob.Inventory.GetSlots())
        {
            if (slot.Item is null){continue;}

            EquipList.AddItem(
                new ItemList<Item>.MenuItem()
                    .ChainSetContained(slot.Item)
                    .ChainSetText(slot.Item.Name)
                );
        }
    }

}
