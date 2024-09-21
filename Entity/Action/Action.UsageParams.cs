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
    /// <summary>
    /// Variables for Effect usage. Must be filled in the order of the variables.
    /// </summary>
    public class UsageParams
    {
        public Mob OwnerRef;
        public Grid GridRef;
        public Action ActionRef;
        /// <summary>
        /// Positions that where selected during the targeting of this action
        /// </summary>
        /// <returns></returns>
        public List<Vector3i> PositionsTargeted = new();
        /// <summary>
        /// Mobs found in the targeted locations or caught in the AoE and filtered afterwards to be deemed as valid to affect.
        /// </summary>
        /// <returns></returns>
        public List<Mob> MobsTargeted = new();

        public UsageParams(Mob owner, Grid grid, Action action_reference)
        {
            this.OwnerRef = owner;
            this.GridRef = grid;
            this.ActionRef = action_reference;
            //this.location_selected = location_selected;
        }
    }
}
