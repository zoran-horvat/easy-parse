using EasyParse.Text;

namespace EasyParse.Parsing
{
    public interface ISymbolCompiler
    {
        object CompileTerminal(string label, string value);
        object CompileNonTerminal(Location location, string label, object[] children);
    }
}
