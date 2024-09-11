global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.IO;
global using System.Numerics;
global using System.Drawing;
global using ChessLike.Shared;

public static partial class Global
{
    public static partial class Directory
    {
        const string GameName = "RPGTactics";
        static string UserContent = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName + @"\content");
        static string GameContent = new(AppDomain.CurrentDomain.BaseDirectory + @"\" + GameName);    
        static string User = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName);

        public static string GetContentDir(EDirectory dir)
        {
            switch(dir)
            {
                case EDirectory.USER_CONTENT:
                    return UserContent;
                
                case EDirectory.GAME_CONTENT:
                    return GameContent;

                default:
                    throw new ArgumentException();
            }
        }
    }

}



