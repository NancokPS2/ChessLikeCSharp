using Godot;
using System;

public partial class HelpTooltip : Panel, ITooltip
{
	[Export(PropertyHint.MultilineText)]
	public string Text = "";
    public Godot.Font GetFont() => Global.Readonly.FONT_SMALL;

    public int GetFontSize() => 16;


    public string GetText() => Text;

}
