using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringSymbolCompiler : MethodMapSymbolCompiler
    {
        private string TerminalNewLine(string value) => "\n";

        private string TerminalCarriageReturn(string value) => "\r";

        private string TerminalTab(string value) => "\t";

        private string TerminalBackslash(string value) => @"\";

        public string TerminalQuote(string value) => "'";

        private string String(string value) => value;
        private string String(string left, string next) => left + next;
        private string Segment(string value) => value;
    }
}
