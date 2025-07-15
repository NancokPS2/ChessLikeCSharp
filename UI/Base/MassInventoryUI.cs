using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Storage;
using Godot;

[GlobalClass]
public partial class MassInventoryUI : BaseButtonMenu<Button, Item>
{
    public MassInventory MassInventorySelected;

    public void Update(MassInventory massInventory)
    {
        MassInventorySelected = massInventory;
        Update(MassInventorySelected.GetItems());
    }

    protected override void OnButtonCreated(Button button, Item param)
    {
        base.OnButtonCreated(button, param);
        button.Text = param.Name;
        button.TooltipText = param.ToString();
    }
}
