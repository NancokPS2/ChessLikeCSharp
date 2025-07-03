using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

public partial class Global
{
    public static class GInput
    {
        public enum Button
        {
            MOVE_FW, MOVE_BW, MOVE_LT, MOVE_RT, MOVE_UP, MOVE_DN,  //Main directions
            VIEW_UP, VIEW_DN, VIEW_LT, VIEW_RT,  //View directions
            ACCEPT, CANCEL, MAIN_A, MAIN_B, //Face buttons
            SHOULDER_LT, SHOULDER_RT, SHOULDER_SUB_LT, SHOULDER_SUB_RT, //Quickly accessible
            PAUSE, MENU, //Other
        }

        private static Dictionary<Button, bool> ButtonsEnabled = new();
        private static System.Numerics.Vector2 AccumulatedMouse = new();

        public static bool MouseEnabled = true;
        public static System.Numerics.Vector2 ViewDeadZone = new(0.0f, 0.0f);

        public static void ParseMouseInputAsActionEvent(InputEvent input)
        {
            if (input is InputEventMouseMotion motion)
            {
                //X Axis
                if (motion.Relative.X > ViewDeadZone.X)
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_RT);
                    @event.Strength = Mathf.Abs(motion.Relative.X);
                    @event.EventIndex = 1;
                    @event.Pressed = true;
                    Input.ParseInputEvent(@event);
                }
                else
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_RT);
                    @event.Strength = 0;
                    @event.EventIndex = 1;
                    @event.Pressed = false;
                    Input.ParseInputEvent(@event);
                }

                if (motion.Relative.X < ViewDeadZone.X)
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_LT);
                    @event.Strength = Mathf.Abs(motion.Relative.X);
                    @event.EventIndex = 1;
                    @event.Pressed = true;
                    Input.ParseInputEvent(@event);
                }
                else
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_LT);
                    @event.Strength = 0;
                    @event.EventIndex = 1;
                    @event.Pressed = false;
                    Input.ParseInputEvent(@event);
                }

                //Y Axis
                if (motion.Relative.Y > ViewDeadZone.Y)
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_UP);
                    @event.Strength = Mathf.Abs(motion.Relative.Y);
                    @event.EventIndex = 1;
                    @event.Pressed = true;
                    Input.ParseInputEvent(@event);
                }
                else
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_UP);
                    @event.Strength = 0;
                    @event.EventIndex = 1;
                    @event.Pressed = false;
                    Input.ParseInputEvent(@event);
                }

                if (motion.Relative.Y < ViewDeadZone.Y)
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_LT);
                    @event.Strength = Mathf.Abs(motion.Relative.Y);
                    @event.EventIndex = 1;
                    @event.Pressed = true;
                    Input.ParseInputEvent(@event);
                }
                else
                {
                    InputEventAction @event = new();
                    @event.Action = GetActionName(Button.VIEW_DN);
                    @event.Strength = 0;
                    @event.EventIndex = 1;
                    @event.Pressed = false;
                    Input.ParseInputEvent(@event);
                }
            }
        }

        public static string GetActionName(Button button)
        {
            string action_name = button switch
            {
                //Directions
                Button.MOVE_FW => "move_forward",
                Button.MOVE_BW => "move_backward",
                Button.MOVE_LT => "move_left",
                Button.MOVE_RT => "move_right",
                Button.MOVE_UP => "move_up",
                Button.MOVE_DN => "move_down",

                //View directions
                Button.VIEW_UP => "view_up",
                Button.VIEW_DN => "view_down",
                Button.VIEW_LT => "view_left",
                Button.VIEW_RT => "view_right",

                //Main buttons
                Button.ACCEPT => "accept",
                Button.CANCEL => "cancel",
                Button.MAIN_A => "main_a",
                Button.MAIN_B => "main_b",


                //Pairs
                Button.SHOULDER_LT => "shoulder_left",
                Button.SHOULDER_RT => "shoulder_right",
                Button.SHOULDER_SUB_LT => "shoulder_sub_left",
                Button.SHOULDER_SUB_RT => "shoulder_sub_right",

                //Other
                Button.PAUSE => "pause",
                Button.MENU => "alt_pause",
                
                _ => throw new Exception()
            };
            return action_name;
        }

        public static bool IsButtonPressed(Button button)
        {
            string action = GetActionName(button);
            bool pressed = Godot.Input.IsActionPressed(action);
            return pressed;
        }
        public static bool IsActionJustReleased(Button button)
        {
            return Godot.Input.IsActionJustReleased(
                GetActionName(button)
            );
        }
        public static bool IsButtonJustPressed(Button button)
        {
            return Godot.Input.IsActionJustPressed(
                GetActionName(button)
            );
        }
        public static float GetActionStrength(Button button)
        {
            float strength = Godot.Input.GetActionStrength(
                GetActionName(button)
                );
            return strength;
        }

        public static System.Numerics.Vector2 GetViewVector(bool round_up)
        {
            System.Numerics.Vector2 output = new();
            if (MouseEnabled)
            {
                output.Y = round_up ? AccumulatedMouse.Y : MathF.Ceiling(AccumulatedMouse.Y);
                output.X = round_up ? AccumulatedMouse.X : MathF.Ceiling(AccumulatedMouse.X);
            }else
            {
                output.Y -= round_up ? GetActionStrength(Button.VIEW_DN) : MathF.Ceiling(GetActionStrength(Button.VIEW_DN));
                output.Y += round_up ? GetActionStrength(Button.VIEW_UP) : MathF.Ceiling(GetActionStrength(Button.VIEW_UP));
                output.X -= round_up ? GetActionStrength(Button.VIEW_LT) : MathF.Ceiling(GetActionStrength(Button.VIEW_LT));
                output.X += round_up ? GetActionStrength(Button.VIEW_RT) : MathF.Ceiling(GetActionStrength(Button.VIEW_RT));
            }
            return output;
        }
        public static System.Numerics.Vector3 GetMovementVector(bool round_up)
        {
            System.Numerics.Vector3 output = new();

            output.Z -= round_up ? GetActionStrength(Button.MOVE_BW) : MathF.Ceiling(GetActionStrength(Button.MOVE_BW));
            output.Z += round_up ? GetActionStrength(Button.MOVE_FW) : MathF.Ceiling(GetActionStrength(Button.MOVE_FW));
            output.X -= round_up ? GetActionStrength(Button.MOVE_LT) : MathF.Ceiling(GetActionStrength(Button.MOVE_LT));
            output.X += round_up ? GetActionStrength(Button.MOVE_RT) : MathF.Ceiling(GetActionStrength(Button.MOVE_RT));
            output.X -= round_up ? GetActionStrength(Button.MOVE_DN) : MathF.Ceiling(GetActionStrength(Button.MOVE_DN));
            output.X += round_up ? GetActionStrength(Button.MOVE_UP) : MathF.Ceiling(GetActionStrength(Button.MOVE_UP));

            return output;
        }

        public static void SetEnabled(Button button, bool enabled)
        {
            ButtonsEnabled[button] = enabled;
        }

        public static void SetEnabled(Button[] buttons, bool enabled)
        {
            foreach (Button button in buttons)
            {
                ButtonsEnabled[button] = enabled;
            }
        }

        public static void SetEnabled(bool enabled)
        {
            SetEnabled(Enum.GetValues<Button>(), enabled);
        }

        public static bool GetEnabled(Button button)
        {
            bool output;
            ButtonsEnabled.TryGetValue(button, out output);
            return output;
        }
    }
}
