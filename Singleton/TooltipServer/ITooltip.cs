using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public interface ITooltip
{
    public string GetText();
    public Godot.Font GetFont() => Global.ManagerFont.ResourceGet("Regular");
    public int GetFontSize() => 16;
	public Godot.Vector2 GetOffset() => Godot.Vector2.Zero;

    public Godot.Vector2 GetRectSize() => new(140, 80);
    public bool IsDirty() => true;
    public bool ShouldShow()
    {
        if (this is Control control)
        {
			return control.IsHovered();
        }
        else
        {
            return false;
        }
    }

    public CanvasItem GetCanvasItem() => UIManager.GetLayerDrawTarget(UIManager.ELayer.TOOLTIP);
}
