using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Godot;
using Godot.NativeInterop;

namespace UNUSED;

public interface IGInput
{
    public enum Button
    {
        FORWARD, BACKWARD, LEFT, RIGHT, UP, DOWN,  //Main directions
        ACCEPT, CANCEL, SPECIAL_A, SPECIAL_B, //"Face buttons"
        QUICK_A_LEFT, QUICK_A_RIGHT, QUICK_B_LEFT, QUICK_B_RIGHT //"Pairs"
    }

    public Dictionary<Button, bool> ButtonsEnabled {get;set;}

    private static string GetActionName(Button button)
    {
        string action_name = button switch
        {
            //Directions
            Button.FORWARD => "move_forward",
            Button.BACKWARD => "move_backward",
            Button.LEFT => "move_left",
            Button.RIGHT => "move_right",
            Button.UP => "move_up",
            Button.DOWN => "move_down",

            //Main buttons
            Button.ACCEPT => "accept",
            Button.CANCEL => "cancel",
            Button.SPECIAL_A => "special_a",
            Button.SPECIAL_B => "special_b",


            //Pairs
            Button.QUICK_A_LEFT => "quick_left",
            Button.QUICK_A_RIGHT => "quick_right",
            Button.QUICK_B_LEFT => "other_left",
            Button.QUICK_B_RIGHT => "other_right",
            
            _ => throw new Exception()
        };
        return action_name;
    }

    public bool InputIsButtonPressed(Button button)
    {

        return Godot.Input.IsActionPressed(
            GetActionName(button)
            );
    }
    public float InputGetActionStrength(Button button)
    {
        return Godot.Input.GetActionStrength(
            GetActionName(button)
            );
    }

    public System.Numerics.Vector3 InputGetMovementVector(bool round_up)
    {
        System.Numerics.Vector3 output = new();

        output.Z -= round_up ? InputGetActionStrength(Button.BACKWARD) : MathF.Ceiling(InputGetActionStrength(Button.BACKWARD));
        output.Z += round_up ? InputGetActionStrength(Button.FORWARD) : MathF.Ceiling(InputGetActionStrength(Button.FORWARD));
        output.X -= round_up ? InputGetActionStrength(Button.LEFT) : MathF.Ceiling(InputGetActionStrength(Button.LEFT));
        output.X += round_up ? InputGetActionStrength(Button.RIGHT) : MathF.Ceiling(InputGetActionStrength(Button.RIGHT));
        output.X -= round_up ? InputGetActionStrength(Button.DOWN) : MathF.Ceiling(InputGetActionStrength(Button.DOWN));
        output.X += round_up ? InputGetActionStrength(Button.UP) : MathF.Ceiling(InputGetActionStrength(Button.UP));

        return output;
    }

    public void InputSetEnabled(Button button, bool enabled)
    {
        ButtonsEnabled[button] = enabled;
    }

    public void InputSetEnabled(Button[] buttons, bool enabled)
    {
        foreach (Button button in buttons)
        {
            ButtonsEnabled[button] = enabled;
        }
    }

    public void InputSetEnabled(bool enabled)
    {
        InputSetEnabled(Enum.GetValues<Button>(), enabled);
    }

    public bool InputGetEnabled(Button button)
    {
        bool output;
        ButtonsEnabled.TryGetValue(button, out output);
        return output;
    }
}
