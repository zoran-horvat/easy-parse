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
            label == "U" ? this.CompileUnit(children)
            : label == "A" ? this.CompileAddition(children)
            : label == "M" ? this.CompileMultiplicative(children)
            : "<Internal error>";

        private object CompileUnit(object[] children) =>
            children.Length == 1 ? children[0]
            : children[1];

        private object CompileAddition(object[] children) =>
            children.Length == 1 ? children[0]
            : children[0] is string leftError ? leftError
            : children[2] is string rightError ? rightError
            : children[1].Equals("+") ? this.Add((int)children[0], (int)children[2])
            : children[1].Equals("-") ? this.Subtract((int)children[0], (int)children[2])
            : "<Internal error>";

        private object CompileMultiplicative(object[] children) =>
            children.Length == 1 ? children[0]
            : children[0] is string leftError ? leftError
            : children[2] is string rightError ? rightError
            : children[1].Equals("*") ? this.Multiply((int) children[0], (int) children[2])
            : children[1].Equals("/") ? this.Divide((int)children[0], (int)children[2])
            : "<Internal error>";

        private object Add(int a, int b) =>
            (long)a + b is long result && result >= int.MinValue && result <= int.MaxValue ? (object)(int)result
            : "<Overflow>";

        private object Subtract(int a, int b) =>
            (long)a - b is long result && result >= int.MinValue && result <= int.MaxValue ? (object)(int)result
            : "<Overflow>";

        private object Multiply(int a, int b) =>
            a == int.MinValue && b == int.MinValue ? "<Overflow>"
            : (long)a * b is long result && result >= int.MinValue && result <= int.MaxValue ? (object) (int) result
            : "<Overflow>";

        private object Divide(int a, int b) =>
            b == 0 ? "<Divide by zero>"
            : (long) a / b is long result && result >= int.MinValue && result <= int.MaxValue ? (object) (int) result
            : "<Overflow>";
    }
}