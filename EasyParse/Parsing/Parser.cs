using System;
using System.Collections.Generic;
using System.Linq;
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

        private Parser(Lexer lexer, ShiftTable shift, ReduceTable reduce)
        {
            this.Lexer = lexer;
            this.Shift = shift;
            this.Reduce = reduce;
        }

        public static Parser From(XDocument definition, Lexer lexer) => 
            new Parser(lexer, new ShiftTable(definition), new ReduceTable(definition));

        public Node Parse(string input) =>
            this.Parse(this.Lexer.Tokenize(input));

        private Node Parse(IEnumerable<Token> input)
        {
            using (IEnumerator<Token> current = input.GetEnumerator())
            {
                return this.ParseInitial(current, new ParsingStack());
            }
        }

        private Node ParseInitial(IEnumerator<Token> input, ParsingStack stack) =>
            input.MoveNext() ? this.Parse(input, stack)
            : new Error("Internal error: Missing end of input.");

        private Node Parse(IEnumerator<Token> input, ParsingStack stack)
        {
            while (true)
            {
                foreach (Node output in Process(input, stack))
                    return output;
            }
        }

        private IEnumerable<Node> Process(IEnumerator<Token> input, ParsingStack stack) =>
            this.NextAction(input, stack).Invoke();

        private Func<IEnumerable<Node>> NextAction(IEnumerator<Token> input, ParsingStack stack) =>
            this.InvalidInputAction(input, stack)
                .Concat(this.ShiftAction(input, stack))
                .Concat(this.ReduceAction(input, stack))
                .Concat(this.DefaultAction(input))
                .First();

        private IEnumerable<Func<IEnumerable<Node>>> InvalidInputAction(IEnumerator<Token> input, ParsingStack stack)
        {
            if (input.Current is InvalidInput invalid)
                yield return () => new [] {new Error($"Unexpected input: {invalid.Value}")};
        }

        private IEnumerable<Func<IEnumerable<Node>>> ShiftAction(IEnumerator<Token> input, ParsingStack stack)
        {
            if (this.Shift.StateFor(this.StatePatternFor(input, stack)).ToList() is List<int> nextState && nextState.Any())
                yield return () => this.ExecuteShift(input, stack, nextState.First());
        }

        private IEnumerable<Func<IEnumerable<Node>>> ReduceAction(IEnumerator<Token> input, ParsingStack stack)
        {
            if (this.Reduce.ReductionFor(this.StatePatternFor(input, stack)).ToList() is List<RulePattern> rule && rule.Any())
                yield return () => this.ExecuteReduce(stack, rule.First());
        }

        private IEnumerable<Func<IEnumerable<Node>>> DefaultAction(IEnumerator<Token> input)
        {
            yield return () => new[]
            {
                input.Current is EndOfInput ? new Error("Unexpected end of input.") : 
                new Error($"Unexpected input: {input.Current}")
            };
        }

        private StatePattern StatePatternFor(IEnumerator<Token> input, ParsingStack stack) =>
            input.Current is Lexeme lexeme ? (StatePattern)new StateIndexAndLabel(stack.StateIndex, lexeme.Label)
            : input.Current is EndOfInput ? new StateEnd(stack.StateIndex)
            : throw new ArgumentException($"Internal error: {input.Current} not expected.");

        private IEnumerable<Node> ExecuteShift(IEnumerator<Token> input, ParsingStack stack, int nextState)
        {
            stack.Shift(input.Current as Lexeme, nextState);
            if (!input.MoveNext()) yield return new Error("Unexpected end of input.");
        }

        private IEnumerable<Node> ExecuteReduce(ParsingStack stack, RulePattern rule)
        {
            yield return new Error($"Not reduced {rule}");
        }
    }
}
