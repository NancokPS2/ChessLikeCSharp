using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class SaveDialog : RefCounted
{
    private FileDialog NodeDialog = new(){FileMode = FileDialog.FileModeEnum.SaveFile};

    private List<Resource> ResourcesToSave = new();

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

    public void Use(Resource resource) => Use( new List<Resource>(){resource});

    public void Use(List<Resource> resource)
    {
        ResourcesToSave = resource;
        
        NodeDialog.ModeOverridesTitle = false;
        NodeDialog.Access = AccessMode;
        NodeDialog.Title = WindownName;

        Parent.AddChild(NodeDialog);
        NodeDialog.PopupCentered(new(400,400));
    }

    private void OnFileSelected(string file)
    {
        if (ResourcesToSave is null)
        {
            throw new Exception();
        }

        int file_count = 0;
        foreach (var item in ResourcesToSave)
        {
            Error error;
            //If there's only 1 file, save it as is.
            if (ResourcesToSave.Count == 1)
            {
                error = ResourceSaver.Save(item, file, ResourceSaver.SaverFlags.ChangePath);
            }else
            {
                string dir = file.GetBaseDir() + "/";
                string extension = "." + file.GetExtension();
                string file_name = file.GetFile().TrimSuffix(extension);
                string new_file = dir + file_name + file_count.ToString() + extension;
                
                error = ResourceSaver.Save(item, new_file, ResourceSaver.SaverFlags.ChangePath);
            }

            if (error != Error.Ok){throw new Exception(error.ToString());}
            file_count ++;
            
        }
        NodeDialog.RemoveSelf();
        ResourcesToSave = new();
    }
}
