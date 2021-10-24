using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Text;

namespace EasyParse.Parsing
{
    public interface ISymbolCompiler
    {
        object CompileTerminal(string label, string value);
        object CompileNonTerminal(Location location, string label, RuleReference production, object[] children);
    }
}
