using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ChessLike.Shared.Identification;

public class Identity : IEquatable<Identity>, IFormattable, IEquatable<string>
{
    
    public const string INVALID_IDENTIFIER = "_UNKNOWN";
    public const string UNKNOWN_STRING = "Unknown";

    public string Identifier = INVALID_IDENTIFIER;
    public string Name = UNKNOWN_STRING;
    public bool Concealed = false;

    public Identity()
    {
    }

    public Identity(string unique_identifier, string displayed_name, bool concealed = false, bool allow_duplicate = true)
    {
        this.Identifier = unique_identifier;
        this.Name = displayed_name;
        this.Concealed = concealed;
    }


    public Identity(string unique_identifier)
    {
        this.Identifier = unique_identifier;
        Name = unique_identifier;
    }

    public bool Equals(Identity? other)
    {
        if(other == null) {return false;}
        return this.Identifier == other.Identifier;
    }

    public bool Equals(string? other)
    {
        return this.Identifier == other;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Identifier;
    }

}
