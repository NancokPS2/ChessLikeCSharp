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

    public static readonly NodeRequirement GENERAL_PANEL = new(
        "MobGeneralUI",
        typeof(MobGeneralUI)
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
        ACTION_CONTAINER, TURN_CONTAINER, GENERAL_PANEL, CONFIRM_BUTTON, CANCEL_BUTTON

    };

    public MobGeneralUI CompUnitStatus;
    public CombatTurnUI CompDelayList;
    public ActionUI CompActionMenu;


    public MobCombatUI()
    {
        Layer = -1;
    }

    public override void _Ready()
    {
        base._Ready();
        this.AddSceneWithDeclarations(SCENE_PATH, NodesRequired);

        CompUnitStatus = new MobGeneralUI().GetInstantiatedScene<MobGeneralUI>();
        CompDelayList = new CombatTurnUI().GetInstantiatedScene<CombatTurnUI>();
        CompActionMenu = new(this.GetNodeFromRequirement<VBoxContainer>(ACTION_CONTAINER));

        var confirm_btn = this.GetNodeFromRequirement<Button>(CONFIRM_BUTTON);
        var cancel_btn = this.GetNodeFromRequirement<TextureButton>(CANCEL_BUTTON);

        confirm_btn.Pressed += () => ConfirmPressed.Invoke();
        cancel_btn.Pressed += () => CancelPressed.Invoke();

        //ShowConfirmationButton(false);
    }

    #pragma warning restore CS8602 // Dereference of a possibly null reference.




}
