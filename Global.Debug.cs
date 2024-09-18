using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Global
{
    private static Dictionary<object, string> DebugText = new();
    
    private static SystemFont _font = new();
    public static void WriteDebug()
    {
        string text = "";
        foreach (var item in DebugText)
        {
            text += item.Value;
        }
        DrawNode.DrawMultilineString(_font, new(0,12), text);
    }

    public static void SetText(object source, string text)
    {
        DebugText[source] = text;
    }
}
