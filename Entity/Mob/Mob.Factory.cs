using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;
using ChessLike.Storage;

namespace ChessLike.Entity;

[Obsolete("Get rid of the Factory.")]
public partial class Mob
{
    /// <summary>
    /// This class should only be used to create a mob, it should not be stored in a variable.
    /// </summary>

    public static Mob CreatePrototype(EMobPrototype mob_template)
    {
        Mob output = new Mob();
        output = mob_template switch
        {
            EMobPrototype.HUMAN => output
                .ChainName("Human")
                .ChainRace(ERace.HUMAN),
            _ => new Mob()
        };
        return output;
    }


    public Mob ChainName(string name)
    {
        DisplayedName = new(name);
        return this;
    }

    public Mob ChainRace(ERace race)
    {
        Race = race;
        return this;
    }

    public Mob ChainFaction(EFaction faction)
    {
        Faction = faction;
        return this;
    }
}
