using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringCompiler : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap =>
            Enumerable.Empty<(string, Func<string, object>)>();

        private string S(string quote1, string quote2) => string.Empty;
        private string S(string quote1, string content, string quote2) => content;
        private string C(string value) => value;
    }
}
