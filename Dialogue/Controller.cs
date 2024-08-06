using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Godot;
using Shared;

namespace ChessLike.Dialogue;

public partial class Controller
{
    public enum Command
    {
        SPEED,

    }
    private readonly Dictionary<char, Command> CommandDict= new(){
        {'S' , Command.SPEED}
    };
    public const char COMMAND_DELIMITER = '/';

    public float setting_speed_multiplier = 1;

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

    public void Advance(float delta)
    {
        if (conversation_curr == null)
        {
            throw new ArgumentNullException("Load a conversation first.");
        }

        advance_delta += delta * setting_speed_multiplier;
        string message = conversation_curr.GetCurrentMessage();

        while(advance_delta > 1 && advance_char_index < message.Length)
        {
            advance_delta -= 1;
            advance_char_index += 1;
        }
    }
/* 
    public void ParseCommand()
    {
        if (!(CurrentGetLetter() != COMMAND_DELIMITER))
        {
            throw new InvalidExpressionException("Commands must start with a /");
        }

        //Adavance to the command key and go past.
        CurrentChangeIndex(1);
        Command command_current = CommandDict[CurrentGetLetter()];
        CurrentChangeIndex(1);

        //Adance trough the parameter
        List<char> parameters_current = new();
        while (CurrentGetLetter() != COMMAND_DELIMITER)
        {
            parameters_current.Add(CurrentGetLetter());
            CurrentChangeIndex(1);
        }

        switch (command_current)
        {
            case Command.SPEED:
                float param = ParseCommandParameterFloat(parameters_current);
                setting_speed_multiplier = param;
                break;
            
            default:
                throw new Exception("???");
        }

        //Move off the final delimiter
        CurrentChangeIndex(1);

    }
 */
    public float ParseCommandParameterFloat(List<char> parameter_chars)
    {
        string output = new(parameter_chars.ToArray());
        return float.Parse(output);

    }

    public bool IsStarted()
    {
        return conversation_curr != null;
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

    public string GetText()
    {
        return new string(conversation_curr.GetCurrentMessage().Substr(0, advance_char_index));
    }
    public void ClearLog()
    {
        message_log.Clear();
    }



}
