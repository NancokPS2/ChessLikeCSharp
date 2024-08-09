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
        public Mob owner;
        public Grid grid;
        public Action action_reference;
        public Vector3i location_selected;
        public List<Vector3i> locations_targeted = new();
        public List<Mob> mob_targets = new();

        public UsageParams(Mob owner, Grid grid, Action action_reference)
        {
            this.owner = owner;
            this.grid = grid;
            this.action_reference = action_reference;
            //this.location_selected = location_selected;
        }
    }
}
