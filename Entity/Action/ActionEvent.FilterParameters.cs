using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class ActionEvent
{
    //Defines what can be affected with this and how.
    public class MobFilterParameters
    {
        //If a mob is standing in a targeted location, they are included in the mob list of the UsageParameters.
        public bool PickMobInTargetPos = true;
        //Only the owner is a valid target. Should have a range of 0.
        public bool OnlyAffectOwner = false;

        //The target cannot be an ally.
        public bool CannotAffectAlly = false; 

        //The target cannot be an enemy.
        public bool CannotAffectEnemy = false;

        //The target must be below this health.
        public float MaximumHealthPercent = 1.0f;

        public bool IsMobValid(UsageParameters usageParams, Mob mob)
        {
            //Faction target_fac = Global.ManagerFaction.GetFromEnum(mob.Faction);
            Faction owner_fac = Global.ManagerFaction.GetFromEnum(usageParams.OwnerRef.Faction);
            //Must be the owner?
            if (mob != usageParams.OwnerRef && OnlyAffectOwner)
            {
                return false;
            }
            //Health must be below this.
            else if (mob.Stats.GetValuePrecent(StatName.HEALTH) >= MaximumHealthPercent)
            {
                return false;
            }
            else if (owner_fac.IsAlly(mob.Faction) && CannotAffectAlly)
            {
                return false;
            }
            else if(owner_fac.IsEnemy(mob.Faction) && CannotAffectEnemy)
            {
                return false;
            }
            return true;
        }
    }
}
