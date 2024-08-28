using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ChessLike.Shared.Serialization;

//
public interface ISerializable
{
    const string UNDEFINED = "";
    public string GetFileName()
    {
        if (this is IIdentify identify)
        {
            return identify.Identity.Identifier;
        }
        return UNDEFINED;
    }
}
