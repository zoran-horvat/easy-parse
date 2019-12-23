namespace EasyParse.Parsing
{
    public interface ICompiler
    {
        object CompileTerminal(string label, string value);
        object CompileNonTerminal(string label, object[] children);
    }
}
