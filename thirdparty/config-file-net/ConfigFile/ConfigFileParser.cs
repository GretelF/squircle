using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public enum StringStreamPosition
    {
        Beginning,
        End,
        Current,
    }

    public class StringStream
    {
        public const char EndOfStreamChar = unchecked((char)-1);

        public string Content { get; set; }

        public string CurrentContent
        {
            get
            {
                return Content.Substring(Index);
            }
        }

        public int Index { get; set; }

        public bool IsAtEndOfStream
        {
            get
            {
                return Index >= Content.Length;
            }
        }

        public char Current
        {
            get
            {
                if (IsAtEndOfStream)
                {
                    return EndOfStreamChar;
                }
                return Peek();
            }
        }

        public StringStream(string content)
        {
            Index = 0;
            Content = content;
        }

        public char Peek()
        {
            if (IsAtEndOfStream)
            {
                return Content[Content.Length - 1];
            }
            return PeekUnchecked();
        }

        public char PeekUnchecked()
        {
            return Content[Index];
        }

        public void Next(int relativeIndex = 1)
        {
            Index += relativeIndex;
        }

        public char Read()
        {
            var result = Peek();
            Next();
            return result;
        }

        public string ReadLine()
        {
            var line = new StringBuilder();

            while (true)
            {
                if (IsAtEndOfStream || IsAtAnyOf(Environment.NewLine))
                {
                    break;
                }

                line.Append(Content[Index++]);
            }
            Skip(Environment.NewLine);

            return line.ToString();
        }

        public void Seek(int index, StringStreamPosition relativePosition)
        {
            switch (relativePosition)
            {
            case StringStreamPosition.Beginning:
                Index = index;
                break;
            case StringStreamPosition.End:
                Index = Content.Length - index - 1;
                break;
            case StringStreamPosition.Current:
                Index += index;
                break;
            }
        }

        public bool IsAt(char c)
        {
            if (IsAtEndOfStream)
            {
                return false;
            }
            return c == PeekUnchecked();
        }

        public bool IsAtAnyOf(string theChars)
        {
            if (IsAtEndOfStream)
            {
                return false;
            }
            return theChars.Contains(PeekUnchecked());
        }

        public void Skip(char charToSkip)
        {
            var charsToSkip = new string(charToSkip, 1);
            Skip(charsToSkip);
        }

        public void Skip(string charsToSkip)
        {
            while (true)
            {
                if (IsAtEndOfStream)
                {
                    return;
                }
                if (!IsAtAnyOf(charsToSkip))
                {
                    return;
                }
                Next();
            }
        }
    }

    public class ConfigFileParser
    {
        static public string WhiteSpace { get; set; }

        static public string WhiteSpaceNewline { get; set; }

        public const char CommentPrefix = '#';
        public const char SectionPrefix = '[';
        public const char SectionSuffix = ']';
        public const char KeyValueSeparator = '=';
        public const char ValueDelimiter = ',';

        static ConfigFileParser()
        {
            WhiteSpace = " \t\v\b";
            WhiteSpaceNewline = WhiteSpace + Environment.NewLine;
        }
        public ConfigFile ConfigFile { get; set; }

        public ConfigFileParser(ConfigFile cfg)
        {
            ConfigFile = cfg;
        }

        public ConfigFileParser()
        {
            ConfigFile = new ConfigFile();
        }

        public void Parse(string serializedConfigFile)
        {
            var stream = new StringStream(serializedConfigFile);
            Parse(stream);
        }

        public void Parse(StringStream stream)
        {
            var cfg = ConfigFile;
            var globalSection = cfg.GlobalSection;
            ParseGlobalSection(ref stream, ref globalSection);

            while (!stream.IsAtEndOfStream)
            {
                stream.Skip(WhiteSpaceNewline);

                string sectionName;
                ConfigSection section;
                ParseSection(ref stream, out sectionName, out section);
                cfg[sectionName] = section;
            }
        }

        #region Internals

        private void ParseOption(ref StringStream stream,
                                 out string out_optionName,
                                 out ConfigOption out_option)
        {
            var splitChars = new char[]{ KeyValueSeparator };
            var line = stream.ReadLine();
            var lineParts = line.Split(splitChars, 2);
            if (lineParts.Length != 2)
            {
                throw new InvalidDataException("The config file contains an invalid option-line.");
            }

            out_optionName = lineParts[0].Trim();
            out_option = ConfigOption.Create(lineParts[1].Trim());
        }

        private void ParseSection(ref StringStream stream,
                                  out string out_sectionName,
                                  out ConfigSection out_section)
        {
            out_section = new ConfigSection();
            ParseSectionHeader(ref stream, out out_sectionName);
            ParseSectionBody(ref stream, ref out_section);
        }

        private void ParseGlobalSection(ref StringStream stream,
                                        ref ConfigSection out_section)
        {
            ParseSectionBody(ref stream, ref out_section);
        }

        private void ParseSectionHeader(ref StringStream stream,
                                        out string ref_sectionName)
        {
            // Skip prefix character
            stream.Read();
            ref_sectionName = string.Empty;

            while (true)
            {
                if (stream.IsAtEndOfStream)
                {
                    break;
                }
                if (stream.PeekUnchecked() == SectionSuffix)
                {
                    // Read the suffix character
                    stream.Read();
                    break;
                }
                ref_sectionName += (char)stream.Read();
            }
        }

        private void ParseSectionBody(ref StringStream stream,
                                      ref ConfigSection ref_section)
        {
            stream.Skip(WhiteSpaceNewline);
            while (true)
            {
                stream.Skip(WhiteSpace);
                if (stream.IsAtEndOfStream)
                {
                    break;
                }

                var currentChar = stream.PeekUnchecked();

                if (currentChar == SectionPrefix)
                {
                    break;
                }

                if (currentChar == CommentPrefix)
                {
                    string comment;
                    ParseComment(ref stream, out comment);
                    if (!string.IsNullOrEmpty(ref_section.Comment))
                    {
                        comment = Environment.NewLine + comment;
                    }
                    ref_section.Comment += comment;
                    continue;
                }

                string optionName;
                ConfigOption option;
                ParseOption(ref stream, out optionName, out option);
                ref_section[optionName] = option;
            }
        }

        private void ParseComment(ref StringStream stream,
                                  out string out_comment)
        {
            // Skip initial prefix character
            stream.Read();

            out_comment = stream.ReadLine();
        }

        #endregion
    }
}
