using System;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class ItemEquipment : Item
{
    public const string BOOST_SOURCE = "EQUIPMENT_SOURCE";

    [Export]
    public Godot.Collections.Array<Ability> AbilitiesGrantedToUser = new();
    [Export]
    public MobStatSet.StatBoost StatBoost = new(BOOST_SOURCE);
}
