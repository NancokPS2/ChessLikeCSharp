using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public interface IDebugDisplay
{
    public string GetText();

    public string GetName()
    {
        return GetType().ToString();
    }
}
