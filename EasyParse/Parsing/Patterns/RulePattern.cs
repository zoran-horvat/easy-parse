namespace EasyParse.Parsing.Patterns
{
    class RulePattern
    {
        public string NonTerminal { get; }
        public int BodyLength { get; }

        public RulePattern(string nonTerminal, int bodyLength)
        {
            this.NonTerminal = nonTerminal;
            this.BodyLength = bodyLength;
        }

        public static string AugmentedRootNonTerminal => "S'";

        public override string ToString() =>
            $"{this.NonTerminal} -> ({this.BodyLength} symbol(s))";
    }
}
