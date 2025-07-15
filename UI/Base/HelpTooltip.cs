using Godot;
using System;

public partial class HelpTooltip : Panel, ITooltip
{
    public const string THEME_OBJECT = "TooltipLabel";

	[Export(PropertyHint.MultilineText)]
	public string Text = "";
    public Godot.Font GetFont() => Global.ManagerFont.ResourceGet("Regular");//GetThemeFont(THEME_OBJECT, "font");

    public int GetFontSize() => 16;


    public string GetText() => Text;

}
