using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

#region ISelectableList
public interface ISelectableList<TSelectable> : IListContainer<TSelectable> where TSelectable : ISelectable
{
    public int MaxItemsSelected { set; get; }
    public List<TSelectable> GetSelected();
    public List<TSelectable> GetHovered();
    public void Select(List<TSelectable> selectables) => this.SelectOnly(selectables);
}

public static class ISelectableCollectionExtension
{
    public static void SelectOnly<TSelectable>(this ISelectableList<TSelectable> collection, List<TSelectable> to_select) 
        where TSelectable : ISelectable
    {
        foreach (var item in collection.GetElements())
        {
            item.Selected = to_select.Contains(item);
        }
    }
}
#endregion

#region ISelectable
public interface ISelectable
{
    public bool Selected {get;set;}
    public bool Hovered {get;set;}
}
#endregion