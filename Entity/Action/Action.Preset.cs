using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action
{
    public static class Preset
    {
        
        public static Action BasicAttack()
        {
            Action output = new();

            //Effect
            EffectParams.Attack effect = new();
            output.effect_params.Add(effect);

            //Targeting
            output.target_params.TargetingRange = 2;

            

            return output;
        }

        public static Action MagicBlast()
        {
            Action output = new();
            EffectParams.Attack effect = new(StatSet.Name.INTELLIGENCE, 0.8f);
            output.effect_params.Add(effect);

            return output;
        }

        public static Action Move()
        {
            Action output = new();

            return output;
        }
    }
}
