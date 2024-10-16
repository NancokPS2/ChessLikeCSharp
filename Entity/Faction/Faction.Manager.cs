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

        public override Faction GetFromResource(FactionResource resource)
        {
            return FromResource(resource);
        }

        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "faction"
            );

        }

        public Faction GetFromEnum(EFaction faction)
        {
            Faction output = FromResource(
              GetAllResources().First(x => x.Identifier == faction)
            );
            return output;
        }
    }

    
}
