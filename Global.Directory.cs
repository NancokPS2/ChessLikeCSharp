global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.IO;
global using System.Numerics;
global using System.Drawing;
global using ChessLike.Shared;

namespace ChessLike;

public static partial class Global
{
    public static class Directory
    {
        const string GameName = "RPGTactics";

        public static string UserContent = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName + @"\content");
        public static string User = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName + @"\");
        public static string GameContent = new(AppDomain.CurrentDomain.BaseDirectory + @"\" + GameName + @"\");    

    }

}



