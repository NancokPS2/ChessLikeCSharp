using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace ChessLike.Shared.GenericStruct;

public class ClampFloat
{

    public List<Modifier> modifiers_innate = new();

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
    public float ChangeValue(float amount, Modifier[]? modifiers)
    {
        float output = amount;
        
        if(modifiers == null){ goto apply;}
        foreach (Modifier mod in modifiers)
        {
            if(ContainsType(mod.type))
            {
                switch (mod.mode)
                {
                    case Modifier.Mode.MULTIPLICATIVE:
                        output *= mod.value;
                        break;

                    case Modifier.Mode.ADDITIVE:
                        output += mod.value;
                        break;
                }
                
            }
        }

        apply:
        Current += output;

        return output;
    }

    public void AddModifier(Modifier modifier)
    {
        modifiers_innate.Add(modifier);
    }

    public void AddModifier(string modifier, float value = 1.0f, Modifier.Mode mode = Modifier.Mode.MULTIPLICATIVE)
    {
        modifiers_innate.Add(new Modifier(modifier, value, mode));
    }

    public bool RemoveModifier(Modifier modifier)
    {
        return modifiers_innate.Remove(modifier);
    }

    public bool RemoveModifier(string modifier_type, bool all = true)
    {
        if (all)
        {
            int count = modifiers_innate.RemoveAll( x => x.type == modifier_type );
            return count > 0;
        }else
        {
            int index = modifiers_innate.FindIndex( x => x.type == modifier_type);
            if (index == -1)
            {
                return false;
            }
            modifiers_innate.RemoveAt(index);
            return true;
        }

    }

    public void SetEnabledModifier(Modifier modifier, bool enabled)
    {
        modifier.enabled = enabled;
    }

    public void SetEnabledModifier(string modifier_type, bool enabled, bool all = true)
    {
        List<Modifier> to_affect = new();
        if (all)
        {
            to_affect = modifiers_innate.FindAll( x => x.type == modifier_type );
        }else
        {
            to_affect.Add(modifiers_innate.Find( x => x.type == modifier_type ));
        }
        for (int i = 0; i < to_affect.Count; i++)
        {
            Modifier modifier = to_affect[i];
            modifier.enabled = enabled;
        }
    }

    public void ClearModifiers()
    {
        modifiers_innate.Clear();
    }


    public void AddType(string type)
    {
        types.Add(type);
    }

    public bool RemoveType(string type)
    {
        return types.Remove(type);
    }

    public void ClearTypes()
    {
        types.Clear();
    }

    public bool ContainsType(string type)
    {

        return types.Contains(type);
    }

    //Modifier
    public struct Modifier
    {

        public enum Mode {ADDITIVE, MULTIPLICATIVE};


        public Mode mode = Mode.MULTIPLICATIVE;

        public bool enabled = true;
        public string type = "";

        public float value = 1;


        public Modifier(string type, float value, Mode mode)
        {
            this.mode = mode;
            this.type = type;
            Value = value;
        }

        public Modifier(string type)
        {
            this.type = type;
        }

        public float Value { get => value; set => this.value = Math.Clamp(value, 0, float.MaxValue); }
        
        public bool IsIncrease()
        {
            return Value > 1;
        }

        public static Dictionary<string, Modifier> ToDictionary(ref Modifier[] modifiers)
        {
            Dictionary<string, Modifier> output = new();
            foreach(Modifier modifier in modifiers)
            {
                output.Add(modifier.type, modifier);
            }
            return output;
        }

    }
}