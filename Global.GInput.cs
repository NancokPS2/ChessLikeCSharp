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
            ACCEPT, CANCEL, SPECIAL_A, SPECIAL_B, //"Face buttons"
            QUICK_A, QUICK_B, QUICK_C, QUICK_D //"Quickly accessible"
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
                
                _ => throw new Exception()
            };
            return action_name;
        }

        public static bool InputIsButtonPressed(Button button)
        {

            return Godot.Input.IsActionPressed(
                GetActionName(button)
                );
        }
        public static float InputGetActionStrength(Button button)
        {
            return Godot.Input.GetActionStrength(
                GetActionName(button)
                );
        }

        public static System.Numerics.Vector2 InputGetViewVector(bool round_up)
        {
            System.Numerics.Vector2 output = new();
            if (MouseEnabled)
            {
                output.Y = round_up ? AccumulatedMouse.Y : MathF.Ceiling(AccumulatedMouse.Y);
                output.X = round_up ? AccumulatedMouse.X : MathF.Ceiling(AccumulatedMouse.X);
            }else
            {
                output.Y -= round_up ? InputGetActionStrength(Button.VIEW_DN) : MathF.Ceiling(InputGetActionStrength(Button.VIEW_DN));
                output.Y += round_up ? InputGetActionStrength(Button.VIEW_UP) : MathF.Ceiling(InputGetActionStrength(Button.VIEW_UP));
                output.X -= round_up ? InputGetActionStrength(Button.VIEW_LT) : MathF.Ceiling(InputGetActionStrength(Button.VIEW_LT));
                output.X += round_up ? InputGetActionStrength(Button.VIEW_RT) : MathF.Ceiling(InputGetActionStrength(Button.VIEW_RT));
            }
            return output;
        }
        public static System.Numerics.Vector3 InputGetMovementVector(bool round_up)
        {
            System.Numerics.Vector3 output = new();

            output.Z -= round_up ? InputGetActionStrength(Button.MOVE_BW) : MathF.Ceiling(InputGetActionStrength(Button.MOVE_BW));
            output.Z += round_up ? InputGetActionStrength(Button.MOVE_FW) : MathF.Ceiling(InputGetActionStrength(Button.MOVE_FW));
            output.X -= round_up ? InputGetActionStrength(Button.MOVE_LT) : MathF.Ceiling(InputGetActionStrength(Button.MOVE_LT));
            output.X += round_up ? InputGetActionStrength(Button.MOVE_RT) : MathF.Ceiling(InputGetActionStrength(Button.MOVE_RT));
            output.X -= round_up ? InputGetActionStrength(Button.MOVE_DN) : MathF.Ceiling(InputGetActionStrength(Button.MOVE_DN));
            output.X += round_up ? InputGetActionStrength(Button.MOVE_UP) : MathF.Ceiling(InputGetActionStrength(Button.MOVE_UP));

            return output;
        }

        public static void InputSetEnabled(Button button, bool enabled)
        {
            ButtonsEnabled[button] = enabled;
        }

        public static void InputSetEnabled(Button[] buttons, bool enabled)
        {
            foreach (Button button in buttons)
            {
                ButtonsEnabled[button] = enabled;
            }
        }

        public static void InputSetEnabled(bool enabled)
        {
            InputSetEnabled(Enum.GetValues<Button>(), enabled);
        }

        public static bool InputGetEnabled(Button button)
        {
            bool output;
            ButtonsEnabled.TryGetValue(button, out output);
            return output;
        }
    }
}
