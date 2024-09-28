namespace ChessLike.Shared;

public abstract class Equipment : Item
{
    public enum Slot
    {
        SUIT, HELMET, ACCESSORY, MAIN_HAND, OFF_HAND
    }

    public List<Slot> valid_slots = new();

}