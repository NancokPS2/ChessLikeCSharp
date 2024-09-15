using System.ComponentModel;
using System.Drawing.Text;
using ChessLike;

namespace Godot.Display;

[GlobalClass]
public partial class Camera : Camera3D
{
    public enum Mode
    {
        FREE,
        PIVOT,
        DELEGATED_PIVOT,//Similar to pivot, but the controls from the camera are limited as it relies on other systems to move it.
    }

    const float SENSITIVITY_MOD = 0.03f;    

    bool camera_move_held;
    bool camera_move_toggled = true;
    Vector3 directional_input = new();
    Vector2 relative_input = new();
    float misc_input;


    [ExportCategory("Sensitivity")]
    [Export]
    public float sensitivity_horizontal = 6.0f;
    [Export]
    public float sensitivity_vertical = 6.0f;
    [Export]
    public float sensitivity_directional = 1.0f;

    [ExportCategory("Pivot")]
    //Pivot camera movement
    [Export]
    public Vector3 pivot_point = Vector3.Zero;
    [Export]
    public float pivot_rotation = 0;
    [Export]
    public float pivot_distance = 15;
    [Export]
    public float pivot_elevation = 8;
    [Export]
    public float pivot_translation_snap = 1;
    [Export]
    public float pivot_translation_auto_delay = 0.3f;
    [Export]
    public bool pivot_rotation_only = true;

    [ExportCategory("Mode")]
    [Export]
    public Mode mode = Mode.DELEGATED_PIVOT;

    public Camera()
    {
    }

    public override void _Input(InputEvent @event)
    {
        base._UnhandledInput(@event);

        //Enable or disable the camera movement (toggle).
        if(Global.GInput.IsButtonPressed(Global.GInput.Button.SHOULDER_SUB_RT))
        {
            camera_move_toggled = !camera_move_toggled; 
            GD.Print(camera_move_toggled);
            return;
        }

        //Enable or disable the camera movement (hold).
        if(Global.GInput.IsButtonPressed(Global.GInput.Button.SHOULDER_SUB_LT))
        {
            camera_move_held = true; 
            return;
        }

        //Try to evaluate it as a relative action. Stop here if valid.
        HandleRelativeInput(@event);
    }

    public override void _Process(double delta)
    {
        //Try to evaluate it as a directional action. Stop here if valid.
        base._Process(delta);

        
        //The camera must be enabled.
        if (!(camera_move_held || camera_move_toggled)){return;}

        HandleDirectionalInput();

        switch (mode)
        {
            case Mode.FREE:
                FreeCameraUpdate(delta);
                break;

            case Mode.PIVOT:
                PivotCameraUpdate(delta);
                break;
            
            case Mode.DELEGATED_PIVOT:
                HandleMiscInput();  
                DelegatedPivotCameraUpdate(delta);
                break;
            
            default:
                throw new ArgumentException("Unknown Mode");
        }

        //Reset the accumulated input after the frame ends.
        ResetInputs();

    }

    public void FreeCameraUpdate(double delta)
    {
        //Relative
        RotateY(relative_input.X * (float)delta);
        Orthonormalize();
        float x_capped = Mathf.Clamp(Rotation.X + (relative_input.Y * (float)delta), -Mathf.Pi*0.45f, Mathf.Pi*0.45f);
        Rotation = new(x_capped, Rotation.Y, Rotation.Z);

        //Directional
        Vector3 translation = directional_input * (float)delta;
        Translate(translation);
    }
    
    public void PivotCameraUpdate(double delta)
    {
        //Relative
        pivot_rotation += relative_input.X * (float)delta;
        pivot_distance += relative_input.Y * (float)delta;

        if (!pivot_rotation_only)
        {
            //Directional
            pivot_point += directional_input.Rotated(Vector3.Up, pivot_rotation * (float)delta);
        }

        //Set displacement from the pivot based on parameters.

        //Step on top of the pivot point
        GlobalPosition = pivot_point;

        //Move away.
        GlobalPosition += (Vector3.Forward * pivot_distance).Rotated(Vector3.Up, pivot_rotation);

        //Move up
        GlobalPosition += Vector3.Up * pivot_elevation;

        LookAt(pivot_point);

    }

