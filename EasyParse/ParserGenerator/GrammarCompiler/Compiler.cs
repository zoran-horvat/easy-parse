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

        protected override IEnumerable<(string label, string methodName)> Map => new[]
        {
            ("r", nameof(Terminal)),
            ("t", nameof(Terminal)),
            ("l", nameof(Terminal)),
            ("i", nameof(Terminal)),
            ("s", nameof(Terminal)),
            ("a", nameof(Terminal)),
            ("e", nameof(Terminal)),
            ("q", nameof(QuotedString)),
            ("n", nameof(NonTerminal)),
            ("F", nameof(FullGrammar)),
            ("L", nameof(Lexemes)),
            ("P", nameof(LexemePattern)),
            ("G", nameof(Grammar)),
            ("R", nameof(Rule)),
            ("B", nameof(RuleBody)),
            ("S", nameof(Symbol))
        };

        private Terminal Terminal(string value) => new Terminal(value);
        private Terminal QuotedString(string raw) => new Terminal(this.CompileStringLiteral(raw));
        private NonTerminal NonTerminal(string value) => new NonTerminal(value);
        private Grammar FullGrammar(ImmutableList<Lexeme> lexemes, Terminal rulesKeyword, Terminal endOfLine, Grammar grammar) => grammar.AddRange(lexemes);
        private Grammar FullGrammar(Terminal endOfLine, Grammar grammar) => grammar;
        private ImmutableList<Lexeme> Lexemes(Terminal lexemesKeyword, Terminal endOfLine) => ImmutableList<Lexeme>.Empty;
        private ImmutableList<Lexeme> Lexemes(ImmutableList<Lexeme> lexemes, Lexeme next, Terminal endOfLine) => lexemes.Add(next);
        private Lexeme LexemePattern(Terminal name, Terminal @is, Terminal pattern) => new LexemePattern(name.Value, pattern.Value);
        private Lexeme LexemePattern(Terminal ignore, Terminal pattern) => new IgnoreLexeme(pattern.Value);
        private Grammar Grammar(Rule rule, Terminal endOfLine) => new Grammar(rule);
        private Grammar Grammar(Grammar rules, Rule next, Terminal endOfLine) => rules.Add(next);
        private Rule Rule(NonTerminal nonTerminal, Terminal arrow, ImmutableList<Symbol> body) => new Rule(nonTerminal, body);
        private ImmutableList<Symbol> RuleBody(Symbol symbol) => ImmutableList<Symbol>.Empty.Add(symbol);
        private ImmutableList<Symbol> RuleBody(ImmutableList<Symbol> list, Symbol next) => list.Add(next);
        private Symbol Symbol(Terminal terminal) => terminal;
        private Symbol Symbol(NonTerminal nonTerminal) => nonTerminal;

        private string CompileStringLiteral(string value) => 
            (string)this.StringParser.Parse(value).Compile(this.StringCompiler);
    }
}
