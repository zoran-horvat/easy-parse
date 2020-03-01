using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing.Collections;
using EasyParse.Parsing.Nodes;
using EasyParse.Parsing.Nodes.Errors;
using EasyParse.Parsing.Patterns;
using EasyParse.Text;
using EndOfInput = EasyParse.LexicalAnalysis.Tokens.EndOfInput;

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

        public static Parser FromXmlResource(Assembly assembly, string resourceName) =>
            FromXmlResource(assembly, resourceName, lexer => lexer);

        public static Parser From(ParserDefinition definition) =>
            new Parser(
                LexerLoader.From(definition),
                ShiftTable.From(definition.Table.Shift),
                ReduceTable.From(definition.Table.Reduce, definition.Grammar.Rules.ToArray()),
                GotoTable.From(definition.Table.Goto));
            
        public static Parser From(XDocument definition, Func<Lexer, Lexer> lexicalRules) => 
            new Parser(lexicalRules(LexerLoader.From(definition)), 
                ShiftTable.From(definition), ReduceTable.From(definition), GotoTable.From(definition));
                
        public ParsingResult Parse(string input) =>
            this.Parse(this.Lexer.Tokenize(Plaintext.Line(input)));

        public ParsingResult Parse(IEnumerable<string> lines) =>
            this.Parse(this.Lexer.Tokenize(Plaintext.Text(lines)));

        private ParsingResult Parse(IEnumerable<Token> input)
        {
            using (IEnumerator<Token> current = input.GetEnumerator())
            {
                return new ParsingResult(this.ParseInitial(current, new ParsingStack()));
            }
        }

        private TreeElement ParseInitial(IEnumerator<Token> input, ParsingStack stack) =>
            input.MoveNext() ? this.Parse(input, stack)
            : new Error(input.Current.Location, "Internal error: Missing end of input.");

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
            if (!input.MoveNext()) yield return new UnexpectedEndOfInput(input.Current.Location);
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
            input is InvalidInput invalid ? new LexingError(input.Location, invalid.Value)
            : input is EndOfInput ? new UnexpectedEndOfInput(input.Location) 
            : new Error(input.Location, $"Unexpected input: {input} at {input.Location}");
    }
}
