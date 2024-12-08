using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IDescription
{
    public string GetDescriptiveName();
    public string GetDescription(bool extended = false);
    public string GetDescriptionRich(bool extended = false) => GetDescription(extended);
}
