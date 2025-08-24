using ChessLike.Entity;
using ChessLike.Entity.MobCommand;
using Godot;
using System;
using System.Diagnostics;

namespace Test;
public partial class MobLoadingTest : Node3D
{

    Mob mob;

    public MobLoadingTest()
    {
        mob = Global.ManagerMob.ResourceGet("Default");
    }

    public void TestChange(string name, bool start)
    {
        if (start)
        {
            Console.WriteLine($"--- Test start: {name}");
        }
        else
        {
            Console.WriteLine($"--- Test end: {name}");
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Console.WriteLine(mob.ToString());

        //Test updates.
        Console.WriteLine(mob.Stats.BoostGetListOfStatChanges().ToString());

        //Test boosts
        TestStatBoost();

        //Test movement
        TestMovement();

        TestMobCommands();
    }

    private void TestStatBoost()
    {
        TestChange("Stat boosts", true);
        Console.WriteLine(mob.Stats.ToString());

        float preHealth = mob.Stats.GetStat(EStatName.HEALTH);
        float healthMult = 1.5f;

        MobStatBoost testBoost = new("TEST");
        testBoost.SetMultiplicativeMax(EStatName.HEALTH, healthMult);
        mob.Stats.BoostAdd(testBoost);
        mob.Stats.GetStat(EStatName.HEALTH);

        Debug.Assert(mob.Stats.GetStat(EStatName.HEALTH) == preHealth * healthMult);

        Console.WriteLine(mob.Stats.ToString());
        TestChange("Stat boosts", false);
    }

    public void TestMovement()
    {
        TestChange("Movement", true);

        List<Vector3i> path = new() { Vector3i.FORWARD, Vector3i.FORWARD * 2, Vector3i.FORWARD, Vector3i.ZERO };
        Console.WriteLine(path);

        EventBus.MobMoved += TestMovementReportMove;
        mob.Move(path, new(EMovementMode.PLACE));
        EventBus.MobMoved -= TestMovementReportMove;

        TestChange("Movement", false);
    }

	private void TestMovementReportMove(Mob mob, List<Vector3i> path, MovementParameters moveParams)
	{
        Console.WriteLine($"Mob {mob.DisplayedName} moved from {path.First()} to {path.Last()}");
	}

    public void TestMobCommands()
    {
        TestChange("Mob Commands", true);

        float damageAmount = 10;
        MobCommandTakeDamage damage = new(){Damage = damageAmount};

        float health = mob.Stats.GetValue(EValueName.HEALTH);
        Console.WriteLine($"Health before damage: {health}");

        mob.CommandProcess(damage);

        float newHealth = mob.Stats.GetValue(EValueName.HEALTH);
        Console.WriteLine($"Health after damage: {newHealth}");

        Debug.Assert(health - damageAmount == newHealth);
        TestChange("Mob Commands", false);
    }
}
