using System;
using EasyParse.Parsing;

namespace ParserCompiler.TextGenerationDemo
{
    class Calculator : ICompiler
    {
        public object CompileTerminal(string label, string value) =>
            label == "n" && int.TryParse(value, out int number) ? number
            : label == "n" ? (object)"<Overflow>"
            : value;

        public object CompileNonTerminal(string label, object[] children) =>
            label == "A" ? this.CompileAddition(children)
            : label == "E" ? children[0]
            : "<Internal error>";

        private object CompileAddition(object[] children) =>
            children.Length == 1 ? children[0]
            : children[0] is string leftError ? leftError
            : children[2] is string rightError ? rightError
            : children[1].Equals("+") ? this.Add((int)children[0], (int)children[2])
            : children[1].Equals("-") ? this.Subtract((int)children[0], (int)children[2])
            : "<Internal error>";


        private object Add(int a, int b) =>
            a < 0 && b > 0 ? a + b
            : a > 0 && b < 0 ? a + b
            : a < 0 && b < 0 && int.MinValue - a <= b ? a + b
            : int.MaxValue - a >= b ? (object)(a + b)
            : "<Overflow>";

        private object Subtract(int a, int b) =>
            a > 0 && b > 0 ? a - b
            : a < 0 && b < 0 ? a - b
            : a < 0 && b > 0 && int.MinValue + b <= a ? a - b
            : int.MinValue - a <= b ? (object) (a - b)
            : "<Overflow>";
    }
}