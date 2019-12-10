using ParserCompiler.Symbols;

namespace ParserCompiler
{
    public abstract class Symbol
    {
        private string Representation { get; }
        
        protected Symbol(string representation)
        {
            this.Representation = representation;
        }

        public static Symbol From(char representation) =>
            char.IsUpper(representation) ? (Symbol)new NonTerminal(representation.ToString()) 
            : new Terminal(representation.ToString());

        public override string ToString() =>
            this.Representation;
    }
}
