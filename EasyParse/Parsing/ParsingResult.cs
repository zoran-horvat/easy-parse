using System;
using EasyParse.Parsing.Formatting;
using EasyParse.Parsing.Nodes;

namespace EasyParse.Parsing
{
    public class ParsingResult
    {
        private TreeElement Content { get; }

        public ParsingResult(TreeElement content)
        {
            this.Content = content;
        }

        public override string ToString() => this.Content.Printable();
    }
}
