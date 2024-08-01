namespace Godot.Display;

public partial class Camera : Camera3D
{
    public enum Mode
    {
        FREE,
        PIVOT,
    }
    public enum InputType
    {
        FORWARD, BACK, UP, DOWN, LEFT, RIGHT, ENABLE_HOLD, ENABLE_TOGGLE

    }
    Dictionary<InputType, string> InputTypeDict = new(){
        {InputType.FORWARD,"forward"},
        {InputType.BACK,"backward"},
        {InputType.LEFT,"left"},
        {InputType.RIGHT,"right"},
        {InputType.UP,"quick_action_a"},
        {InputType.DOWN,"quick_action_b"},
        {InputType.ENABLE_HOLD,"modifier_a"},
        {InputType.ENABLE_TOGGLE,"ui_cancel"},
    };

    const float SENSITIVITY_MOD = 0.03f;    

    bool camera_move_held;
    bool camera_move_toggled;
    Vector3 directional_input = new();
    Vector2 relative_input = new();

    public float sensitivity_horizontal = 6.0f;
    public float sensitivity_vertical = 6.0f;
    public float sensitivity_directional = 6.0f;

    //Pivot camera movement
    public Vector3 pivot_point = Vector3.Zero;
    public float pivot_rotation = 0;
    public float pivot_distance = 10;
    public float pivot_elevation = 8;
    public float pivot_translation_snap = 1;
    public float pivot_translation_auto_delay = 0.3f;

    public Mode mode = Mode.FREE;

    public Camera(Mode mode = Mode.FREE)
    {
        this.mode = mode;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        //Enable or disable the camera movement (toggle).
        if(@event.IsActionPressed(GetActionName(InputType.ENABLE_TOGGLE)))
        {
            camera_move_toggled = !camera_move_toggled; 
            GD.Print(camera_move_toggled);
            return;
        }

        //Enable or disable the camera movement (hold).
        if(@event.IsAction(GetActionName(InputType.ENABLE_HOLD)))
        {
            camera_move_held = @event.IsPressed(); 
            return;
        }

        //Try to evaluate it as a relative action. Stop here if valid.
        if(HandleRelativeInput(@event))
        {
            return;
        }

        //Try to evaluate it as a directional action. Stop here if valid.
        if(HandleDirectionalInput(@event))
        {
            return;
        }

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        //The camera must be enabled.
        if (!(camera_move_held || camera_move_toggled)){return;}

        switch (mode)
        {
            case Mode.FREE:
                FreeCameraUpdate(delta);
                break;

            case Mode.PIVOT:
                PivotCameraUpdate(delta);
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
        Vector3 translation = directional_input * (float)delta * sensitivity_directional;
        Translate(translation);
    }
    
    public void PivotCameraUpdate(double delta)
    {
        //Relative
        pivot_rotation += relative_input.X * (float)delta;
        pivot_distance += relative_input.Y * (float)delta;

        //Directional
        pivot_point += directional_input.Rotated(Vector3.Up, pivot_rotation * (float)delta);

        //Set displacement from the pivot based on parameters.
        //Move away
        Vector3 forward_displacement = Vector3.Forward * pivot_distance;
        //Raise
        Vector3 upward_displacement = Vector3.Up * pivot_elevation;
        //Rotate
        forward_displacement = forward_displacement.Rotated(Vector3.Up, pivot_rotation);

        //Update position with the displacements.
        GlobalPosition = pivot_point + forward_displacement + upward_displacement;
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
    public bool HandleDirectionalInput(InputEvent @event)
    {
        directional_input += 
        @event.IsActionPressed(GetActionName(InputType.FORWARD)) ? 
        Vector3.Forward * sensitivity_directional : Vector3.Zero;

        directional_input += 
        @event.IsActionPressed(GetActionName(InputType.BACK)) ? 
        Vector3.Back * sensitivity_directional : Vector3.Zero;

        directional_input += 
        @event.IsActionPressed(GetActionName(InputType.LEFT)) ? 
        Vector3.Left * sensitivity_directional : Vector3.Zero;

        directional_input += 
        @event.IsActionPressed(GetActionName(InputType.RIGHT)) ? 
        Vector3.Right * sensitivity_directional : Vector3.Zero;

        directional_input += 
        @event.IsActionPressed(GetActionName(InputType.UP)) ? 
        Vector3.Up * sensitivity_directional : Vector3.Zero;

        directional_input += 
        @event.IsActionPressed(GetActionName(InputType.DOWN)) ? 
        Vector3.Down * sensitivity_directional : Vector3.Zero;
        return true;


    }

    public void ResetInputs()
    {
        relative_input = new();
        directional_input = new();
    }



    public void EnableMovement(bool enable)
    {
        SetProcessUnhandledInput(enable);
        SetProcess(enable);
    }

    public void Watch()
    {
        Current = true;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public string GetActionName(InputType input)
    {
        return InputTypeDict[input];
    }
}
