using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Parsing.Nodes;

namespace EasyParse.Parsing.Formatting
{
    static class Formatter
    {
        public static string Printable(this TreeElement tree, bool dense = false) =>
            tree is Error error ? error.Message
            : tree is Node node ? node.Printable(dense)
            : "<unknown>";

        private static string Printable(this Node node, bool dense) =>
            string.Join(Environment.NewLine, node.PrintableLines(dense).ToArray());

        private static IEnumerable<string> PrintableLines(this Node node, bool dense) =>
            node is NonTerminalNode nonTerminal ? nonTerminal.PrintableLines(dense)
            : new[] {$" {node.Label}"};

        private static IEnumerable<string> PrintableLines(this NonTerminalNode nonTerminal, bool dense)
        {
            yield return $" {nonTerminal.Label}";
            Stack<(NonTerminalNode node, int position)> stack = new Stack<(NonTerminalNode node, int position)>();
            stack.Push((nonTerminal, 0));

            string pipeBlank = "| " + (dense ? "" : "  ");
            string blank = "  " + (dense ? "" : "  ");
            string line = "+--" + (dense ? "" : "-");

            while (stack.Any())
            {
                (NonTerminalNode node, int position) = stack.Pop();
                if (position < node.Children.Length)
                {
                    string prefix = string.Join(
                        string.Empty,
                        stack.Reverse().Select(tuple => tuple.position < tuple.node.Children.Length ? pipeBlank : blank).ToArray());
                    
                    if (!dense)
                        yield return $" {prefix}|";

                    string nodeValue = node.Children[position] is TerminalNode terminal ? terminal.Value.Printable() : node.Children[position].Label;
                    yield return $" {prefix}{line} {nodeValue}";
                    
                    stack.Push((node, position + 1));
                    if (node.Children[position] is NonTerminalNode nonTerminalChild)
                        stack.Push((nonTerminalChild, 0));
                }
            }
        }

        private static string Printable(this string value) =>
            value.Replace("\n", "\\n").Replace("\r", "\\r");
    }
}
