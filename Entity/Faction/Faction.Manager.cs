using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Faction
{
    public class Manager : SerializableManager<Faction, FactionResource>
    {
        public override List<Faction> CreatePrototypes()
        {
            List<Faction> output = new();
            foreach (var item in Enum.GetValues<EFaction>())
            {
                output.Add(CreatePrototype(item));
            }

            return output;
        }

        public Faction GetFromEnum(EFaction faction)
        {
            Faction output = GetAllPooled().First(x => x.Identifier == faction);

            if (output is null)
            {
                output = FromResource(
                GetAllResources().First(x => x.Identifier == faction)
                );
            }
            return output;
        }
    }

    
}
