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

        private string TerminalQuotedString(string value) =>
            this.CompileString(value.Substring(1, value.Length - 2));

        private string TerminalVerbatimString(string value) =>
            value.Substring(2, value.Length - 3);

        private Terminal TerminalTerminal(string name) =>
            new Terminal(name);

        private NonTerminal TerminalNonTerminal(string value) =>
            new NonTerminal(value);

        private string CompileString(string content) =>
            string.IsNullOrEmpty(content) ? string.Empty
            : (string)this.StringParser.Parse(content).Compile(this.StringCompiler);

        private Grammar Grammar(ImmutableList<Lexeme> lexemes, NonTerminal start, ImmutableList<RuleDefinition> rules) => new Grammar(start, rules).AddRange(lexemes);
        
        private ImmutableList<Lexeme> Lexemes(string lexemesKeyword, string colon) => ImmutableList<Lexeme>.Empty;
        private ImmutableList<Lexeme> Lexemes(ImmutableList<Lexeme> lexemes, Lexeme next) => lexemes.Add(next);
        private Lexeme Lexeme(string ignoreKeyword, string pattern, string semicolon) => new IgnoreLexeme(pattern);
        private Lexeme Lexeme(Terminal terminal, string matchesKeyword, string pattern, string semicolon) => new LexemePattern(terminal.Value, pattern);

        private NonTerminal Start(string startKeyword, string colon, NonTerminal nonTerminal, string semicolon) => nonTerminal;

        private ImmutableList<RuleDefinition> Rules(string rulesKeyword, string colon, RuleDefinition initialRule) => ImmutableList<RuleDefinition>.Empty.Add(initialRule);
        private ImmutableList<RuleDefinition> Rules(ImmutableList<RuleDefinition> rules, RuleDefinition next) => rules.Add(next);
        private RuleDefinition Rule(NonTerminal nonTerminal, string arrow, ImmutableList<Symbol> body, string semicolon) => new RuleDefinition(nonTerminal, body);
        private ImmutableList<Symbol> RuleBody(Symbol symbol) => ImmutableList<Symbol>.Empty.Add(symbol);
        private ImmutableList<Symbol> RuleBody(ImmutableList<Symbol> symbols, Symbol next) => symbols.Add(next);
        private Symbol Symbol(Symbol symbol) => symbol;
        private Symbol Symbol(string constant) => new Constant(constant);
        private string String(string value) => value;
    }
}
