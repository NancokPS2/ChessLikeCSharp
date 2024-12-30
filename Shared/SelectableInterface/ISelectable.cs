namespace ChessLike.Shared;

public interface ISelectable
{
    public delegate void SelectionChange(bool selected);
    public event SelectionChange? SelectedEvent;

    public bool Selected {get;set;}
}