using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static ChessLike.Entity.Action;

namespace ChessLike.Entity;

public partial class Action
{
    public class Loader : ILoadsByIdentity<EAction, Action>
    {

        public Action GetNewObject(EAction identity_enum)
        {
            Action output;
            output = identity_enum switch
            {
                EAction.PUNCH => Preset.BasicAttack(),
                EAction.HEAL => Preset.MagicBlast(),
                _ => Preset.BasicAttack(),
            };
            return output;
            
        }



    public static class Preset
    {
        
        public static Action BasicAttack()
        {
            Action output = new();

            //Effect
            EffectParams.Attack effect = new();
            output.effect_params.Add(effect);

            //Targeting
            output.target_params.TargetingRange = 1;

            return output;
        }

        public static Action MagicBlast()
        {
            Action output = new();
            EffectParams.Attack effect = new(StatName.INTELLIGENCE, 0.8f);
            output.effect_params.Add(effect);

            return output;
        }

        public static Action Move()
        {
            Action output = new();
            output.target_params.TargetingRangeStatBonus = StatName.MOVEMENT;

            return output;
        }
    }

    }
}