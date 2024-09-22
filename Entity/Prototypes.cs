using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public static partial class Prototypes
{
    public static Dictionary<EJob, Job> Jobs = new(){
        {EJob.CIVILIAN , Job.CreatePrototype(EJob.CIVILIAN)},
        {EJob.WARRIOR , Job.CreatePrototype(EJob.WARRIOR)},
        {EJob.WIZARD , Job.CreatePrototype(EJob.WIZARD)},
    };

    public static Dictionary<EAction, Action> Actions = new(){
        {EAction.PUNCH , Action.Create(EAction.PUNCH)},
        {EAction.HEAL , Action.Create(identity_enum: EAction.HEAL)},
    };

    static Prototypes()
    {
        foreach (var item in Enum.GetValues<EJob>())
        {
            Jobs[item] = Job.CreatePrototype(item);
        }
        
        foreach (var item in Enum.GetValues<EAction>())
        {
            Actions[item] = Action.Create(item);
        }
    }


}
