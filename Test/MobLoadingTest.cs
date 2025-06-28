using ChessLike.Entity;
using Godot;
using System;
using System.Diagnostics;

public partial class MobLoadingTest : Node3D
{

    Mob mob;

    public MobLoadingTest()
    {
        mob = Global.ManagerMob.GetResource("Default");
    }

    public override void _Ready()
    {
        base._Ready();

        Console.WriteLine(mob.ToString());

        //Test updates.
        mob.UpdateEquipmentStatBoosts();
        mob.UpdateJobStatBoosts();

        Console.WriteLine(mob.Stats.BoostGetListOfStatChanges().ToString());

        //Test boosts
        TestStatBoost();

        Console.WriteLine("Stats post health boost");

    }

    private void TestStatBoost()
    {

        System.Console.WriteLine("--- Test start: Stat boosts\n" + mob.Stats.ToString());

        float preHealth = mob.Stats.GetMax(EStatName.HEALTH);
        float healthMult = 1.5f;

        MobStatBoost testBoost = new("TEST");
        testBoost.SetMultiplicativeMax(EStatName.HEALTH, healthMult);
        mob.Stats.BoostAdd(testBoost);
        mob.Stats.GetMax(EStatName.HEALTH);

        Debug.Assert(mob.Stats.GetMax(EStatName.HEALTH) == preHealth * healthMult);

        System.Console.WriteLine(mob.Stats.ToString() + "\n" + "--- Test end: Stat boosts");
    }
}
