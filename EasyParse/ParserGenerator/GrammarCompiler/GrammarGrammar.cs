using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class GrammarGrammar : NativeGrammar
    {
        private Compiler<string> StringCompiler { get; } = new StringGrammar().BuildCompiler<string>();
        protected override IEnumerable<Regex> IgnorePatterns => new[] { new Regex(@"\s+"), new Regex("#.*") };

        public NonTerminal NonTerminal([R("nonTerminal", "[A-Z][a-zA-Z0-9_]*")] string name) => new(name);
        public Terminal Terminal([R("terminal", @"[a-z_][a-zA-Z0-9_]*")] string name) => new(name);
        public string QuotedString([R("quotedString", @"\'((\\[\\\'nrt])|[^\\\'])*\'")] string value) => this.StringCompiler.Compile(value[1..^1]).Result;
        public string VerbatimString([R("verbatimString", "@\'[^\']*\'")] string value)
        {
            string result = this.StringCompiler.Compile(value[2..^1]).Result;
            if (result is null) throw new Exception($"Verbatim string [{value[2..^1]}] compiled into null");
            return result;
        }

        public NonTerminal Start([L("start")] string start, [L(":")] string colon, NonTerminal nonTerminal, [L(";")] string semicolon) => nonTerminal;

        public ImmutableList<Lexeme> Lexemes([L("lexemes")] string lexemes, [L(":")] string colon) => ImmutableList<Lexeme>.Empty;
        public ImmutableList<Lexeme> Lexemes(ImmutableList<Lexeme> lexemes, Lexeme lexeme) => lexemes.Add(lexeme);

        public Lexeme Lexeme([L("ignore")] string ignore, Constant @string, [L(";")] string semicolon) => new IgnoreLexeme(@string.Value);
        public Lexeme Lexeme(Terminal terminal, [L("matches")] string matches, Constant @string, [L(";")] string semicolon) => new LexemePattern(terminal.Value, @string.Value);

        [Start] public Grammar Grammar(ImmutableList<Lexeme> lexemes, NonTerminal start, ImmutableList<RuleDefinition> rules) =>
            new Grammar(start, rules).AddRange(lexemes);

        public ImmutableList<RuleDefinition> Rules([L("rules")] string rules, [L(":")] string colon, RuleDefinition rule) =>
            ImmutableList<RuleDefinition>.Empty.Add(rule);

        public ImmutableList<RuleDefinition> Rules(ImmutableList<RuleDefinition> rules, RuleDefinition rule) => rules.Add(rule);

        public RuleDefinition Rule(NonTerminal nonTerminal, [L("->")] string arrow, ImmutableList<Symbol> ruleBody, [L(";")] string semicolon) =>
            new(nonTerminal, ruleBody);

        public ImmutableList<Symbol> RuleBody(Symbol symbol) => ImmutableList<Symbol>.Empty.Add(symbol);
        public ImmutableList<Symbol> RuleBody(ImmutableList<Symbol> ruleBody, Symbol symbol) => ruleBody.Add(symbol);

        public Symbol Symbol(Terminal terminal) => terminal;
        public Symbol Symbol(NonTerminal nonTerminal) => nonTerminal;
        public Symbol Symbol(Constant @string) => @string;
        public Constant String([From(nameof(QuotedString), nameof(VerbatimString))] string value)
        {
            if (string.IsNullOrEmpty(value)) throw new Exception($"Constant string is {(value is null ? "null" : "empty")}");
            return new(value);
        }
    }
}
