using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.DebugDisplay;

public interface IDebugDisplay
{
    public string GetText()
    {
        return GetType().ToString();
    }

    public string GetName();
}
