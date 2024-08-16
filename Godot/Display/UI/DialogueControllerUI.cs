using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Resolvers;
using ChessLike.Dialogue;

namespace Godot;

public class DialogueController
{
    static readonly Godot.Font FONT_DEFAULT = GD.Load<Godot.Font>("assets/Font.tres");
    public ChessLike.Dialogue.Controller controller = new();
    
    [Export]
    public Node? target;

    public DialogueController(Controller controller, Node target)
    {
        this.controller = controller;
        this.target = target;
    }

    public void AdvanceDelta(float delta)
    {
    }

    public void UpdateNode()
    {
        string text = controller.GetText();
        if (target == null){ GD.PushError("No node connected."); return;}
        
        if(target is Label label)
            {
                label.Text = new string(text);
                label.AddThemeFontOverride("font", FONT_DEFAULT);
            }
        else if(target is Label3D label3d)
            {
                label3d.Text = new string(text);
                label3d.Font = FONT_DEFAULT;
            }
        else if(target is RichTextLabel rich)
            {
                throw new NotImplementedException();
            }
    }
}
