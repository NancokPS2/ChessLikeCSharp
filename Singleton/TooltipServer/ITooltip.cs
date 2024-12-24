using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public interface ITooltip
{
    public string GetText();
    public Godot.Font GetFont();
    public int GetFontSize();

    public Godot.Vector2 GetRectSize() => new(120,80);
    public bool IsDirty() => true;
    bool IsShown()
    {
        if (this is Control control)
        {
            return control.GetGlobalRect().HasPoint(control.GetGlobalMousePosition());
        }
        else
        {
            return false;
        }
    }

    public CanvasItem GetCanvasItem() => Global.DrawNode;
}
