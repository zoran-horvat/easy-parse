using System;
using System.Collections.Generic;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringCompiler : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string terminal, Func<string, object> map)[]
        {

        };

        private string S(string value) => value;
        private string P(string quote1, string quote2) => string.Empty;
        private string P(string quote1, string content, string quote2) => content;
        private string C(string value) => value;
        private string C(string left, string more) => left + more;
        private string D(string value) => value;
        private string D(string left, string more) => left + more;
        private string G(string value) => value;
        private string V(string at, string quote1, string quote2) => string.Empty;
        private string V(string at, string quote1, string content, string quote2) => content;
    }
}
