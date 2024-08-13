using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Dialogue;

public partial class Controller
{
    List<string> message_log = new();
    Conversation conversation_curr = new();

    float advance_delta = 0;
    int advance_char_index = 0;

    public void LoadConversation(Conversation conversation)
    {
        conversation_curr = conversation;
        advance_delta = 0;
        advance_char_index = 0;
    }

    public bool IsStarted()
    {
        return conversation_curr != null;
    }

    public string GetText()
    {
        return new string(conversation_curr.GetCurrentMessage().Substr(0, advance_char_index));
    }
    public void ClearLog()
    {
        message_log.Clear();
    }



}
