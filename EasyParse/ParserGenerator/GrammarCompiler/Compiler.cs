using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class Compiler : MethodMapCompiler
    {
        private Parser StringParser { get; } = Parser.FromXmlResource(
            Assembly.GetExecutingAssembly(), 
            "EasyParse.ParserGenerator.GrammarCompiler.StringParserDefinition.xml", 
            GrammarParser.AddStringLexicalRules);

        private StringCompiler StringCompiler { get; } = new StringCompiler();

        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string, Func<string, object>)[]
        {
            ("q", raw => this.StringParser.Parse(raw).Compile(this.StringCompiler)),
            ("n", value => new NonTerminal(value))
        };

        private Grammar F(ImmutableList<Lexeme> lexemes, string rulesKeyword, string endOfLine, Grammar grammar) => grammar.AddRange(lexemes);
        private Grammar F(string endOfLine, Grammar grammar) => grammar;
        private ImmutableList<Lexeme> L(string lexemesKeyword, string endOfLine) => ImmutableList<Lexeme>.Empty;
        private ImmutableList<Lexeme> L(ImmutableList<Lexeme> lexemes, Lexeme next, string endOfLine) => lexemes.Add(next);
        private Lexeme P(string name, string @is, string pattern) => new LexemePattern(name, pattern);
        private Lexeme P(string ignoreKeyword, string pattern) => new IgnoreLexeme(pattern);
        private Grammar G(Rule rule, string endOfLine) => new Grammar(rule);
        private Grammar G(Grammar rules, Rule next, string endOfLine) => rules.Add(next);
        private Rule R(NonTerminal nonTerminal, string arrow, ImmutableList<Symbol> body) => new Rule(nonTerminal, body);
        private ImmutableList<Symbol> B(Symbol symbol) => ImmutableList<Symbol>.Empty.Add(symbol);
        private ImmutableList<Symbol> B(ImmutableList<Symbol> list, Symbol next) => list.Add(next);
        private Symbol S(string terminal) => new Terminal(terminal);
        private Symbol S(NonTerminal nonTerminal) => nonTerminal;
    }
}
