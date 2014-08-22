﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration;
using Microsoft.Xna.Framework;

namespace Squircle
{
    public static class ConfigFileExtensions
    {
        public static DVector2 AsDVector2(this ConfigOption cfg)
        {
            return DVector2.Parse(cfg);
        }

        public static Vector2 AsVector2(this ConfigOption cfg)
        {
            return (Vector2)cfg.AsDVector2();
        }

    }
}
