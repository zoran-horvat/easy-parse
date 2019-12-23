using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Parsing.Nodes;

namespace EasyParse.Parsing.Formatting
{
    static class Formatter
    {
        public static string Printable(this TreeElement tree) =>
            tree is Error error ? error.Message
            : tree is Node node ? node.Printable()
            : "<unknown>";

        private static string Printable(this Node node) =>
            string.Join(Environment.NewLine, node.PrintableLines().ToArray());

        private static IEnumerable<string> PrintableLines(this Node node) =>
            node is NonTerminalNode nonTerminal ? nonTerminal.PrintableLines()
            : new[] {$" {node.Label}"};

        private static IEnumerable<string> PrintableLines(this NonTerminalNode nonTerminal)
        {
            yield return $" {nonTerminal.Label}";
            Stack<(NonTerminalNode node, int position)> stack = new Stack<(NonTerminalNode node, int position)>();
            stack.Push((nonTerminal, 0));

            while (stack.Any())
            {
                (NonTerminalNode node, int position) = stack.Pop();
                if (position < node.Children.Length)
                {
                    string prefix = string.Join(
                        string.Empty,
                        stack.Reverse().Select(tuple => tuple.position < tuple.node.Children.Length ? "|    " : "     ").ToArray());
                    
                    yield return $" {prefix}|";

                    string nodeValue = node.Children[position] is TerminalNode terminal ? terminal.Value : node.Children[position].Label;
                    yield return $" {prefix}+--- {nodeValue}";
                    
                    stack.Push((node, position + 1));
                    if (node.Children[position] is NonTerminalNode nonTerminalChild)
                        stack.Push((nonTerminalChild, 0));
                }
            }
        }
    }
}
