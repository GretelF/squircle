using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration;
using Microsoft.Xna.Framework;

namespace Squircle
{
    public static class ConfigFileExtensions
    {
        public static void IfOptionExists(this ConfigSection section, string optionName, Action<ConfigOption> action)
        {
            ConfigOption option;
            if (section.Options.TryGetValue(optionName, out option))
            {
                action(option);
            }
        }

        public static void IfOptionExists(this ConfigSection section,
                                          string optionName,
                                          Action<ConfigOption> thenAction,
                                          Action elseAction)
        {
            ConfigOption option;
            if (section.Options.TryGetValue(optionName, out option))
            {
                thenAction(option);
            }
            else
            {
                elseAction();
            }
        }

        public static DVector2 AsDVector2(this ConfigOption cfg)
        {
            return DVector2.Parse(cfg);
        }

        public static Vector2 AsVector2(this ConfigOption cfg)
        {
            return (Vector2)cfg.AsDVector2();
        }

        public static DRectangle AsDRectangle(this ConfigOption cfg)
        {
            return DRectangle.Parse(cfg);
        }

        public static Rectangle AsRectangle(this ConfigOption cfg)
        {
            return (Rectangle)cfg.AsDRectangle();
        }

        public static bool AsBool(this ConfigOption cfg)
        {
            return bool.Parse(cfg);
        }
    }
}
