using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public class TimerFloat
{
    public bool Frozen = false;
    public float WaitTime; 
    private float CurrentTime;


    public TimerFloat(float? wait_time)
    {
        if (wait_time is float wait)
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

    public void Advance(float amount)
    {
        if (Frozen){return;}

        CurrentTime -= amount;
    }

    public float GetTimeLeft()
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
