using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.GenericStruct;

public interface IEnumIdentifier<TEnum>
{
    public TEnum Identifier {get;set;}
}
