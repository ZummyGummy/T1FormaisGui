namespace t1FormaisGUI
{
    public abstract class RegularLanguage
    {
        public enum inputType
        {
            RE,
            RG,
            FA,
            Undefined
        };

        public enum Operation
        {
            Union,
            Concatenation,
            Intersection,
            Difference
        };

        protected string input;
        private string id;
        private inputType type = inputType.Undefined;

        public RegularLanguage(inputType type)
        {
            this.type = type;
        }

        public RegularLanguage(string input, inputType type)
        {
            this.input = input;
            this.type = type;
        }

        public static RegularLanguage validate(string ip)
        {
            if (ip.Contains("->"))
            {
                return (RegularLanguage) RegularGrammar.isValidRG(ip);
            }

            return (RegularLanguage) RegularExpression.isValidRE(ip);
        }

        public string getId()
        {
            return this.id;
        }

        public string getInput()
        {
            return this.input;
        }

        public void setId(string id)
        {
            this.id = id;
        }

        public inputType getType()
        {
            return this.type;
        }

        public bool isFinite()
        {
            return true;
        }

        public bool isEmpty()
        {
            return true;
        }

        public bool isEqualTo(RegularLanguage l1, RegularLanguage l2)
        {
            return true;
        }

        public bool isContainedIn(RegularLanguage l1, RegularLanguage l2)
        {
            return true;
        }

        public string toString()
        {
            return this.id;
        }

        public abstract string getDefinition();

        public abstract RegularGrammar getRG();

        public abstract RegularExpression getRE();

//        public abstract FiniteAutomata getFA();

    }
}