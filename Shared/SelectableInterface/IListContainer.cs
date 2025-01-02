using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IListContainer<TElement>
{
    public List<TElement> GetElements();
    public void AddElement(TElement element);
    public void RemoveElement(TElement element);

}
