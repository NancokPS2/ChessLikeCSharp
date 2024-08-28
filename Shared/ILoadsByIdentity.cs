using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface ILoadsByIdentity<TIdentEnum, TLoaded> 
    where TIdentEnum : notnull, Enum
{

    public TLoaded GetNewObject(TIdentEnum identity_enum);

}
