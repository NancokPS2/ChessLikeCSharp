using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

public partial class TooltipParameters : Resource
{
    public bool Visible;

    [Export]
    public string DisplayName = "Unnamed ActionEvent";
    [Export]
    private string Description = "No Description";

    public TooltipParameters(string name, string description)
    {
        DisplayName = name;
        Description = description;
    }

    //public string Description { get => GetDescription(); set => SetDescription(value); }

    public string GetDescription()
    {
        return Description;
    }

    public void SetDescription(string desc)
    {
        Description = desc;
    }
}
