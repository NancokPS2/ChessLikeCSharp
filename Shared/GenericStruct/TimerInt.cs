using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class TimerInt
{
    public bool Frozen = false;
    public int WaitTime; 
    private int CurrentTime;

    public TimerInt(int? wait_time)
    {
        if (wait_time is int wait)
        {
            WaitTime = wait;
            Reset();    
        }
        else
        {
            WaitTime = 1;
            Frozen = true;
        }

    }

    public void Advance(int amount)
    {
        if (Frozen){return;}

        CurrentTime -= amount;
    }

    public int GetTimeLeft()
    {
        return CurrentTime;
    }

    public void Reset()
    {
        CurrentTime = WaitTime;
    }

    public bool IsFinished()
    {
        return CurrentTime <= 0;
    }
}
