using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class SaveDialog : RefCounted
{
    private FileDialog NodeDialog = new(){FileMode = FileDialog.FileModeEnum.SaveFile};

    private Resource? ResourceToSave;

    private Node Parent;

    public FileDialog.AccessEnum AccessMode = FileDialog.AccessEnum.Userdata;

    public string WindownName = "Save resource.";

    public SaveDialog(Node parent, Resource? resource = null)
    {
        NodeDialog.FileSelected += OnFileSelected;
        Parent = parent;

        if (resource is not null)
        {
            Use(resource);
        }
    }

    public void Use(Resource resource)
    {
        ResourceToSave = resource;
        
        NodeDialog.ModeOverridesTitle = false;
        NodeDialog.Access = AccessMode;
        NodeDialog.Title = WindownName;

        Parent.AddChild(NodeDialog);
        NodeDialog.PopupCentered(new(400,400));
    }

    private void OnFileSelected(string file)
    {
        if (ResourceToSave is null)
        {
            throw new Exception();
        }

        ResourceSaver.Save(ResourceToSave, file, ResourceSaver.SaverFlags.ChangePath);
        NodeDialog.RemoveSelf();
        ResourceToSave = null;
    }
}
