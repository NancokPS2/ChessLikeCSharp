using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public static class CompatibilityHelper
{
    public static bool IsObjectOfType(Object target, Type[] required_types)
    {
        Type[] target_interfaces = target.GetType().GetInterfaces();
        foreach (Type type in required_types)
        {
            if(!target_interfaces.Contains(type))
            {
                return false;
            }
        }
        return true;
    }
}
