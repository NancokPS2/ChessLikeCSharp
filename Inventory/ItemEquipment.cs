using System;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;

namespace ChessLike.Storage;

[GlobalClass]
public partial class ItemEquipment : Item
{
    public const string BOOST_SOURCE = "EQUIPMENT_SOURCE";

    [Export]
    public Godot.Collections.Array<Ability> AbilitiesGrantedToUser = new();
	[Export]
	public MobStatBoost StatBoost
	{
		get => statBoost;
        set
        {
            statBoost = value;
            statBoost.Source = BOOST_SOURCE;
		}
	}
	private MobStatBoost statBoost = new(BOOST_SOURCE);


	public override string ToString()
	{
		return base.ToString() + $"{StatBoost}\n";
	}

}
