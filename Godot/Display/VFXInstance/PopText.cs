using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Godot.Display;

public partial class PopText : Label3D
{
    public enum Animation
    {
        NONE,
        SHAKE_AT_END,
    }

    [Export]
    public Animation AnimationMode = Animation.NONE;
    [Export]
    public Color TextColor = new(1,1,1);
    [Export]
    public Vector3 Direction = Vector3.Up;
    [Export]
    public float Distance = 1;
    [Export]
    public float JumpDuration = 0.3f;
    [Export]
    public float WaitDuration = 1f;
    [Export]
    public float FadeDuration = 0.5f;
    [Export]
    public float AngleVariation = MathF.PI / 10;
    [Export]
    public float DistanceVariation = 0.5f;

    private float fade_alpha;
    private Godot.Font font = new SystemFont();
    public PopText()
    {
    }
    public PopText(string Text)
    {
        this.Text = Text;
    }
    public PopText(string Text, Vector3 Direction, Color TextColor) : this(Text)
    {
        this.Direction = Direction;
        this.TextColor = TextColor;
    }

    public override void _Ready()
    {
        base._Ready();
        Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
        PixelSize = 0.0025f;
        FixedSize = true;
        NoDepthTest = true;
        Modulate = TextColor;
        OutlineModulate = new(0,0,0, 0);

        CreateTweens(AnimationMode);

        GetTree().CreateTimer(JumpDuration+WaitDuration+FadeDuration).Timeout += QueueFree;
    }

    public void CreateTweens(Animation animation)
    {
        RandomNumberGenerator rng = new();
        Tween tween = CreateTween();

        switch (animation)
        {
            case Animation.NONE:
                tween.TweenProperty(
                    this, 
                    new NodePath("global_position"), 
                    GlobalPosition + GetTargetPosition(),
                    JumpDuration
                );
                tween.TweenInterval(WaitDuration);
                tween.TweenProperty(this,
                    new NodePath("modulate"),
                    new Color(TextColor, 0),
                    FadeDuration
                );
                break;
            case Animation.SHAKE_AT_END:
                List<Vector3> shake_offsets = new(){new(), new(), new()};
                for (int i = 0; i < shake_offsets.Count; i++)
                {
                    Vector3 item = shake_offsets[i];
                    item += new Vector3(
                        rng.RandfRange(-DistanceVariation,DistanceVariation), rng.RandfRange(-DistanceVariation,DistanceVariation), rng.RandfRange(-DistanceVariation,DistanceVariation)
                        );   
                    GD.Print(item);
                }
                tween.Chain();
                Vector3 target_pos = GlobalPosition + GetTargetPosition();
                tween.TweenProperty(
                    this, 
                    new NodePath("global_position"), 
                    target_pos,
                    JumpDuration
                );
                tween.TweenProperty(
                    this, 
                    new NodePath("global_position"), 
                    target_pos + shake_offsets[0],
                    WaitDuration / 3
                );
                tween.TweenProperty(
                    this, 
                    new NodePath("global_position"), 
                    target_pos + shake_offsets[1],
                    WaitDuration / 3
                );
                tween.TweenProperty(
                    this, 
                    new NodePath("global_position"), 
                    target_pos + shake_offsets[2],
                    WaitDuration / 3
                );

                tween.TweenProperty(this,
                    new NodePath("modulate"),
                    new Color(TextColor, 0),
                    FadeDuration
                );
                break;

            default:
                break;
        }
        

    }

    private Vector3 GetTargetPosition()
    {
        RandomNumberGenerator rng = new();
        Vector3 output = Direction.Normalized() * (Distance + rng.RandfRange(-DistanceVariation, DistanceVariation));
        output = output.Rotated(Vector3.Right, rng.RandfRange(-AngleVariation, AngleVariation));
        output = output.Rotated(Vector3.Forward, rng.RandfRange(-AngleVariation, AngleVariation));
        return output;
    }

    public PopText SetJumpTime(float time)
    {
        JumpDuration = time;
        return this;
    }
    public PopText SetWaitTime(float time)
    {
        WaitDuration = time;
        return this;
    }
    public PopText SetFadeTime(float time)
    {
        FadeDuration = time;
        return this;
    }
    public PopText SetColor(Color color)
    {
        TextColor = color;
        return this;
    }
    public PopText SetAnimation(Animation animation)
    {
        AnimationMode = animation;
        return this;
    }
}
