using System.Collections.Generic;
using EasyParse.LexicalAnalysis.Tokens;
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

        public void Shift(Lexeme input, int nextState)
        {
            this.Content.Push(new TerminalNode(input.Label, input.Value));
            this.Content.Push(nextState);
        }
    }
}
