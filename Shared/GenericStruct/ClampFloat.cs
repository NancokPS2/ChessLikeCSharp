using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace ChessLike.Shared.GenericStruct;

public class ClampFloat
{

    public ClampFloat()
    {
        Current = Max;
    }

    public ClampFloat(float _current_and_max)
    {
        Max = _current_and_max;
        Min = 0;
        Current = _current_and_max;
    }

    public ClampFloat(float _current, float _max, float _min = 0)
    {
        Max = _max;
        Min = _min;
        Current = _current;
    }

    /// <summary>
    /// Determines which kinds of modifiers affect this value.
    /// </summary>
    /// <returns></returns>
    private List<string> types = new();

    //These MUST be public to be serialized!
    public float Max = 100;
    public float Min = 0;
    public float Current;
    
        // Summary:
        //     Changes Current by the amount and apply modifiers.
        //
        // Parameters:
        //   amount:
        //     The amount to modify Current by.
        //
        //   modifiers:
        //     Any Modifier objects to use on the operation.
        //
        // Returns:
        //     The total change after modifiers, ignoring bounds.

    public void SetMax(float value)
    {
        Max = value;
    }
    public float GetMax()
    {
        return Max;
    }
    public void SetMin(float value)
    {
        Min = value;
    }
    public float GetMin()
    {
        return Min;
    }
    public void SetCurrent(float value)
    {
        Current = Math.Clamp(value, Min, Max);
    }
    public float GetCurrent()
    {
        return Current;
    }

    /// <summary>
    /// Changes the Current variable respecting modifiers.
    /// </summary>
    /// <param name="amount">How much to change Current.</param>
    /// <param name="modifiers">Modifiers to apply to the change.</param>
    /// <returns>The actual change, including modifiers.</returns>
    public float ChangeValue(float amount)
    {
        float output = amount;
    
        Current += output;

        return output;
    }
    public float ChangeMax(float amount)
    {
        float output = amount;
        
        Max += output;

        return output;
    }

    public void Fill() => SetCurrent(GetMax());

    public override string ToString()
    {
        return GetCurrent().ToString() + "/" + GetMax().ToString();
    }

}