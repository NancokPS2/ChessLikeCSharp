using Godot;
using System;

[GlobalClass]
public partial class CombatStateOverlay : Control
{
    public ECombatState StateCurrent;

    [Export]
    public Panel? NodePanel;

    [Export]
    protected Godot.Collections.Dictionary<ECombatState, StyleBox> Styles = new();

    [Export]
    protected StyleBox? StylePlaceholder;

    public CombatStateOverlay()
    {
        EventBus.CombatStateChanged += OnBattleStateChanged;
    }

    public override void _Ready()
    {
        base._Ready();
        if (NodePanel is null) throw new Exception();
        if (StylePlaceholder is null) throw new Exception();
    }


    protected void UpdateStyle(ECombatState state)
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
                case ECombatState.PAUSED:
                    flat.BorderColor = Colors.Gray;
                    break;

                case ECombatState.ACTION_INPUT:
                    flat.BorderColor = Colors.LightCyan;
                    break;

                case ECombatState.TARGETING:
                    flat.BorderColor = Colors.Yellow;
                    break;

                case ECombatState.ACTION_RUNNING:
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
    private void OnBattleStateChanged(ECombatState state)
    {
        UpdateStyle(state);
    }
    #endregion
}
