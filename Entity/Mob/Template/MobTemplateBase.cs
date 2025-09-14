using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobTemplateBase : MobTemplate
{
	public static readonly MobTemplateBase DEFAULT = GD.Load<MobTemplateBase>("uid://7btkf10h82tr");
	
    [Export]
    protected MobStatSet MobStats = MobStatSet.GetDefault();

    public MobTemplateBase() : base(ETemplateType.BASE)
    {

    }

    public override Mob ApplyTemplate(Mob mob)
    {
        ApplyBaseStats(mob, MobStats);
        return mob;
    }
}
