using System;
using System.Collections.Generic;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringCompiler : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string terminal, Func<string, object> map)[]
        {
            ("newLine", _ => "\n"), 
            ("carriageReturn", _ => "\r"),
            ("tab", _ => "\t"),
            ("backslash", _ => @"\"),
            ("quote", _ => "'"),
        };

        private string String(string value) => value;
        private string String(string left, string next) => left + next;
        private string Segment(string value) => value;
    }
}
