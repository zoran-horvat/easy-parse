﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing.Collections;
using EasyParse.Parsing.Nodes;

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
            input.Current is InvalidInput invalid ? new [] {new Error($"Unexpected input: {invalid.Value}")}
            : this.ShiftReduce(input, stack);

        private IEnumerable<Node> ShiftReduce(IEnumerator<Token> input, ParsingStack stack) =>
            this.Shift.StateFor(input, stack)
                .SelectMany(nextState => this.ShiftAction(input, stack, nextState));

        private IEnumerable<Node> ShiftAction(IEnumerator<Token> input, ParsingStack stack, int nextState)
        {
            stack.Shift(input.Current as Lexeme, nextState);
            if (!input.MoveNext()) yield return new Error("Unexpected end of input.");
            yield return new Error("Not parsed");
        }
    }
}
