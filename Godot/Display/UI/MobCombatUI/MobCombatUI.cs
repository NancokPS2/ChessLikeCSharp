using ChessLike.Entity;
using ChessLike.Turn;
using Action = ChessLike.Entity.Action;

namespace Godot.Display;

public partial class MobCombatUI : CanvasLayer
{
    public const string SCENE_PATH = "res://assets/PackedScene/CombatUI/MobCombatUI.tscn";

    public delegate void ButtonPress();
    public event ButtonPress ConfirmPressed;
    public event ButtonPress CancelPressed;

    public static readonly NodeRequirement ACTION_CONTAINER = new(
        "ACTION_CONTAINER",
        typeof(VBoxContainer)
        );

    public static readonly NodeRequirement TURN_CONTAINER = new(
        "TURN_CONTAINER",
        typeof(HBoxContainer)
        );

    public static readonly NodeRequirement COMPACT_PANEL = new(
        "COMPACT_PANEL",
        typeof(Control)
        );        

    public static readonly NodeRequirement EQUIPMENT_CONTAINER = new(
        "EQUIP_LIST", typeof(VBoxContainer)
        );

    public static readonly NodeRequirement CONFIRM_BUTTON = new(
        "EXECUTE_BUTTON", typeof(Button)
        );
    public static readonly NodeRequirement CANCEL_BUTTON = new(
        "CANCEL_BUTTON", typeof(TextureButton)
        );


    public static readonly List<NodeRequirement> NodesRequired = new()
    {
        //Container
        ACTION_CONTAINER, TURN_CONTAINER, COMPACT_PANEL, EQUIPMENT_CONTAINER, CONFIRM_BUTTON, CANCEL_BUTTON

    };

    public UnitStatsUI CompUnitStatus;
    public DelayListUI CompDelayList;
    public ActionUI CompActionMenu;
    public EquipmentUI CompEquipMenu;


    public MobCombatUI()
    {
        Layer = -1;
    }

    public override void _Ready()
    {
        base._Ready();
        this.AddSceneWithDeclarations(SCENE_PATH, NodesRequired);

        CompUnitStatus = new(this.GetNodeFromRequirement<Control>(COMPACT_PANEL));
        CompDelayList = new(this.GetNodeFromRequirement<HBoxContainer>(TURN_CONTAINER));
        CompActionMenu = new(this.GetNodeFromRequirement<VBoxContainer>(ACTION_CONTAINER));
        CompEquipMenu = new(this.GetNodeFromRequirement<VBoxContainer>(EQUIPMENT_CONTAINER));

        var confirm_btn = this.GetNodeFromRequirement<Button>(CONFIRM_BUTTON);
        var cancel_btn = this.GetNodeFromRequirement<TextureButton>(CANCEL_BUTTON);

        confirm_btn.Pressed += () => ConfirmPressed.Invoke();
        cancel_btn.Pressed += () => CancelPressed.Invoke();

        ShowConfirmationButton(false);
    }

    public void OnCancel()
    {
        
    }


    public void ShowConfirmationButton(bool activate)
    {
        var accept = this.GetNodeFromRequirement<Button>(CONFIRM_BUTTON);
        var cancel = this.GetNodeFromRequirement<TextureButton>(CANCEL_BUTTON);

        accept.Activate(activate);
        cancel.Activate(activate);
    }

    #pragma warning restore CS8602 // Dereference of a possibly null reference.




}
