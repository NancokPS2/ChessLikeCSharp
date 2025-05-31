using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer;

namespace ChessLike.Entity.Action;

public partial class Passive
{
    public class DurationParameters
    {

        public TimerInt Turns;

        public TimerInt Uses;

        public TimerFloat Delay;

        public DurationParameters(int? turns, int? uses = null, float? delay = null)
        {
            Turns = new(turns);
            Uses = new(uses);
            Delay = new(delay);
        }

        public void Reset()
        {
            Turns.Reset();
            Uses.Reset();
            Delay.Reset();

        }

        public void AdvanceUses(int amount = 1) => Uses.Advance(amount);
        public void AdvanceTurns(int amount = 1) => Turns.Advance(amount);
        public void AdvanceDelay(float amount) => Delay.Advance(amount);

        public void FreezeUses(bool freeze) => Uses.Frozen = freeze;
        public void FreezeTurns(bool freeze) => Turns.Frozen = freeze;
        public void FreezeDelay(bool freeze) => Delay.Frozen = freeze;

        public bool IsFinished()
        {
            //Conditions will count as finished if they ARE finished and NOT frozen.
            bool turns_finished = Turns.IsFinished() && !Turns.Frozen;
            bool uses_finished = Uses.IsFinished() && !Uses.Frozen;
            bool delay_finished = Delay.IsFinished() && !Delay.Frozen;

            return turns_finished || uses_finished || delay_finished;
        }
    }
}
