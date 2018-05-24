using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace t1FormaisGUI
{
    public class RegularGrammar : RegularLanguage
    {
        private HashSet<char> vn;
        private HashSet<char> vt;
        private Dictionary<char, HashSet<string>> productions; 
        private char s;
        private static TextScanner prodScan;

        public RegularGrammar(string inp) : base (inp, inputType.RG)
        {
            vn = new HashSet<char>();
            vt = new HashSet<char>();
            productions = new Dictionary<char, HashSet<string>>();
        }

        public static RegularGrammar isValidRG(string inp)
        {
            RegularGrammar rg = new RegularGrammar(inp);
            if (!lexicalValidation(inp))
            {
                return null;
            }

            String[] productions = getProductions(inp);

            validateProductions(productions, rg);

            if (rg.vn.Count == 0)
            {
                return null;
            }

            return rg;
        }

        public HashSet<char> getVn()
        {
            return vn;
        }

        public HashSet<char> getVt()
        {
            return vt;
        }

        public char getInitial()
        {
            return s;
        }

        public HashSet<string> getProductions(char vn)
        {
            HashSet<string> prod = productions[vn];
            if (prod == null)
            {
                prod = new HashSet<string>();
            }

            return prod; //unmodifiable set!
        }

        public override string getDefinition()
        {
            string grammar = "";
            string aux = "";
            HashSet<string> value;
            HashSet<string> prodList;

            foreach (char vN in productions.Keys)
            {
                prodList = productions[vN];
                foreach (string prod in prodList)
                { 
                    aux += prod + " | ";
                }

                aux = aux.Substring(0, aux.Length - 2);
                if (vN.Equals(s))
                {
                    grammar = vN + " -> " + aux + "\n" + grammar;
                } else
                {
                    grammar += vN + " -> " + aux + "\n";
                }
                aux = "";
            }
            return grammar;
        }
        
//        public FiniteAutomata getFA()
//        {
//            return null;
//        }

        public override RegularExpression getRE()
        {
            return null;
        }

        public override RegularGrammar getRG()
        {
            return this;
        }
        
        private static string[] getProductions(string str)
        {
            string[] prod = str.Split(new[] { "[\\r\\n]+" }, StringSplitOptions.None);
            int i = 0;
            foreach (string s in prod) {
                prod[i++] = s.Replace("\\s+", "");
            }
            return prod;
        }        

        private static bool lexicalValidation(string str)
        {
            string formatted = str.Replace("\\s+","");
            if (Regex.Matches(formatted, "^[a-zA-Z0-9\\->[&]+").Count > 0)
            {
                return false;
            }
            return true;
        }

        private static RegularGrammar validateProductions(string[] nt, RegularGrammar rg)
        {
            TextScanner vnScan = null;
            string vn = "";
            string prod = "";
            HashSet<string> pr = new HashSet<string>();
            bool isDefined = false;

            for (int i = 0; i < nt.Length; i++)
            {
                prod = nt[i];
                vnScan = new TextScanner(prod);
                vnScan.UseDelimiter("->");
                bool valid;
                
                if (vnScan.HasNext())
                {
                    vn = vnScan.Next();
                    valid = rg.productions.TryGetValue(vn[0], out pr);
                    if (pr == null)
                    {
                        pr = new HashSet<string>();
                    }

                    if (vn.Length > 1 || char.IsLower(vn[0]) || !char.IsLetter(vn[0]))
                    {
                        rg.vn.Clear();
                        vnScan.Close();
                    }
                    else
                    {
                        rg.vn.Add(vn[0]);
                        if (!isDefined)
                        {
                            rg.s = vn[0];
                            isDefined = true;
                        }

                        if (!validateProduction(vn[0], prod, pr, rg))
                        {
                            rg.vn.Clear();
                            vnScan.Close();
                            return null;
                        }
                    }
                }

                vnScan.Close();
            }

            return rg;
        }

        private static bool validateProduction(char vn, string productions, HashSet<string> prodList, RegularGrammar rg)
        {
            string prod = productions.Substring(productions.IndexOf("->") + 2);
            int prodLenght = 0;
            char first, second;
            prodScan = new TextScanner(prod);
            prodScan.UseDelimiter("[|]");
            if (prod.Length < 1)
            {
                prodScan.Close();;
                return false;
            }

            while (prodScan.HasNext())
            {
                prod = prodScan.Next();
                prodLenght = prod.Length;
                if (prodLenght < 1 || prodLenght > 2)
                {
                    prodScan.Close();
                    return false;
                }
                else
                {
                    first = prod[0];
                    if (char.IsUpper(first))
                    {
                        prodScan.Close();
                        return false;
                    }

                    if (char.IsDigit(first) || char.IsLetter(first))
                    {
                        rg.vt.Add(first);
                    }

                    if (prodLenght == 2)
                    {
                        second = prod[1];
                        if (char.IsUpper(first))
                        {
                            prodScan.Close();
                            return false;
                        }
                        if (char.IsLetter(second) || char.IsDigit(second))
                        {
                            prodScan.Close();
                            return false;
                        } 
                        
                        if (first == '&' || second == '&')
                        {
                            prodScan.Close();
                            return false;
                        }
                        if (second == rg.s && rg.vt.Contains('&'))
                        {
                            prodScan.Close();
                            return false;
                        }

                        rg.vn.Add(second);
                        prodList.Add(prod);
                        rg.productions.Add(vn, prodList);
                    }
                    else
                    {
                        if (char.IsUpper(first))
                        {
                            prodScan.Close();
                            return false;
                        }

                        if (first == '&')
                        {
                            rg.vt.Add(first);
                            if (vn != rg.s)
                            {
                                prodScan.Close();
                                return false;
                            }

                            if (rg.productions.Values.Any()) //rever!
                            {
                                prodScan.Close();
                                return false;
                            }
                        }

                        prodList.Add(prod);
                        rg.productions.Add(vn, prodList);
                    }
                }
            }

            prodScan.Close();
            return true;
        }
    }
    
}