using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.Storage;

public partial class Item
{
    public class Manager : SerializableManager<Item>
    {
        public override List<Item> CreatePrototypes()
        {
            List<Item> output = new()
            {

            };
            

            return output;
        }
        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "faction"
            );

        }
    }

    
}