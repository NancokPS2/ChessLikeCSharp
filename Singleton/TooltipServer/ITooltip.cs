using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public interface ITooltip
{
    public string GetText();
    public Godot.Font GetFont() => Global.Readonly.FONT_SMALL;
    public int GetFontSize() => 16;

    public Godot.Vector2 GetRectSize() => new(140,80);
    public bool IsDirty() => true;
    bool IsShown()
    {
        if (this is Control control && GodotObject.IsInstanceValid(control) && control.IsVisibleInTree())
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
