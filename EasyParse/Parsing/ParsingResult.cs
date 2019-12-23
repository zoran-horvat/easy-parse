using System;
using System.Linq;
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

        public string ErrorMessage =>
            this.Content is Error error ? error.Message : string.Empty;

        public bool IsSuccess =>
            this.Content is Node;

        public object Compile(Func<string, object[], object> nodeCompiler) =>
            this.Content is Node node ? this.Compile(node, nodeCompiler)
            : throw new InvalidOperationException("Cannot compile failed parse result.");

        private object Compile(Node node, Func<string, object[], object> nodeCompiler) =>
            node is TerminalNode terminal ? this.Compile(terminal, nodeCompiler)
            : node is NonTerminalNode nonTerminal ? this.Compile(nonTerminal, nodeCompiler)
            : throw new InvalidOperationException($"Internal error compiling {node}");

        private object Compile(TerminalNode terminal, Func<string, object[], object> nodeCompiler) =>
            nodeCompiler(terminal.Label, new object[] {terminal.Value});

        private object Compile(NonTerminalNode nonTerminal, Func<string, object[], object> nodeCompiler) =>
            nodeCompiler(nonTerminal.Label, nonTerminal.Children.Select(child => this.Compile(child, nodeCompiler)).ToArray());

        public override string ToString() => 
            this.Content.Printable();
    }
}
