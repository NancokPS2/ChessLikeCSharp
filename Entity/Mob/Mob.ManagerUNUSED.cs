using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Mob
{
    public class ManagerUNUSED : SerializableManager<Mob>
    {


        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "mob"
            );

        }

    }

}
