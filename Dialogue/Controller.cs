using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Shared;

namespace ChessLike.Dialogue;

public delegate void ChangingText(string text);
public delegate void Finishing();
//public delegate void ChangeConversation(Conversation conversation);
public class DialogueController
{
    public event ChangingText? TextChanged;
    //public event ChangeConversation ConversationChanged;
    public event Finishing? Finished;

    List<string> message_log = new();
    Flags flags = new();
    Conversation? conversation_curr = null;
    List<Phrase> phrases_curr = new();
    int phrase_curr_index = 0;

    public void StartConversation(Conversation conversation, Flags flags)
    {
        this.conversation_curr = conversation;
        this.flags = flags;

        if(conversation_curr.flags_required.Contains(flags))
        {
            phrases_curr = conversation_curr.phrases_if_true;
        }else
        {
            phrases_curr = conversation_curr.phrases_if_false;
        }
        phrase_curr_index = 0;
        TextChanged?.Invoke(GetPhraseText(0));
    }

    public void NextPhrase()
    {
        //Cannot advance if no conversation has started
        if (!IsStarted()){throw new NullReferenceException("Cannot advance, start a conversation first.");}

        phrase_curr_index += 1;
        string text = GetPhraseText(phrase_curr_index);
        //If there is text, report it.
        if(text != string.Empty)
        {
            TextChanged?.Invoke(text);

        // There is no more text, conversation ended.
        }else{

            //Start the next one if applicable.
            // There must be a next conversation to continue.
            if (conversation_curr?.next_conversation_true == null){return;}
            //A conversation must have been started beforehand.
            if(flags == Flags.Empty){throw new NullReferenceException("flags is null, tried to advance without starting a conversation.");}
                
            StartConversation(conversation_curr.next_conversation_true, flags);
            
        }
    }

    public bool IsStarted()
    {
        return conversation_curr != null && flags != Flags.Empty;
    }

    public string GetCurrentText()
    {
        return GetPhraseText(null);
    }

    string GetPhraseText(int? index = null)
    {
        if (conversation_curr == null)
        {
            return string.Empty;
        }
        index ??= phrase_curr_index;

        if(phrase_curr_index < phrases_curr.Count)
        {
            return phrases_curr[phrase_curr_index].message;
        }else{
            return string.Empty;
        }
    }


    public void Finish()
    {
        conversation_curr = null;
        phrase_curr_index = 0;
        phrases_curr.Clear();
        Finished?.Invoke();
    }

    public string[] GetLogged(int max_amount = 10)
    {
        if (max_amount < 1)
        {
            return message_log.ToArray();
        }else
        {
            max_amount = Math.Clamp(max_amount, 0, message_log.Count);
            string[] output = new string[max_amount];
            for (int i = (max_amount - 1); i >= 0; i--)
            {
                string message = message_log[i];
                output[i] = message;
            }
            return output;
        }

    }
    public void ClearLog()
    {
        message_log.Clear();
    }



}
