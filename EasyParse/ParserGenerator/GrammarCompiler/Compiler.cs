using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class Compiler : ICompiler
    {
        public object CompileTerminal(string label, string value) =>
            label == "t" ? new Terminal(value)
            : label == "n" ? (object)new NonTerminal(value)
            : value;

        public object CompileNonTerminal(string label, object[] children) =>
            label == "Q" ? children[children.Length - 1]
            : label == "G" ? this.CompileGrammar(children)
            : label == "U" ? this.CompileLine(children)
            : label == "R" ? this.CompileRule(children)
            : label == "B" ? this.CompileBody(children)
            : label == "S" ? this.CompileSymbol(children)
            : this.InternalError(label, children);

        private object CompileGrammar(object[] children) =>
            children[0] is IEnumerable<Rule> rules ? new Grammar(rules)
            : children[0] is Grammar grammar && children[1] is IEnumerable<Rule> nextRules ? (object)grammar.AddRange(nextRules)
            : this.InternalError("G", children);

        private object CompileLine(object[] children) =>
            children.OfType<Rule>().ToList();

        private object CompileRule(object[] children) =>
            children[0] is NonTerminal nonTerminal && children[2] is ImmutableList<Symbol> symbols ? this.CompileRule(nonTerminal, symbols)
            : this.InternalError("R", children);

        private object CompileRule(NonTerminal nonTerminal, IEnumerable<Symbol> symbols) =>
            new Rule(nonTerminal, symbols);

        private object CompileBody(object[] children) =>
            children[0] is ImmutableList<Symbol> symbols && children[1] is Symbol next ? symbols.Add(next)
            : children[0] is Symbol first ? (object)ImmutableList<Symbol>.Empty.Add(first)
            : this.InternalError("B", children);

        private object CompileSymbol(object[] children) =>
            children[0] is Symbol symbol ? (object)symbol
            : this.InternalError("S", children);

        private string InternalError(string label, object[] children) =>
            $"Internal error compiling {label} -> {this.ToString(children)}";

        private string ToString(object[] children) =>
            string.Join(" ", children.Select(child => $"{child}").ToArray());
    }
}
