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
        public enum Content
        {
            USER_CONTENT,
            GAME_CONTENT,
        }
        const string GameName = "RPGTactics";
        static string UserContent = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName + @"\content");
        static string GameContent = new(AppDomain.CurrentDomain.BaseDirectory + @"\" + GameName);    
        static string User = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName);

        public static string GetContentDir(Content dir)
        {
            switch(dir)
            {
                case Content.USER_CONTENT:
                    return UserContent;
                
                case Content.GAME_CONTENT:
                    return GameContent;

                default:
                    throw new ArgumentException();
            }
        }
    }

}



