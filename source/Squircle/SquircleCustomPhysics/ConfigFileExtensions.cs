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
        public static void IfSectionExists(this ConfigFile cfg,
                                           string sectionName,
                                           Action<ConfigSection> thenAction,
                                           Action elseAction = null)
        {
            ConfigSection section;
            if (cfg.Sections.TryGetValue(sectionName, out section))
            {
                thenAction(section);
            }
            else if (elseAction != null)
            {
                elseAction();
            }
        }

        public static void IfOptionExists(this ConfigSection section,
                                          string optionName,
                                          Action<ConfigOption> thenAction,
                                          Action elseAction = null)
        {
            ConfigOption option;
            if (section.Options.TryGetValue(optionName, out option))
            {
                thenAction(option);
            }
            else if (elseAction != null)
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

        public static ConfigFile AsConfigFile(this ConfigOption opt)
        {
            return ConfigFile.FromFile(opt);
        }
    }
}
