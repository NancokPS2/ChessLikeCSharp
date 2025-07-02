using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using Godot;

[GlobalClass]
public partial class StatusEffectIcon : FloatingIcon3D
{
    [Export]
    double SwapInterval = 1.2;

    public List<ActionEvent> StatusEffects = new();

    int currentIndex;

    double timeSinceLast;

    public void SetStatusEffects(List<ActionEvent> statusEffects)
        => StatusEffects = statusEffects;

    [Obsolete("Missing a placeholder.")]
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (timeSinceLast > SwapInterval)
        {
            currentIndex++;
        }
        if (currentIndex >= StatusEffects.Count)
        {
            currentIndex = 0;

            if (StatusEffects.Count == 0) return;
        }

        ActionEvent actionCurrent = StatusEffects[currentIndex];
        SetTexture(actionCurrent.GetAnimationFloatingTexture() ?? new());
        SetCount(actionCurrent.GetAutoActivationsLeft());
        timeSinceLast += delta;
    }

}
