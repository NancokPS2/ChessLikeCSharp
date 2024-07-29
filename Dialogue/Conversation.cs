using Shared;

namespace ChessLike.Dialogue;


/// <summary>
/// Displays text over time based on the flags active. If the flags fail, the fallback is used.
/// </summary>
public class Conversation
{
    public Flags flags_required;

    public Font font;
    public List<Phrase> phrases_if_true;
    public List<Phrase> phrases_if_false;
    public Conversation? next_conversation_true;
    public Conversation? next_conversation_false;

    public Conversation(
        List<Phrase> phrases_if_true,
        List<Phrase> phrases_if_false,
        Flags flags_required,
        Font font, 
        Conversation? next_conversation_true = null,
        Conversation? next_conversation_false = null
        )
    {

        this.phrases_if_true = phrases_if_true;
        this.phrases_if_false = phrases_if_false;
        this.flags_required = flags_required;
        this.font = font;
        this.next_conversation_true = next_conversation_true;
        this.next_conversation_false = next_conversation_false;
        
        
    }

    public Conversation(
        List<Phrase> phrases,
        Flags flags_required,
        Font font) : 
        this(phrases, phrases, flags_required, font)
    {}

    public Conversation(
        List<Phrase> phrases,
        Font font) : 
        this(phrases, phrases, new Flags(), font)
    {}


}
