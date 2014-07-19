using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Configuration
{
    public class ConfigFile
    {
        static public string GlobalSectionName { get; set; }

        static ConfigFile()
        {
            GlobalSectionName = "<GLOBAL>";
        }

        public IDictionary<string, ConfigSection> Sections { get; set; }

        public ConfigSection GlobalSection { get; set; }

        /// <summary>
        /// This actually directly delegates to the global section's comment.
        /// The ConfigFile itself does not have a comment.
        /// </summary>
        public string Comment
        {
            get
            {
                return GlobalSection.Comment;
            }
            set
            {
                GlobalSection.Comment = value;
            }
        }

        static public ConfigFile FromFile(string fileName)
        {
            var content = string.Empty;
            using (var fileStream = File.OpenText(fileName))
            {
                content = fileStream.ReadToEnd();
            }
            return FromString(content);
        }

        static public ConfigFile FromString(string content)
        {
            var cfg = new ConfigFile();
            var parser = new ConfigFileParser(cfg);
            parser.Parse(content);
            return cfg;
        }

        public ConfigFile()
        {
            Sections = new Dictionary<string, ConfigSection>();
            GlobalSection = new ConfigSection();
            Sections.Add(GlobalSectionName, GlobalSection);
        }

        public ConfigSection this[string sectionName]
        {
            get
            {
                return Sections[sectionName];
            }

            set
            {
                if (Sections.ContainsKey(sectionName))
                {
                    Sections[sectionName] = value;
                }
                else
                {
                    Sections.Add(sectionName, value);
                }
            }
        }

        /// <summary>
        /// Serializes all sections and options to a single continuous string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        public string Serialize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the serialized content of this instance to the specified file.
        /// 
        /// <remarks>Will create the file and directories if they don't exist yet.</remarks>
        /// </summary>
        /// <param name="fileName">The name of the file to save to. May be relative or absolute.</param>
        public void Save(string fileName)
        {
            var content = Serialize();

            var dir = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(dir);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fileStream = File.CreateText(fileName))
            {
                fileStream.Write(content);
            }
        }

    }
}
