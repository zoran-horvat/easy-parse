using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing.Collections;
using EasyParse.Parsing.Nodes;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing
{
    public class Parser
    {
        public Lexer Lexer { get; }
        private ShiftTable Shift { get; }
        private ReduceTable Reduce { get; }
        private GotoTable Goto { get; }

        private Parser(Lexer lexer, ShiftTable shift, ReduceTable reduce, GotoTable @goto)
        {
            this.Lexer = lexer;
            this.Shift = shift;
            this.Reduce = reduce;
            this.Goto = @goto;
        }

        public static Parser FromXmlResource(Assembly assembly, string resourceName, Func<Lexer, Lexer> lexicalRules) =>
            From(new XmlResource(assembly, resourceName).Load(), lexicalRules);

        private static Parser From(XDocument definition, Func<Lexer, Lexer> lexicalRules) => 
            new Parser(lexicalRules(LoadLexer(definition)), new ShiftTable(definition), new ReduceTable(definition), new GotoTable(definition));

        private static Lexer LoadLexer(XDocument definition) =>
            LexerWithPatterns(LexerWithIgnores(definition), definition);

        private static Lexer LexerWithIgnores(XDocument definition) =>
            LoadIgnorePatterns(definition)
                .Aggregate(new Lexer(), (lexer, ignore) => lexer.IgnorePattern(ignore));

        private static Lexer LexerWithPatterns(Lexer initial, XDocument definition) =>
            LoadLexicalPatterns(definition)
                .Aggregate(initial, (lexer, pattern) => lexer.AddPattern(pattern.pattern, pattern.name));

        private static IEnumerable<string> LoadIgnorePatterns(XDocument definition) =>
            definition.Root
                ?.Element("LexicalRules")
                ?.Elements("Ignore")
                .Select(ignore => ignore.Attribute("Pattern")?.Value ?? string.Empty)
                .Where(pattern => !string.IsNullOrEmpty(pattern))
            ?? Enumerable.Empty<string>();

        private static IEnumerable<(string pattern, string name)> LoadLexicalPatterns(XDocument definition) =>
            definition.Root
                ?.Element("LexicalRules")
                ?.Elements("Lexeme")
                .Select(lexeme => (pattern: lexeme.Attribute("Pattern")?.Value ?? string.Empty, name: lexeme.Attribute("Name")?.Value ?? string.Empty))
                .Where(tuple => !string.IsNullOrEmpty(tuple.pattern) && !string.IsNullOrEmpty(tuple.name))
            ?? Enumerable.Empty<(string, string)>();
                
        public ParsingResult Parse(string input) =>
            this.Parse(this.Lexer.Tokenize(input));

        public ParsingResult Parse(IEnumerable<string> lines) =>
            this.Parse(this.Lexer.Tokenize(lines));

        private ParsingResult Parse(IEnumerable<Token> input)
        {
            using (IEnumerator<Token> current = input.GetEnumerator())
            {
                return new ParsingResult(this.ParseInitial(current, new ParsingStack()));
            }
        }

        private TreeElement ParseInitial(IEnumerator<Token> input, ParsingStack stack) =>
            input.MoveNext() ? this.Parse(input, stack)
            : new Error("Internal error: Missing end of input.");

        private TreeElement Parse(IEnumerator<Token> input, ParsingStack stack)
        {
            while (true)
            {
                foreach (TreeElement output in Process(input, stack))
                    return output;
            }
        }

        private IEnumerable<TreeElement> Process(IEnumerator<Token> input, ParsingStack stack) => 
            this.NextAction(input, stack).Invoke().ToList();

        private Func<IEnumerable<TreeElement>> NextAction(IEnumerator<Token> input, ParsingStack stack) =>
            this.InvalidInputAction(input, stack)
                .Concat(this.ShiftAction(input, stack))
                .Concat(this.ReduceAction(input, stack))
                .Concat(this.DefaultAction(input))
                .First();

        private IEnumerable<Func<IEnumerable<TreeElement>>> InvalidInputAction(IEnumerator<Token> input, ParsingStack stack)
        {
            if (input.Current is InvalidInput invalid)
                yield return () => new [] {this.InputError(input.Current)};
        }

        private IEnumerable<Func<IEnumerable<TreeElement>>> ShiftAction(IEnumerator<Token> input, ParsingStack stack)
        {
            if (this.Shift.StateFor(this.StatePatternFor(input, stack)).ToList() is List<int> nextState && nextState.Any())
                yield return () => this.ExecuteShift(input, stack, nextState.First());
        }

        private IEnumerable<Func<IEnumerable<TreeElement>>> ReduceAction(IEnumerator<Token> input, ParsingStack stack)
        {
            if (this.Reduce.ReductionFor(this.StatePatternFor(input, stack)).ToList() is List<RulePattern> rule && rule.Any())
                yield return () => this.ExecuteReduce(input, stack, rule.First());
        }

        private IEnumerable<Func<IEnumerable<TreeElement>>> DefaultAction(IEnumerator<Token> input)
        {
            yield return () => new[] { this.InputError(input.Current) };
        }

        private StatePattern StatePatternFor(IEnumerator<Token> input, ParsingStack stack) =>
            input.Current is EndOfInput ? new StateEnd(stack.StateIndex)
            : input.Current is Lexeme lexeme ? (StatePattern)new StateIndexAndLabel(stack.StateIndex, lexeme.Label)
            : throw new ArgumentException($"Internal error: {input.Current} not expected.");

        private IEnumerable<TreeElement> ExecuteShift(IEnumerator<Token> input, ParsingStack stack, int nextState)
        {
            stack.Shift(input.Current as Lexeme, nextState);
            if (!input.MoveNext()) yield return new Error("Unexpected end of input.");
        }

        private IEnumerable<TreeElement> ExecuteReduce(IEnumerator<Token> input, ParsingStack stack, RulePattern rule)
        {
            int stateIndex = stack.Reduce(rule);
            StateIndexAndLabel statePattern = new StateIndexAndLabel(stateIndex, rule.NonTerminal);

            if (rule.NonTerminal == RulePattern.AugmentedRootNonTerminal)
            {
                yield return stack.Result;
            }
            else if (this.Goto.NextStateFor(statePattern).ToList() is List<int> nextState && nextState.Any())
            {
                stack.Goto(nextState.First());
            }
            else
            {
                yield return this.InputError(input.Current);
            }
        }

        private TreeElement InputError(Token input) =>
            input is InvalidInput invalid ? new Error($"Unexpected input: {invalid.Value}")
            : input is EndOfInput ? new Error("Unexpected end of input.") 
            : new Error($"Unexpected input: {input}");
    }
}
