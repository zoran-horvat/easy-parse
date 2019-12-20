using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Parsing.Nodes;

namespace EasyParse.Parsing
{
    class ParsingStack
    {
        private Stack<object> Content { get; }

        public ParsingStack()
        {
            this.Content = new Stack<object>();
            this.Content.Push(0);
        }

        public int StateIndex =>
            (int) this.Content.Peek();

        public bool Shift(IEnumerator<Lexeme> input, int nextState)
        {
            this.Content.Push(new TerminalNode(input.Current?.Label ?? string.Empty, input.Current?.Value ?? string.Empty));
            this.Content.Push(nextState);
            return input.MoveNext();
        }
    }
}
