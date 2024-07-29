using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Relation;

public struct Identity : IEquatable<Identity>, IFormattable, IEquatable<string>
{
    
    public const string INVALID = "_UNKNOWN";
    public const string UNKNOWN_STRING = "Unknown";

    public string unique_identifier;
    public string displayed_name {get => concealed ? displayed_name : UNKNOWN_STRING; set => displayed_name = value;}
    public bool concealed;

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
