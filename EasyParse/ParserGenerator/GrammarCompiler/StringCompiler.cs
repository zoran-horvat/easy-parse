using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
        private object P(string quote1, ImmutableList<string> content, string quote2) => this.Join(this.EscapeRegular(content));
        private ImmutableList<string> C(string value) => ImmutableList<string>.Empty.Add(value);
        private ImmutableList<string> C(ImmutableList<string> left, string more) => left.Add(more);
        private string D(string value) => value;
        private string D(string left, string more) => left + more;
        private string G(string value) => value;
        private string V(string at, string doubleQuote) => string.Empty;
        private object V(string at, string quote1, ImmutableList<string> content, string quote2) => this.Join(this.EscapeVerbatim(content));

        private IEnumerable<object> EscapeRegular(IEnumerable<string> segments) =>
            segments.Select(segment =>
                segment == "''" ? (object)new FormatException($"Not supported {segment} in regular string")
                : segment);

        private IEnumerable<object> EscapeVerbatim(IEnumerable<string> segments) =>
            segments.Select(segment =>
                segment == "''" ? "'"
                : segment);

        private object Join(IEnumerable<object> segments) =>
            segments.Aggregate((object)string.Empty, (acc, segment) =>
                acc is string left && segment is string more ? left + more
                : acc is Exception ? acc
                : segment);
    }
}
