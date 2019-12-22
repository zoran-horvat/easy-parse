using System.Collections.Generic;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing.Nodes;
using EasyParse.Parsing.Patterns;

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

        public int Reduce(RulePattern rule)
        {
            Stack<TreeElement> children = new Stack<TreeElement>();
            for (int i = 0; i < rule.BodyLength; i++)
            {
                this.Content.Pop();
                children.Push((TreeElement) this.Content.Pop());
            }

            int stateIndex = (int) this.Content.Peek();
            this.Content.Push(new NonTerminalNode(rule.NonTerminal, children));
            return stateIndex;
        }
    }
}
