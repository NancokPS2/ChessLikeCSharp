using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Global
{
    public static class GInput
    {
        public enum Button
        {
            MOVE_FW, MOVE_BW, MOVE_LT, MOVE_RT, MOVE_UP, MOVE_DN,  //Main directions
            VIEW_UP, VIEW_DN, VIEW_LT, VIEW_RT,  //View directions
            ACCEPT, CANCEL, SPECIAL_A, SPECIAL_B, //Face buttons
            QUICK_A, QUICK_B, QUICK_C, QUICK_D, //Quickly accessible
            PAUSE, MENU, //Other
        }

        private static Dictionary<Button, bool> ButtonsEnabled = new();
        public static Window RootNode = new Node().GetWindow();
        public static bool MouseEnabled = true;

        private static System.Numerics.Vector2 AccumulatedMouse = new();

        public static void ConnectToWindow()
        {
            RootNode = new Node().GetWindow();
            RootNode.WindowInput += HandleInput;    
        }

        public static void HandleInput(InputEvent input)
        {
            if (input is InputEventMouseMotion motion)
            {
                AccumulatedMouse = new System.Numerics.Vector2(motion.Relative.X, motion.Relative.Y);
            }
        }

        private static string GetActionName(Button button)
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
                Button.SPECIAL_A => "special_a",
                Button.SPECIAL_B => "special_b",


                //Pairs
                Button.QUICK_A => "quick_left",
                Button.QUICK_B => "quick_right",
                Button.QUICK_C => "other_left",
                Button.QUICK_D => "other_right",

                //Other
                Button.PAUSE => "pause",
                Button.MENU => "alt_pause",
                
                _ => throw new Exception()
            };
            return action_name;
        }

        public static bool IsButtonPressed(Button button)
        {

            return Godot.Input.IsActionPressed(
                GetActionName(button)
                );
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
            return Godot.Input.GetActionStrength(
                GetActionName(button)
                );
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
