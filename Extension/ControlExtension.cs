using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike;

public static class ControlExtension
{
    public static void FitFontSizeToContainerSize(this Label @this, float ratio = 1.0f)
    {
        if (@this.LabelSettings is null){@this.LabelSettings = new();}

        @this.LabelSettings.FontSize = (int)(@this.Size.Y / 2 * ratio);
    }

    public static void Activate(this Control @this, bool activate)
    {
        if (activate)
        {
            int filter = (int)@this.GetMeta("og_filter");
            int focus = (int)@this.GetMeta("og_focus");

            if (filter == (int)Control.MouseFilterEnum.Ignore){filter = (int)Control.MouseFilterEnum.Pass;}
            if (focus == (int)Control.FocusModeEnum.None){filter = (int)Control.FocusModeEnum.All;}

            @this.MouseFilter = (Control.MouseFilterEnum)filter;
            @this.FocusMode = (Control.FocusModeEnum)focus;
            @this.Show();            
        }
        else
        {
            int og_filter = (int)@this.MouseFilter;
            int og_focus = (int)@this.FocusMode;
            @this.SetMeta("og_filter", og_filter);
            @this.SetMeta("og_focus", og_focus);

            @this.MouseFilter = Control.MouseFilterEnum.Ignore;
            @this.FocusMode = Control.FocusModeEnum.None;
            @this.Hide();
        }
    }
}
