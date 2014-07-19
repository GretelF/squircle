using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class ConfigOption
    {
        public string Comment { get; set; }

        public string Value { get; set; }
        
        /// <summary>
        /// Use the static named constructors Create([...]) to create an instance.
        /// </summary>
        private ConfigOption()
        {
            Value = string.Empty;
        }

        public void Set<T>(T value)
        {
            Value = value.ToString();
        }

        public override string ToString()
        {
            return Value;
        }

        public static ConfigOption Create()
        {
            return new ConfigOption();
        }

        public static ConfigOption Create<T>(T value)
        {
            var instance = new ConfigOption();
            instance.Set(value);
            return instance;
        }

        #region Implicit conversion operators

        public static implicit operator ConfigOption(byte value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(char value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(short value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(int value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(long value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(float value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(double value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }
        public static implicit operator ConfigOption(string value)
        {
            var option = new ConfigOption();
            option.Set(value);
            return option;
        }

        public static implicit operator byte(ConfigOption option)
        {
            return byte.Parse(option.Value);
        }
        public static implicit operator char(ConfigOption option)
        {
            return char.Parse(option.Value);
        }
        public static implicit operator short(ConfigOption option)
        {
            return short.Parse(option.Value);
        }
        public static implicit operator int(ConfigOption option)
        {
            return int.Parse(option.Value);
        }
        public static implicit operator long(ConfigOption option)
        {
            return long.Parse(option.Value);
        }
        public static implicit operator float(ConfigOption option)
        {
            return float.Parse(option.Value);
        }
        public static implicit operator double(ConfigOption option)
        {
            return double.Parse(option.Value);
        }
        public static implicit operator string(ConfigOption option)
        {
            return option.Value;
        }

        #endregion
    }
}
