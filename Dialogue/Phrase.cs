using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Dialogue;

public struct Phrase
{
    //public Image portrait;
    public string message;

    public float speed_modifier;

    public readonly List<string> flags_true_on_read;
    public readonly List<string> flags_false_on_read;
}
