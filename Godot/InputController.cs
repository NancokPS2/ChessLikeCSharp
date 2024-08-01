using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public class InputController
{
    public enum InputType {
        UP, DOWN, LEFT, RIGHT, //Main directions
        MAIN_A, MAIN_B, MAIN_C, MAIN_D, //"Face buttons"
        QUICK_A, QUICK_B, //"Bumpers"
    }
}
