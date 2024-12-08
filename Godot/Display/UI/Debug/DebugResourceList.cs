using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;

public partial class DebugResourceList : BaseButtonMenu<Button, IDescription> , ISceneDependency
{
    const int ID_JOB = 0;
    const int ID_ITEM = 1;
    const int ID_ABILITY = 2;
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Debug/DebugResourceList.tscn";

    [Export]
    public RichTextLabel? NodeDescription;
    [Export]
    public MenuButton? NodeResourceTypeMenu {
        set {
            if (_noderestypemenu is not null)
            {
                _noderestypemenu.GetPopup().IdPressed -= OnIdPressed;
            }
            _noderestypemenu = value; 
            if (_noderestypemenu is not null)
            {
                _noderestypemenu.GetPopup().IdPressed += OnIdPressed;
            }
            }
        get => _noderestypemenu;
    }


    private MenuButton? _noderestypemenu;

    private Dictionary<long, List<IDescription>> Entries = new();

    public void RefreshEntries()
    {
        Entries.Clear();
        NodeResourceTypeMenu?.GetPopup().Clear(true);

        //Jobs
        List<IDescription> jobs = new();
        jobs.AddRange(Global.ManagerJob.CreatePrototypes());
        Entries.Add(ID_JOB, jobs);
        NodeResourceTypeMenu?.GetPopup().AddItem("Jobs", ID_JOB);

        //Items
        List<IDescription> items = new();
        items.AddRange(Global.ManagerItem.CreatePrototypes());
        Entries.Add(ID_ITEM, items);
        NodeResourceTypeMenu?.GetPopup().AddItem("Items", ID_ITEM);

        //Abilities
        List<IDescription> abilities = new();
        abilities.AddRange(Global.ManagerAbility.CreatePrototypes());
        Entries.Add(ID_ABILITY, abilities);
        NodeResourceTypeMenu?.GetPopup().AddItem("Abilities", ID_ABILITY);
    }

    protected override void OnButtonCreated(Button button, IDescription param)
    {
        button.Text = param.GetDescriptiveName();
    }

    protected override void OnButtonPressed(Button button, IDescription param)
    {
        base.OnButtonPressed(button, param);
        string resource_text = "";
        //If it is a resource
        if (param is Resource res)
        {
            resource_text += "Path: " + res.ResourcePath;
        }

        NodeDescription.Text = param.GetDescription(true) + 
        "\n" + resource_text +
        "\n" + "Type: " + param.GetType().ToString();
    }

    private void OnIdPressed(long id)
    {
        Update(Entries[id]);
    }
}