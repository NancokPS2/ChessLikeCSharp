using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity.Action;

public partial class Ability
{
    public class Manager : SerializableManager<Ability, AbilityResource>
    {

        public override List<Ability> CreatePrototypes()
        {
            List<Ability> output = new()
            {
                Ability.Create(EAbility.HEAL),
                Ability.Create(EAbility.PUNCH),
                Ability.Create(EAbility.MOVE),
            };
            return output;
        }

        public override Ability GetFromResource(AbilityResource resource)
        {
            return FromResource(resource);
        }

        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "action"
            );

        }

        public Ability GetFromEnum(EAbility action)
        {
            return FromResource(
              GetAllResources().First(x => x.Identifier == action)
            );
        }
    }
}
