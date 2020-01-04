using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringCompiler : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string terminal, Func<string, object> map)[]
        {

        };

        private string S(string value) => value;
        private string P(string doubleQuoted) => string.Empty;

        private string P(string quote1, ImmutableList<string> content, string quote2) =>
            string.Join(string.Empty, content.ToArray());

        private ImmutableList<string> C(string value) => ImmutableList<string>.Empty.Add(value);
        private ImmutableList<string> C(ImmutableList<string> left, string more) => left.Add(more);
        private string D(string value) => value;
        private string D(string left, string more) => left + more;
        private string G(string value) => value;
        private string V(string at, string doubleQuote) => string.Empty;

        private string V(string at, string quote1, ImmutableList<string> content, string quote2) =>
            string.Join(string.Empty, content.ToArray());
    }
}
