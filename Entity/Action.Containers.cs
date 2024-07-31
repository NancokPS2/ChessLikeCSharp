using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Action 
{

    public enum Error
    {
        NONE,
        MISSING_INTERFACES,//Missing interfaces, and therefore, the required methods for this to work.
        PARAMETER_BLOCK,//The parameters prevented this from working.
        FATAL,
    }
    
    /// <summary>
    /// Variables for Effect usage. Must be filled in the order of the variables.
    /// </summary>
    public class UsageParams
    {
        public Mob owner;
        public Grid grid;
        public Vector3i location_selected;
        public List<Vector3i> locations_targeted = new();
        public List<Mob> mob_targets = new();

        public UsageParams(Mob owner, Grid grid, Vector3i location_selected)
        {
            this.owner = owner;
            this.grid = grid;
            this.location_selected = location_selected;
        }
    }

    //Defines what can be affected with this and how.
    public class EffectFilterParams
    {
        public bool AffectMob = true;
        public bool IgnoreAlly = false;
        public bool IgnoreEnemy = false;
        public float MaximumHealthPercent = 1.0f;

        public EffectFilterParams()
        {
        }
    }

    //Properties used during the targeting phase to decide if the targets are valid.
    public class TargetParams
    {
        //Distance from the user.
        public float RangeMax = 4;
        //Only works if there is a mob being hit that was deemed as valid.
        public bool NeedValidMob = true;

        public TargetParams()
        {
        }
    }

    //Decides what effect it will have on targets
    public class EffectParams
    {
        public Dictionary<StatSet.Name, float> TargetStatChangeValue = new Dictionary<StatSet.Name, float>();
        public Dictionary<StatSet.Name, float> OwnerStatChangeValue = new Dictionary<StatSet.Name, float>();
        public Vector3i PositionChange = Vector3i.ZERO;

        public EffectParams()
        {
            PositionChange = Vector3i.ZERO;
            TargetStatChangeValue = new Dictionary<StatSet.Name, float>();
        }
    }
}
