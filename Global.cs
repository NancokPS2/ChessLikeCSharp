global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.IO;
global using System.Numerics;
global using System.Drawing;
global using ChessLike.Shared;

namespace ChessLike;

public static class Global
{
    public static Directory directory = new();
    public static ResourceDictionary resources = new();

    public static Variables variables = new();

    public class Directory
    {
        const string GameName = "RPGTactics";

        public static string UserContent = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName + @"\content");
        public static string User = new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + GameName + @"\");
        public static string GameContent = new(AppDomain.CurrentDomain.BaseDirectory + @"\" + GameName + @"\");    

    }

    public struct Variables
    {
        public string player_profile;

    }
}



