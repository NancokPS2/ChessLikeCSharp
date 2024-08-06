using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action
{
    //Defines what can be affected with this and how.
    public class EffectFilterParams
    {
        //Only the owner is a valid target. Should have a range of 0.
        public bool OnlyOwner = false;

        //The target can be a mob. Set it to false if you only want to target the terrain.
        public bool AffectMob = true; 

        //The target cannot be an ally.
        public bool IgnoreAlly = false; 

        //The target cannot be an enemy.
        public bool IgnoreEnemy = false;

        //The target must be below this health.
        public float MaximumHealthPercent = 1.0f;

    }
}
