using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Passive
{
    public class LimitParameters
    {
        const float DISABLE_MAX = float.MaxValue;

        private ClampFloat turns = new(DISABLE_MAX);
        private ClampFloat uses = new(DISABLE_MAX);
        private ClampFloat delay = new(DISABLE_MAX);

        public float TurnsBase { get => turns.GetMax(); set => turns.SetMax(value); }
        public float UsesBase { get => uses.GetMax(); set => uses.SetMax(value); }
        public float DelayBase { get => delay.GetMax(); set => delay.SetMax(value); }

        public void Reset()
        {
            turns.Fill();
            uses.Fill();
            delay.Fill();
        }

        public void AdvanceUses(float amount = 1) => uses.ChangeValue(-amount);
        public void AdvanceTurns(float amount = 1) => turns.ChangeValue(-amount);
        public void AdvanceDelay( float amount ) => delay.ChangeValue(-amount);

        public void DisableUses() => uses.SetMax(DISABLE_MAX);
        public void DisableTurns() => turns.SetMax(DISABLE_MAX);
        public void DisableDelay() => delay.SetMax(DISABLE_MAX);

        public bool IsUsesDisabled() => uses.GetMax() == DISABLE_MAX;
        public bool IsTurnsDisabled() => turns.GetMax() == DISABLE_MAX;
        public bool IsDelayDisabled() => delay.GetMax() == DISABLE_MAX;

        public bool IsFinished()
        {
            bool no_turns = turns.GetCurrent() <= 0 && !IsUsesDisabled();
            bool no_uses = uses.GetCurrent() <= 0 && !IsTurnsDisabled();
            bool no_delay = delay.GetCurrent() <= 0 && !IsDelayDisabled();

            return no_turns || no_uses || no_delay;
        }
    }
}
