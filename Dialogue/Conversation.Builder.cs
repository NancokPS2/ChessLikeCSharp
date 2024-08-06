using Sprache;

namespace ChessLike.Dialogue;

public partial class Conversation
{
    public class Builder
    {
        public Conversation target;

        public Builder()
        {
            target = new Conversation();
        }

        public Builder ChangeTarget(Conversation new_target)
        {
            target = new_target;
            return this;
        }

        public Conversation Result()
        {
            return target;
        }

    }


}