using System.Dynamic;
using Shared;

namespace ChessLike.Dialogue;

//TODO
/// <summary>
/// Displays text over time based on the flags active. If the flags fail, the fallback is used.
/// </summary>
public partial class Conversation
{
    //A dictionary containing snippets and their identifiers.
    public List<Snippet> snippets = new(){new Snippet("Nothing, sorry.")};
    int snippet_index = 0;
    float delta = 0;
    Flags flags_in_use = new();

    Snippet GetCurrentSnippet()
    {
        return snippets[snippet_index];
    }

    public void AdvanceSnippet()
    {
        JumpToSnippet(snippet_index += 1);
    }

    public void JumpToSnippet(int index)
    {
        snippet_index = index;

        //Logic for snippet validity.
        Flags snippet_flags = GetCurrentSnippet().flags_for_valid;
        // If the current snippet has required flags but they are not present, jump to a set snippet.
        if (snippet_flags != Flags.Empty && !Flags.AContainsAllInB(flags_in_use, snippet_flags))
        {
            JumpToSnippet(GetCurrentSnippet().snippet_if_not_valid);
        }
    }
    //For jumping to snippets with an identifier.
    public void JumpToSnippet(string snippet_identifier)
    {
        int index_attempt = snippets.IndexOf( snippets.First(x => x.identifier == snippet_identifier) );

        //Fail if not present.
        if(index_attempt == -1)
        {
            throw new ArgumentException("No snippet with this identifier was found.");
        } else
        {
            JumpToSnippet(index_attempt);
        }
    }
    
    public string GetCurrentMessage()
    {
        return snippets[snippet_index].message;
    }
}
