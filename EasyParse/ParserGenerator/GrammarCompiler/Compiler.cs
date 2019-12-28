using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class Compiler : ICompiler
    {
        public object CompileTerminal(string label, string value)
        {
            throw new System.NotImplementedException();
        }

        public object CompileNonTerminal(string label, object[] children)
        {
            throw new System.NotImplementedException();
        }
    }
}
