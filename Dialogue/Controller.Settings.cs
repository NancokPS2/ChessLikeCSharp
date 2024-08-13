using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Dialogue;

public partial class Controller
{
    public struct Settings
    {
        public float SpeedMultiplier;

        public Settings()
        {
            SpeedMultiplier = 1.0f;
        }

        
    }


}
