using System;
using System.Collections.Generic;
using System.Linq;
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
            Stack<Node> children = new Stack<Node>();
            for (int i = 0; i < rule.BodyLength; i++)
            {
                this.Content.Pop();
                children.Push((Node)this.Content.Pop());
            }

            int stateIndex = (int) this.Content.Peek();
            this.Content.Push(new NonTerminalNode(rule.NonTerminal, children));
            return stateIndex;
        }

        public void Goto(int stateIndex)
        {
            this.Content.Push(stateIndex);
        }

        public Node Result =>
            ((NonTerminalNode) this.Content.Peek()).Children[0];

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Content.Select(item => $"{item}").ToArray());
    }
}
