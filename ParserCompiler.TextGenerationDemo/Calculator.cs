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
            label == "S" ? this.CompileSimple(children)
            : label == "A" ? this.CompileAddition(children)
            : "<Internal error>";

        private object CompileSimple(object[] children) =>
            children.Length == 1 ? children[0]
            : children[1];

        private object CompileAddition(object[] children) =>
            children.Length == 1 ? children[0]
            : children[0] is string leftError ? leftError
            : children[2] is string rightError ? rightError
            : children[1].Equals("+") ? this.Add((int)children[0], (int)children[2])
            : children[1].Equals("-") ? this.Subtract((int)children[0], (int)children[2])
            : "<Internal error>";

        private object Add(int a, int b) =>
            (long)a + b is long result && result >= int.MinValue && result <= int.MaxValue ? (object)(int)result
            : "<Overflow>";

        private object Subtract(int a, int b) =>
            (long)a - b is long result && result >= int.MinValue && result <= int.MaxValue ? (object)(int)result
            : "<Overflow>";
    }
}