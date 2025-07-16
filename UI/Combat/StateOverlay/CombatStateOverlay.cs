using Godot;
using System;

[GlobalClass]
public partial class CombatStateOverlay : Control
{
    public EBattleState StateCurrent;

    [Export]
    public Panel? NodePanel;

    [Export]
    protected Godot.Collections.Dictionary<EBattleState, StyleBox> Styles = new();

    [Export]
    protected StyleBox? StylePlaceholder;

    public CombatStateOverlay()
    {
        EventBus.BattleStateChanged += OnBattleStateChanged;
    }

    public override void _Ready()
    {
        base._Ready();
        if (NodePanel is null) throw new Exception();
        if (StylePlaceholder is null) throw new Exception();
    }


    protected void UpdateStyle(EBattleState state)
    {
        StyleBox style;
        if (Styles.ContainsKey(state))
        {
            style = Styles[state];
            NodePanel?.AddThemeStyleboxOverride("panel", style);
            return;
        }

        style = StylePlaceholder ?? throw new Exception("Missing placeholder!");

        if (style is StyleBoxFlat flat)
        {
            switch (state)
            {
                case EBattleState.PAUSED:
                    flat.BorderColor = Colors.Gray;
                    break;

                case EBattleState.ACTION_INPUT:
                    flat.BorderColor = Colors.LightCyan;
                    break;

                case EBattleState.TARGETING:
                    flat.BorderColor = Colors.Yellow;
                    break;

                case EBattleState.ACTION_RUNNING:
                    flat.BorderColor = Colors.DarkRed;
                    break;

                default:
                    flat.BorderColor = Colors.White;
                    break;
            }
        }

        NodePanel?.AddThemeStyleboxOverride("panel", style);
    }

    #region Event Handling
    private void OnBattleStateChanged(EBattleState state)
    {
        UpdateStyle(state);
    }
    #endregion
}
