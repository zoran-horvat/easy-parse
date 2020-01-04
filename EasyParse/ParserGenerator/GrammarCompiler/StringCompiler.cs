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
            value.Length != 2 ? (object)new InvalidOperationException($"Internal error parsing '{value}'")
            : new string(value[1], 1); 

        private string S(string value) => value;
        private string S(string left, string next) => left + next;
        private string T(string value) => value;
    }
}
