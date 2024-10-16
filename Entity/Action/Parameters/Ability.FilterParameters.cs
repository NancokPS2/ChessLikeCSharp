using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Ability
{
    //Defines what can be affected with this and how.
    public class FilterParameters
    {
        //Only the owner is a valid target. Should have a range of 0.
        public bool OnlyAffectOwner = false;

        //The target can be a mob. Set it to false if you only want to target the terrain.
        public bool CanAffectMob = true; 

        //The target cannot be an ally.
        public bool CannotAffectAlly = false; 

        //The target cannot be an enemy.
        public bool CannotAffectEnemy = false;

        //The target must be below this health.
        public float MaximumHealthPercent = 1.0f;

    }
}
