using System;
using System.Collections.Generic;
using static System.Console;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace t1FormaisGUI
{
    public class RegularExpression : RegularLanguage
    {
        private HashSet<Char> vt;
        private string regex;
        private string formattedRegex;
        private bool isEmpty = false;

        public RegularExpression(string inp) : base(inp, inputType.RE)
        {
            vt = new HashSet<char>();
            regex = inp;
            formattedRegex = "";
        }

        public override string getDefinition()
        {
            return input;
        }

        public bool IsEmpty ()
        {
            return isEmpty();
        }

        public string getRegex()
        {
            return regex;
        }

        public string getFormattedRegex()
        {
            return formattedRegex;
        }

        public override RegularGrammar getRG()
        {
//            return this.getFA().getRG();
            return null;
        }

//        public FiniteAutomara getFA()
//        {
//            return null;
//        }

        public override RegularExpression getRE()
        {
            return this;
        }
        
        public string getExplicitConcatenation()
        {
            string concatenation = "";

            char next = new char();
            char c;
            for (int i = 0; i < formattedRegex.Length; i++)
            {
                c = formattedRegex[i];
                concatenation += c;
                if (c == ')')
                {
                    if (i < formattedRegex.Length - 1)
                    {
                        next = formattedRegex[i+1];
                        if (char.IsLetterOrDigit(next) || next == '(' || c == '&')
                        {
                            concatenation += '.';
                        }
                    }
                } else if (c == '*' || c == '?' || c == '*')
                {
                    if (i < formattedRegex.Length -1)
                    {
                        next = formattedRegex[i + 1];
                    }

                    if (next == '(' || char.IsLetterOrDigit(next) || next == '&')
                    {
                        concatenation += '.';
                    }
                }

                if (char.IsLetterOrDigit(c) || c == '&')
                {
                    if (i < formattedRegex.Length - 1)
                    {
                        next = formattedRegex[i + 1];
                        if (char.IsLetterOrDigit(next) || next == '&' || next == '(')
                        {
                            concatenation += '.';
                        }
                    }
                }
            }

            return concatenation;
        }

        public static RegularExpression isValidRE(string input)
        {
            RegularExpression re = new RegularExpression(input);
            string formatted = input.Replace("[\\s]", "");
            if (formatted.Replace("[\\(\\)\\+\\?\\*\\|]+", "").Equals(""))
            {
                re.isEmpty = true;
            }

            if (!lexicalValidation(formatted, re))
            {
                return null;
            }

            if (!syntaticAnalysis(formatted, re))
            {
                return null;
            }

            return re;
        }

        public static bool lexicalValidation(string input, RegularExpression re)
        {
            if (Regex.Matches("^[a-z0-9\\(\\)\\?\\*|&\\+]*", input).Count == 0)
            {
                return false;
            }

            return true;
        }

        public static bool syntaticAnalysis(string input, RegularExpression re)
        {
            Stack<char> symbols = new Stack<char>();
            char c, before, next;
            if (!(input.Count() == 0))
            {
                c = input[0];
                if (c == '*' || c == '|' || c == '?' || c == '+')
                {
                    return false;
                }
            }

            string formatted = input.Replace("\\?+", "?");
            formatted = formatted.Replace("\\*+", "*");
            formatted = formatted.Replace("\\*+", "+");
            formatted = formatted.Replace("\\?\\*+", "*");
            formatted = formatted.Replace("\\?\\+", "*");
            formatted = formatted.Replace("\\*\\?", "*");
            formatted = formatted.Replace("\\*+\\?", "*");
            formatted = formatted.Replace("\\+\\*+", "*");
            formatted = formatted.Replace("\\*+\\+", "*");
            formatted = formatted.Replace("\\*+", "*");

            re.formattedRegex = formatted;

            for (int i = 0; i < formatted.Length; i++)
            {
                c = formatted[i];

                if (char.IsLetterOrDigit(c) || c == '&')
                {
                    re.vt.Add(c);
                }

                if (c == '(')
                {
                    if (i+1 < formatted.Length)
                    {
                        next = formatted[i + 1];
                        if (!char.IsLetterOrDigit(next) && next != ')' && next != '(' && next != '&')
                        {
                            return false;
                        }
                    }
                    symbols.Push(c);
                }else if (c == ')')
                {
                    if (symbols.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (symbols.Peek() == '(')
                        {
                            symbols.Pop();
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (i == 0)
                {
                    continue;
                }

                before = formatted[i - 1];
                if (c == '*' || c == '?' || c == '+')
                {
                    if (!char.IsLetterOrDigit(before) && before != ')' && before != '&')
                    {
                        return false;
                    }
                } else if (c == '|')
                {
                    if (i+1 < formatted.Length)
                    {
                        next = formatted[i + 1];
                        if (!char.IsLetterOrDigit(next) && (next != '(') && (next != '&'))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return symbols.Count == 0;

        }
        
    }
}