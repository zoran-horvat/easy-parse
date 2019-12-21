using System;
using System.Collections.Generic;
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

        private Parser(Lexer lexer, ShiftTable shift)
        {
            this.Lexer = lexer;
            this.Shift = shift;
        }

        public static Parser From(XDocument definition, Lexer lexer) =>
            new Parser(lexer, new ShiftTable(definition));

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
            : this.ShiftReduce(stack, input);

        private IEnumerable<Node> ShiftReduce(ParsingStack stack, IEnumerator<Token> current) =>
            new[] {new Error("Error")};
    }
}
