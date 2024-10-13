using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Extension;

public static class CanvasItemExtension
{


    private static Tween ReplaceMetaTween(this CanvasItem @this, string meta_key)
    {
        //Kill existing tween (if any)
        DeleteMetaTween(@this, meta_key);

        //Create a new one.
        Tween tween = @this.CreateTween();
        @this.SetMeta(meta_key, tween);

        return tween;
    }

    private static void DeleteMetaTween(this CanvasItem @this, string meta_key)
    {
        Tween meta = (Tween)@this.GetMeta(meta_key, new());
        if (meta is Tween tween && GodotObject.IsInstanceValid(tween))
        {
            tween.Kill();
        }
        @this.RemoveMeta(meta_key);
    }

    public static void AnimateIntermitentGlowStop(this CanvasItem @this) => AnimateIntermitentGlow(@this, 0, Godot.Colors.White);

    public static void AnimateIntermitentGlow(this CanvasItem @this, float duration, Godot.Color target_color)
    {
        const string TWEEN_META_KEY = "META_AnimateIntermitentGlow_CanvasItemExtension";

        Tween tween;
        if (duration > 0)
        {
            tween = @this.ReplaceMetaTween(TWEEN_META_KEY);
        }
        //If duration 0, delete it.
        else
        {
            @this.DeleteMetaTween(TWEEN_META_KEY);
            @this.Modulate = Godot.Colors.White;
            return;
        }


        tween.TweenProperty(@this, "modulate", target_color, duration / 2);
        tween.TweenProperty(@this, "modulate", Godot.Colors.White, duration / 2);
        tween.SetLoops();
    }

    public static void AnimateFadeAway(this CanvasItem @this, float duration, bool kill_at_end)
    {
        const string TWEEN_META_KEY = "META_AnimateIntermitentGlow_CanvasItemExtension";

        Tween tween = @this.ReplaceMetaTween(TWEEN_META_KEY);

        tween.TweenProperty(@this, "modulate", Godot.Colors.Transparent, duration);
        if (kill_at_end)
        {
            tween.Finished += @this.QueueFree;
        } 
            
    }
    
}
