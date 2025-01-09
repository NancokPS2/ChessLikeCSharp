using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;

namespace ChessLike.Shared.Storage;

public partial class Item : ISerializable
{
    public string GetFileName()
    {
        string flags = "";
        foreach (var item in Flags)
        {
            flags +=  "_" + item.ToString();
        }
        return Name + flags + ".xml";
    }

    public string GetSubDirectory()
    {
        return "item";
    }
}
