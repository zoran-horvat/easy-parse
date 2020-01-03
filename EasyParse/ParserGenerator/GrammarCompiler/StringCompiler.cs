using System;
using System.Linq;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringCompiler : ICompiler
    {
        public object CompileTerminal(string label, string value) =>
            new Terminal(value);

        public object CompileNonTerminal(string label, object[] children) =>
            label == "S" && children.Length == 2 ? string.Empty
            : label == "S" && children.Length == 3 && children[1] is Terminal finalContent ? finalContent.Value
            : label == "C" && children.Length == 1 ? children[0]
            : this.Fail(label, children);

        private object Fail(string label, object[] children) =>
            new Exception($"{label}({this.Join(children)})");

        private string Join(object[] children) =>
            string.Join(", ", children.Select(child => $"{child}").ToArray());
    }
}
