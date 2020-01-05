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
            ("nonTerminal", value => new NonTerminal(value)),
        };

        private object CompileString(string content) =>
            string.IsNullOrEmpty(content) ? string.Empty
            : this.StringParser.Parse(content).Compile(this.StringCompiler);

        private Grammar FullGrammar(ImmutableList<Lexeme> lexemes, string rulesKeyword, string endOfLine, Grammar grammar) => grammar.AddRange(lexemes);
        private Grammar FullGrammar(string endOfLine, Grammar grammar) => grammar;
        private ImmutableList<Lexeme> Lexemes(string lexemesKeyword, string endOfLine) => ImmutableList<Lexeme>.Empty;
        private ImmutableList<Lexeme> Lexemes(ImmutableList<Lexeme> lexemes, Lexeme next, string endOfLine) => lexemes.Add(next);
        private Lexeme LexemePattern(string name, string @is, string pattern) => new LexemePattern(name, pattern);

        private Lexeme LexemePattern(string ignoreKeyword, string pattern) => new IgnoreLexeme(pattern);

        private Grammar Grammar(Rule rule, string endOfLine) => new Grammar(rule);
        private Grammar Grammar(Grammar rules, Rule next, string endOfLine) => rules.Add(next);
        private Rule Rule(NonTerminal nonTerminal, string arrow, ImmutableList<Symbol> body) => new Rule(nonTerminal, body);
        private ImmutableList<Symbol> RuleBody(Symbol symbol) => ImmutableList<Symbol>.Empty.Add(symbol);
        private ImmutableList<Symbol> RuleBody(ImmutableList<Symbol> list, Symbol next) => list.Add(next);
        private Symbol Symbol(string terminal) => new Terminal(terminal);
        private Symbol Symbol(NonTerminal nonTerminal) => nonTerminal;
        private string String(string value) => value;
    }
}
