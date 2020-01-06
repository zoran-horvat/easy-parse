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
            Assembly.GetExecutingAssembly(), "EasyParse.ParserGenerator.GrammarCompiler.StringParserDefinition.xml");

        private StringCompiler StringCompiler { get; } = new StringCompiler();

        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string, Func<string, object>)[]
        {
            ("quotedString", raw => this.CompileString(raw.Substring(1, raw.Length - 2))),
            ("verbatimString", raw => raw.Substring(2, raw.Length - 3)),
            ("terminal", name => new Terminal(name)),
            ("nonTerminal", value => new NonTerminal(value)),
        };

        private object CompileString(string content) =>
            string.IsNullOrEmpty(content) ? string.Empty
            : this.StringParser.Parse(content).Compile(this.StringCompiler);

        private Grammar Grammar(ImmutableList<Lexeme> lexemes, NonTerminal start, ImmutableList<Rule> rules) => new Grammar(rules).AddRange(lexemes);
        private Grammar Grammar(ImmutableList<Lexeme> lexemes, ImmutableList<Rule> rules) => new Grammar(rules).AddRange(lexemes);

        private ImmutableList<Lexeme> Lexemes(string lexemesKeyword) => ImmutableList<Lexeme>.Empty;
        private ImmutableList<Lexeme> Lexemes(ImmutableList<Lexeme> lexemes, Lexeme next) => lexemes.Add(next);
        private Lexeme Lexeme(string ignoreKeyword, string pattern, string semicolon) => new IgnoreLexeme(pattern);
        private Lexeme Lexeme(Terminal terminal, string matchesKeyword, string pattern, string semicolon) => new LexemePattern(terminal.Value, pattern);

        private NonTerminal Start(string startKeyword, NonTerminal nonTerminal, string semicolon) => nonTerminal;

        private ImmutableList<Rule> Rules(string rulesKeyword) => ImmutableList<Rule>.Empty;
        private ImmutableList<Rule> Rules(ImmutableList<Rule> rules, Rule next) => rules.Add(next);
        private Rule Rule(NonTerminal nonTerminal, string arrow, ImmutableList<Symbol> body, string semicolon) => new Rule(nonTerminal, body);
        private ImmutableList<Symbol> RuleBody(Symbol symbol) => ImmutableList<Symbol>.Empty.Add(symbol);
        private ImmutableList<Symbol> RuleBody(ImmutableList<Symbol> symbols, Symbol next) => symbols.Add(next);
        private Symbol Symbol(Symbol symbol) => symbol;
        private Symbol Symbol(string constant) => new Constant(constant);
        private string String(string value) => value;
    }
}
