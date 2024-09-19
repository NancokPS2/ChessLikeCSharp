using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.DebugDisplay;
using ChessLike.Turn;

namespace Godot;

[GlobalClass]
public partial class TestTurnManager : Node
{

    TurnManager manager = new();

    public override void _Ready()
    {
        base._Ready();
        manager.Add( new TurnTakerTest(100, "normal") );
        manager.Add( new TurnTakerTest(50, "fast") );
        manager.Add( new TurnTakerTest(25, "faster") );
        manager.Add( new TurnTakerTest(300, "SLOW") );

        manager.TurnEnded += OnTurnEnded;
        manager.TurnStarted += OnTurnStarted;

        Timer timer = new();
        timer.Timeout += Cycle;
        AddChild(timer);
        timer.OneShot = false;
        timer.Start(1f);

        DebugDisplay.Instance.Add(manager);


    }


    public void Cycle()
    {
        manager.StartTurn();
        manager.EndTurn();

        string output = ""; 
        foreach (var item in manager.GetParticipants())
        {
            if (item is TurnTakerTest taker)
            {
                output += taker.Name + " | " + taker.DelayCurrent + "\n";
            }
        }
        GD.Print(output);
    }

    public void OnTurnEnded(ITurn who)
    {
        GD.Print("Turn ended by " + (who as TurnTakerTest).Name);
    }
    public void OnTurnStarted(ITurn who)
    {
        GD.Print("Turn started by " + (who as TurnTakerTest).Name);
    }
}

public class TurnTakerTest : ITurn
{
    public float DelayCurrent { get; set; }
    public float DelayToAddOnTurnEnd { get; set; }

    public string Name;
    public float DelayDefault;

    public TurnTakerTest(float def, string name)
    {
        this.DelayDefault = def;
        this.Name = name;
    }

    public float GetDelayBase()
    {
        return DelayDefault;
    }
}
