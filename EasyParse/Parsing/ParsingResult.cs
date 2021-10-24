using System;
using System.Linq;
using EasyParse.Parsing.Formatting;
using EasyParse.Parsing.Nodes;
using EasyParse.Parsing.Nodes.Errors;

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

        public Error Error =>
            this.Content is Error error ? error : throw new InvalidOperationException("Not an error.");

        public bool IsSuccess =>
            this.Content is Node;

        public object Compile(ISymbolCompiler nodeCompiler) =>
            this.Content is Error error ? error
            : this.Content is Node node ? this.Compile(node, nodeCompiler)
            : this.Throw(this.Content);

        private object Throw(object result) =>
            throw new InvalidOperationException(result is Error error ? error.Message : "Cannot compile failed parse result.");

        private object Compile(Node node, ISymbolCompiler nodeCompiler) =>
            node is TerminalNode terminal ? this.Compile(terminal, nodeCompiler)
            : node is NonTerminalNode nonTerminal ? this.Compile(nonTerminal, nodeCompiler)
            : this.InternalError(node);

        private object InternalError(Node node) =>
            new CompileError(node.Location, node.GetType().Name, Enumerable.Empty<object>());

        private object Compile(TerminalNode terminal, ISymbolCompiler nodeCompiler) =>
            nodeCompiler.CompileTerminal(terminal.Label, terminal.Value);

        private object Compile(NonTerminalNode nonTerminal, ISymbolCompiler nodeCompiler) =>
            nodeCompiler.CompileNonTerminal(
                nonTerminal.Location,
                nonTerminal.Label, 
                nonTerminal.ProducedBy,
                nonTerminal.Children.Select(child => this.Compile(child, nodeCompiler)).ToArray());

        public override string ToString() => 
            this.Content.Printable();

        public string ToDenseString() =>
            this.Content.Printable(true);
    }
}
