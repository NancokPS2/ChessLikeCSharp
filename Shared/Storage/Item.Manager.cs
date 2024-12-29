using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ChessLike.Shared.Storage;

public partial class Item
{
    public class Manager : SerializableManager<Item, ItemResource>
    {
        public override List<Item> CreatePrototypes()
        {
            List<Item> output = new();        
            return output;
        }

        public Item GetFromEnum(EItem identifier)
        {
            ItemResource res = GetAllResources().First<ItemResource>(x => x.Identifier == identifier);
            Item item = FromResource(res);
            return item;
        }
    }

    
}
