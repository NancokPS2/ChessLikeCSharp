using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Dialogue;

public struct Snippet
{
    public string identifier = String.Empty;
    public string message = "Nothing, sorry.";

    public Flags flags_for_valid = new Flags();
    public string snippet_if_not_valid = String.Empty;

    public Snippet(string message, string identifier, Flags flags_for_valid, string snippet_if_not_valid)
    {
        this.message = message;
        this.identifier = identifier;
        this.flags_for_valid = flags_for_valid;
        this.snippet_if_not_valid = snippet_if_not_valid;
    }

    public Snippet(string message, string identifier)
    {
        this.message = message;
        this.identifier = identifier;
    }

    public Snippet(string message)
    {
        this.message = message;
    }

}
