using System;
using System.Collections.Generic;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringCompiler : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string terminal, Func<string, object> map)[]
        {
            ("e", this.Unescape),
        };

        private object Unescape(string value) => 
            value.Length != 2 ? new InvalidOperationException($"Internal error parsing '{value}'")
            : value[1] == 'n' ? "\n"
            : value[1] == 'r' ? "\r"
            : value[1] == 't' ? (object)"\t"
            : new string(value[1], 1); 

        private string String(string value) => value;
        private string String(string left, string next) => left + next;
        private string Segment(string value) => value;
    }
}
