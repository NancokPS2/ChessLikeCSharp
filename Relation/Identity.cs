using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Relation;

public class Identity : IEquatable<Identity>, IFormattable, IEquatable<string>
{
    
    public const string INVALID_IDENTIFIER = "_UNKNOWN";
    public const string UNKNOWN_STRING = "Unknown";

    public string unique_identifier = INVALID_IDENTIFIER;
    public string displayed_name = UNKNOWN_STRING;
    public bool concealed = false;

    public Identity()
    {
    }

    public Identity(string unique_identifier, string displayed_name, bool concealed = false, bool allow_duplicate = true)
    {
        this.unique_identifier = unique_identifier;
        this.displayed_name = displayed_name;
        this.concealed = concealed;
    }


    public Identity(string unique_identifier)
    {
        this.unique_identifier = unique_identifier;
        displayed_name = unique_identifier;
    }

    public bool Equals(Identity other)
    {
        return this.unique_identifier == other.unique_identifier;
    }

    public bool Equals(string? other)
    {
        return this.unique_identifier == other;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return unique_identifier;
    }

}
