using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class Manager<TManaged>
{
    protected static UniqueList<TManaged> Contents = new();

    public virtual List<TManaged> GetAll()
    {
        return Contents;
    }

    public void Add(TManaged managed)
    {
        Contents.Add(managed);
    }
    public void Add(TManaged[] managed)
    {
        foreach (var item in managed)
        {
            Add(item);
        }
    }
}
