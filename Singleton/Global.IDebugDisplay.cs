using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public static partial class Global
{
    public class Debug : IDebugDisplay
    {
        string text = "";
        public string GetText()
        {
            if (RootNode is null || RootNode.GetViewport() is null)
            {
                return "";
            }
            else
            {
                text = string.Format(
                    "Hovered control: {0}",
                    RootNode.GetViewport().GuiGetHoveredControl()?.GetPath()
                );
                return text;
            }
        }

    }
}
