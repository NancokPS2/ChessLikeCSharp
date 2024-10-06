using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared;

public interface IListVisual
{
    public string Name {get; set;}
    public string Description {get; set;}
    public string DescriptionRich {get => Description;}
    public Texture2D? Icon => null;
}
