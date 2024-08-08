using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ChessLike.Shared.Identification;

public interface IIdentify
{
    public Identity Identity {get;set;}
}
