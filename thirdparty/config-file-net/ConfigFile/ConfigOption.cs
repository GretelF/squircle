using System;
using System.Collections.Generic;
using System.Globalization;
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

        public override string ToString()
        {
            return Value;
        }

        #region Static Create Functions

        public static ConfigOption Create()
        {
            return new ConfigOption();
        }

        public static ConfigOption Create(byte value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(char value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(short value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(int value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(long value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(float value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(double value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }
        public static ConfigOption Create(string value)
        {
            var instance = new ConfigOption();
            instance = value;
            return instance;
        }

        #endregion

        #region Implicit conversion operators to ConfigOption

        public static implicit operator ConfigOption(byte value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(char value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(short value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(int value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(long value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(float value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(double value)
        {
            var option = new ConfigOption();
            option.Value = value.ToString(CultureInfo.InvariantCulture);
            return option;
        }
        public static implicit operator ConfigOption(string value)
        {
            var option = new ConfigOption();
            option.Value = value;
            return option;
        }

        #endregion

        #region Implicit conversion operators to other types

        public static implicit operator byte(ConfigOption option)
        {
            return byte.Parse(option.Value, CultureInfo.InvariantCulture);
        }
        public static implicit operator char(ConfigOption option)
        {
            return char.Parse(option.Value);
        }
        public static implicit operator short(ConfigOption option)
        {
            return short.Parse(option.Value, CultureInfo.InvariantCulture);
        }
        public static implicit operator int(ConfigOption option)
        {
            return int.Parse(option.Value, CultureInfo.InvariantCulture);
        }
        public static implicit operator long(ConfigOption option)
        {
            return long.Parse(option.Value, CultureInfo.InvariantCulture);
        }
        public static implicit operator float(ConfigOption option)
        {
            return float.Parse(option.Value, CultureInfo.InvariantCulture);
        }
        public static implicit operator double(ConfigOption option)
        {
            return double.Parse(option.Value, CultureInfo.InvariantCulture);
        }
        public static implicit operator string(ConfigOption option)
        {
            return option.Value;
        }

        #endregion
    }
}
