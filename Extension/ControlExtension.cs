using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike;

public static class ControlExtension
{
	private const string META_KEY_OG_FILTER = "extension_og_filter";
	private const string META_KEY_OG_FOCUS = "extension_og_focus";

	public static void FitFontSizeToContainerSize(this Label @this, float ratio = 1.0f)
	{
		if (@this.LabelSettings is null) { @this.LabelSettings = new(); }

		@this.LabelSettings.FontSize = (int)(@this.Size.Y / 2 * ratio);
	}

	public static void Activate(this Control @this, bool activate)
	{
		if (activate)
		{
			int filter = (int)@this.GetMeta(META_KEY_OG_FILTER);
			int focus = (int)@this.GetMeta(META_KEY_OG_FOCUS);

			if (filter == (int)Control.MouseFilterEnum.Ignore) { filter = (int)Control.MouseFilterEnum.Pass; }
			if (focus == (int)Control.FocusModeEnum.None) { focus = (int)Control.FocusModeEnum.All; }

			@this.MouseFilter = (Control.MouseFilterEnum)filter;
			@this.FocusMode = (Control.FocusModeEnum)focus;
			@this.Show();
		}
		else
		{
			int og_filter = (int)@this.MouseFilter;
			int og_focus = (int)@this.FocusMode;
			@this.SetMeta(META_KEY_OG_FILTER, og_filter);
			@this.SetMeta(META_KEY_OG_FOCUS, og_focus);

			@this.MouseFilter = Control.MouseFilterEnum.Ignore;
			@this.FocusMode = Control.FocusModeEnum.None;
			@this.Hide();
		}
	}

	public static bool IsHovered(this Control control)
	{
		if (!GodotObject.IsInstanceValid(control)) return false;
		if (!control.IsVisibleInTree()) return false;
		return control.GetGlobalRect().HasPoint(control.GetGlobalMousePosition());
	}
}
