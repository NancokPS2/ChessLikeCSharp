using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public static partial class Prototypes
{
    public static Dictionary<EJob, Job> Jobs = new(){
        {EJob.CIVILIAN , Job.Create(EJob.CIVILIAN)},
        {EJob.WARRIOR , Job.Create(EJob.WARRIOR)},
        {EJob.WIZARD , Job.Create(EJob.WIZARD)},
        {EJob.CIVILIAN , Job.Create(EJob.CIVILIAN)},
    };

    public static Dictionary<EAction, Action> Actions = new(){
        {EAction.PUNCH , Action.Create(EAction.PUNCH)},
        {EAction.HEAL , Action.Create(EAction.HEAL)},
    };

    static Prototypes()
    {
        foreach (var item in Enum.GetValues<EJob>())
        {
            Jobs[item] = Job.Create(item);
        }
        foreach (var item in Enum.GetValues<EAction>())
        {
            Actions[item] = Action.Create(item);
        }
    }


}
