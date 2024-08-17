using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Dialogue;

public partial class Controller
{
    public partial class Parser
    {

        public enum Command
        {
            SPEED,
        }
        private enum State
        {
            OUTPUT,
            READING_COMMAND,
            READING_PARAMETER,
            EXECUTING_COMMAND,
        }

        private readonly Dictionary<string, Command> CommandDict= new(){
            {"SPD" , Command.SPEED}
        };

        public const char COMMAND_DELIMITER = '/';
        public const char COMMAND_PARAMETER_DELIMITER = '=';

        Settings settings = new();

        string parse_text = "Dummy /SPD=10/text";
        float parse_delta = 0;
        int parse_index = 0;

        State parse_state = State.OUTPUT;

        string buffered_command = "";
        string buffered_parameter = "";

        bool finished = false;
        List<char> output = new();

        public void StartParsing(string text, Settings settings_reference)
        {
            ResetParser();
            parse_text = text;
        }

        public void ResetParser()
        {
            settings = new();
            parse_text = "";
            parse_delta = 0;
            parse_index = 0;

            buffered_command = "";
            buffered_parameter = "";

            finished = false;
            output = new();
        }

        public void Advance(float delta)
        {
            parse_delta += delta * settings.SpeedMultiplier;

            float delta_required_to_advance = 1f / settings.SpeedMultiplier;
            while(delta > delta_required_to_advance)
            {
                //Check if it reached the end.
                if (parse_index > parse_text.Length)
                {
                    finished = true;
                    return;
                }

                //Decide what to do based on the current character.
                ParseChar(parse_text[parse_index]);

                parse_index++;
                parse_delta -= delta_required_to_advance;
            }
            
        }

        public void ExecuteCommandFromBuffer()
        {
            Command command;
            if (!CommandDict.TryGetValue(buffered_command, out command))
            {
                throw new Exception(buffered_command + " is not a valid command.");
            } 

            switch (command)
            {
                case Command.SPEED:
                    float parameter_float = float.Parse(buffered_parameter);
                    settings.SpeedMultiplier = parameter_float;
                    break;

                default:
                    break;
            }

            buffered_command = "";
            buffered_parameter = "";
        }

        public void ParseChar(char character)
        {
            switch (parse_state)
            {
                case State.OUTPUT:

                    //A command was started, change to read it.
                    if (character == COMMAND_DELIMITER)
                    {
                        parse_state = State.READING_COMMAND;
                    } 
                    //Do not act on parameter delimiters outside of commands.
                    //else if (char_current == COMMAND_PARAMETER_DELIMITER)
                    //{
                    //    
                    //}

                    //Nothing special, just add it to the output.
                    else
                    {
                        output.Add(character);
                    }
                    break;

                case State.READING_COMMAND:

                    //A command was already being parsed, finish it and run the command. No parameter was provided.)
                    if (character == COMMAND_DELIMITER)
                    {
                        ExecuteCommandFromBuffer();
                        parse_state = State.OUTPUT;
                    } 

                    //A parameter is to be parsed now.
                    else if (character == COMMAND_PARAMETER_DELIMITER)
                    {
                        parse_state = State.READING_PARAMETER;
                    }

                    //It is part of a command, add it to the buffer.
                    else
                    {
                        buffered_command += character;
                    }
                    break;
                
                case State.READING_PARAMETER:

                    //A parameter was being parsed, finish it and run the command.
                    if (character == COMMAND_DELIMITER)
                    {
                        ExecuteCommandFromBuffer();
                        parse_state = State.OUTPUT;
                    } 

                    //A parameter is to be parsed now.
                    else if (character == COMMAND_PARAMETER_DELIMITER)
                    {
                        throw new InvalidDataException("Cannot set two consecutive parameter delimiters.");
                    }

                    //It is part of a parameter, add it to the buffer.
                    else
                    {
                        buffered_parameter += character;
                    }
                    break;
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
        public Settings GetSettingsCurrent()
        {
            return settings;
        }

        public bool IsFinished()
        {
            return finished;
        }
    }


}
