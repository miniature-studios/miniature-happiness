/*Copyright (C) 2012-2013 Alexander Forselius <drsounds@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, A
RISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*/

//https://github.com/krikelin/CSS.NET

using System.Collections.Generic;
using System.Text;

namespace DA_Assets.Shared.CssNet
{
    public class CssStylesheet
    {
        public List<CssSelector> selectors = new List<CssSelector>();

        public CssStylesheet(string stylesheet)
        {
            char currentChar;

            StringBuilder buffer = new StringBuilder();

            for (int i = 0, j = 0; i < stylesheet.Length; i++, j++)
            {
                currentChar = stylesheet[i];

                switch (currentChar)
                {
                    case ' ':
                        continue;
                    case '{':
                        {
                            int endIndex = stylesheet.IndexOf('}', i);
                            string block = stylesheet.Substring(i, endIndex - i);
                            CssSelector selector = new CssSelector(buffer.ToString().Trim(), block);
                            this.selectors.Add(selector);
                            i = endIndex - 1;
                            buffer.Clear();
                            continue;
                        }
                    default:
                        buffer.Append(currentChar);
                        break;
                }
            }
        }
    }

    public class CssSelector
    {
        public List<CssRule> rules = new List<CssRule>();

        public string Name { get; set; }

        public CssSelector(string name, string block)
        {
            this.Name = name;
            string[] rules = block.Split(';');

            if (rules.Length < 1)
            {
                CssRule rule = new CssRule(block + ";");
                this.rules.Add(rule);
            }

            foreach (string _rule in rules)
            {
                CssRule rule = new CssRule(_rule + ";");
                this.rules.Add(rule);
            }
        }
    }

    public class CssRule
    {
        public string originalRule;
        public string rule;
        public string value;

        public object objectValue => this.value;

        public enum ParserMode
        {
            property,
            value
        };

        public CssRule(string input)
        {
            StringBuilder propBuffer = new StringBuilder();
            StringBuilder valueBuffer = new StringBuilder();
            ParserMode mode = ParserMode.property;
            bool inString = false;
            char currentChar;

            for (var i = 0; i < input.Length; i++)
            {
                currentChar = input[i];

                switch (currentChar)
                {
                    case '{':
                        if (!inString)
                            continue;
                        break;
                    case '"':
                    case '\'':
                        inString = !inString;
                        break;
                    case ':':
                        if (!inString)
                            mode = ParserMode.value;
                        break;
                    case '}':
                    case ';':
                        if (!inString)
                            break;
                        i = 1000;
                        break;
                    default:
                        switch (mode)
                        {
                            case ParserMode.property:
                                propBuffer.Append(currentChar);
                                break;
                            case ParserMode.value:
                                valueBuffer.Append(currentChar);
                                break;
                        }
                        break;
                }
            }

            this.rule = propBuffer.ToString().Trim();
            this.value = valueBuffer.ToString().Trim();
        }
    }
}