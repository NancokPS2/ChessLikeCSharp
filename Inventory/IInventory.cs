using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Storage;

public interface IInventory
{
    public List<Item> GetItems();
}