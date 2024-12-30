using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface ISelectableCollection<TSelectable> where TSelectable : ISelectable
{
    public int MaxItemsSelected { set; get; }
    public List<TSelectable> GetSelectables();
    public List<TSelectable> GetSelected();
    public void Select(List<TSelectable> selectables) => this.SelectOnly(selectables);
}

public static class ISelectableCollectionExtension
{
    public static void SelectOnly<TSelectable>(this ISelectableCollection<TSelectable> collection, List<TSelectable> to_select) 
        where TSelectable : ISelectable
    {
        foreach (var item in collection.GetSelectables())
        {
            item.Selected = to_select.Contains(item);
        }
    }
}
