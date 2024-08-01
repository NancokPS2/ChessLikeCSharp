using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public partial class Camera : Camera3D
{
    public enum Mode
    {
        FREE
    }

    const float SENSITIVITY_MOD = 0.03f;    
    const string CAMERA_ENABLED_INPUT = "ui_accept";

    bool camera_move_enabled;

    public float sensitivity_horizontal = 1.0f;
    public float sensitivity_vertical = 1.0f;
    public float sensitivity_movement = 3.0f;
    public Mode mode = Mode.FREE;

    public Camera(Mode mode = Mode.FREE)
    {
        this.mode = mode;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        //Enable or disable the camera movement.
        if(@event.IsAction(CAMERA_ENABLED_INPUT)){camera_move_enabled = @event.IsPressed(); return;}

        switch (mode)
        {
            case Mode.FREE:
                FreeCameraRotation(@event);
                break;
            
            default:
            throw new ArgumentException("Unknown Mode");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        //The camera button must be pressed.
        if (!camera_move_enabled){return;}

        switch (mode)
        {
            case Mode.FREE:
                FreeCameraTranslation(delta);
                break;
            
            default:
            throw new ArgumentException("Unknown Mode");
        }



    }

    public void FreeCameraTranslation(double delta)
    {
        //Movement
        Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 translation = new(input.X * (float)delta * sensitivity_movement, 0, input.Y * (float)delta * sensitivity_movement);
        Translate(translation);
    }

    public void FreeCameraRotation(InputEvent @event)
    {
        //The camera button must be pressed.
        if (!camera_move_enabled){return;}

        //Rotation
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            float up_down = -eventMouseMotion.Relative.Y * sensitivity_horizontal * SENSITIVITY_MOD;
            float left_right = -eventMouseMotion.Relative.X * sensitivity_vertical * SENSITIVITY_MOD;

            //Transform3D new_trans = Transform;
            //new_trans.Basis = new_trans.Basis.Rotated(Vector3.Up, left_right);
            //Vector3 vert_axis = Basis.Identity.Rotated(Vector3.Up, Mathf.Pi/2).X;
            //new_trans.Basis = new_trans.Basis.Rotated(vert_axis, up_down);

            //Transform = new_trans;

            RotateY(left_right);
            Orthonormalize();

            //RotateObjectLocal(Vector3.Right, up_down);
            //RotateObjectLocal(Vector3.Up, left_right);

            //Limit the X rotation.
            float x_capped = Mathf.Clamp(Rotation.X + up_down, -Mathf.Pi*0.45f, Mathf.Pi*0.45f);
            Rotation = new(x_capped, Rotation.Y, Rotation.Z);

            //RotateX(eventMouseMotion.Relative.Y * sensitivity_horizontal * SENSITIVITY_MOD);
            //RotateY(-eventMouseMotion.Relative.X * sensitivity_vertical * SENSITIVITY_MOD);
            
        }
        
    }

    public void EnableMovement(bool enable)
    {
        SetProcessUnhandledInput(enable);
        SetProcess(enable);
    }

    public void Watch()
    {
        Current = true;
    }
}