    float rotation_target;
    public void DelegatedPivotCameraUpdate(double delta)
    {
        rotation_target += (MathF.PI / 4.0f) * misc_input;

        //Relative
        pivot_rotation = (float)Mathf.MoveToward(pivot_rotation, rotation_target, sensitivity_horizontal * delta);
        //pivot_rotation = (float)Mathf.Lerp(pivot_rotation, rotation_target, delta);


        //Step on top of the pivot point
        GlobalPosition = pivot_point;

        //Move away.
        GlobalPosition += (Vector3.Forward * pivot_distance).Rotated(Vector3.Up, pivot_rotation);

        //Move up
        GlobalPosition += Vector3.Up * pivot_elevation;

        LookAt(pivot_point);

    }
    
    /// <summary>
    /// Tries to accumulate the input to the relative movement.
    /// </summary>
    /// <param name="event"></param>
    /// <returns>If the input was of a valid type.</returns>
    public bool HandleRelativeInput(InputEvent @event)
    {
        if(@event is InputEventMouseMotion inputEventMouseMotion)
        {
            Vector2 relative = new(
                -inputEventMouseMotion.Relative.X * sensitivity_horizontal * SENSITIVITY_MOD,
                -inputEventMouseMotion.Relative.Y * sensitivity_vertical * SENSITIVITY_MOD
            );

            relative_input += relative;

            return true;
        }
        return true;
    }

    /// <summary>
    /// Tries to accumulate the input to the directional movement.
    /// </summary>
    /// <param name="event"></param>
    /// <returns>If the input was of a valid type.</returns>
    public bool HandleDirectionalInput()
    {
        directional_input += 
        Global.GInput.IsButtonPressed(Global.GInput.Button.MOVE_FW) ? 
        Vector3.Forward * sensitivity_directional : Vector3.Zero;

        directional_input += 
        Global.GInput.IsButtonPressed(Global.GInput.Button.MOVE_BW) ? 
        Vector3.Back * sensitivity_directional : Vector3.Zero;

        directional_input += 
        Global.GInput.IsButtonPressed(Global.GInput.Button.MOVE_LT) ? 
        Vector3.Left * sensitivity_directional : Vector3.Zero;

        directional_input += 
        Global.GInput.IsButtonPressed(Global.GInput.Button.MOVE_RT) ? 
        Vector3.Right * sensitivity_directional : Vector3.Zero;

        directional_input += 
        Global.GInput.IsButtonPressed(Global.GInput.Button.MOVE_UP) ? 
        Vector3.Up * sensitivity_directional : Vector3.Zero;

        directional_input += 
        Global.GInput.IsButtonPressed(Global.GInput.Button.MOVE_DN) ? 
        Vector3.Down * sensitivity_directional : Vector3.Zero;

        return true;
    }

    public void HandleMiscInput()
    {
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.SHOULDER_LT))
        {
            misc_input += Global.GInput.GetActionStrength(Global.GInput.Button.SHOULDER_LT);
        }
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.SHOULDER_RT))
        {
            misc_input -= Global.GInput.GetActionStrength(Global.GInput.Button.SHOULDER_RT);
        }
    }

    public void ResetInputs()
    {
        relative_input = new();
        directional_input = new();
        misc_input = 0;
    }

    public float GetRotationSnapped(float snap = Mathf.Pi / 2)
    {
        float output = Mathf.Snapped((float)Rotation.Y, snap);
        return output;
    }

    public Vector3 GetFacingCardinal()
    {
        foreach (Vector3 displacement in new[]{GlobalPosition + Vector3.Forward, GlobalPosition + Vector3.Back, GlobalPosition + Vector3.Left, GlobalPosition + Vector3.Right})
        {
            Vector2 look_target = 
                new(GlobalPosition.X + displacement.X, GlobalPosition.Z + displacement.Z);

            Vector2 current_look = new(GlobalBasis.Z.X,GlobalBasis.Z.Z);

            if (look_target.Dot(current_look) > 0.5 )
            {
                return displacement;
            }

            
        }
        throw new Exception("It should be impossible to land outside any of these values.");
    }

    public void SetControl(bool enable)
    {
        SetProcessUnhandledInput(enable);
        SetProcess(enable);
    }

    public void Watch()
    {
        Current = true;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
