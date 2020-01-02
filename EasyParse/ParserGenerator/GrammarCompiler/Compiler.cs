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
        public object CompileTerminal(string label, string value)
        {
            var x = new[] {"t", "l", "i", "s"}.Contains(label) ? new Terminal(value)
                : label == "q" ? new Terminal(value.Substring(1, value.Length - 2))
                : label == "n" ? (object) new NonTerminal(value)
                : value;
            return x;
        }

        public object CompileNonTerminal(string label, object[] children) =>
            label == "Q" ? this.CompileFullGrammar(children)
            : label == "G" ? this.CompileGrammar(children)
            : label == "R" ? this.CompileRule(children)
            : label == "B" ? this.CompileBody(children)
            : label == "S" ? this.CompileSymbol(children)
            : label == "L" ? this.CompileLexemes(children)
            : label == "P" ? this.CompileLexemePattern(children)
            : this.InternalError(label, children);

        private object CompileFullGrammar(object[] children) =>
            children[0] is ImmutableList<Lexeme> lexemes && children[3] is Grammar grammar ? grammar.AddRange(lexemes)
            : children[1] is Grammar fullGrammar ? (object)fullGrammar
            : this.InternalError("Q", children);

        private object CompileGrammar(object[] children) =>
            children[0] is Rule firstRule ? new Grammar(firstRule)
            : children[0] is Grammar leftGrammar && children[1] is Rule lastRule ? (object)leftGrammar.Add(lastRule)
            : this.InternalError("G", children);

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

        private object CompileLexemes(object[] children)
        {
            var x = 
                children.Length == 3 && children[0] is ImmutableList<Lexeme> lexemes && children[1] is Lexeme next ? lexemes.Add(next)
                : ImmutableList<Lexeme>.Empty;
            return x;
        }

        private object CompileLexemePattern(object[] children) =>
            children.Length == 3 && children[0] is Terminal name && children[2] is Terminal pattern ? new LexemePattern(name.Value, pattern.Value)
            : children.Length == 2 && children[1] is Terminal ignore ? (object)new IgnoreLexeme(ignore.Value)
            : this.InternalError("L", children);

        private string InternalError(string label, object[] children) =>
            $"Internal error compiling {label} -> {this.ToString(children)}";

        private string ToString(object[] children) =>
            string.Join(" ", children.Select(child => $"{child}").ToArray());
    }
}
