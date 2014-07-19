using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class ConfigSection
    {
        public string Comment { get; set; }

        public IDictionary<string, ConfigOption> Options { get; set; }

        public ConfigSection()
        {
            Comment = string.Empty;
            Options = new Dictionary<string, ConfigOption>();
        }

        public ConfigOption this[string optionName]
        {
            get
            {
                return Options[optionName];
            }

            set
            {
                if (Options.ContainsKey(optionName))
                {
                    Options[optionName] = value;
                }
                else
                {
                    Options.Add(optionName, value);
                }
            }
        }
    }
}
