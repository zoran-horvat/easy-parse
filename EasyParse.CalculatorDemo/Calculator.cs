using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    public class Calculator : ICompiler
    {
        public object CompileTerminal(string label, string value) =>
            label == "n" && int.TryParse(value, out int number) ? number
            : label == "n" ? (object)"<Overflow>"
            : value;

        public object CompileNonTerminal(string label, object[] children) =>
            label == "U" ? this.CompileUnit(children)
            : label == "A" ? this.CompileBinaryOperator(children)
            : label == "M" ? this.CompileBinaryOperator(children)
            : "<Internal error>";

        private object CompileUnit(object[] children) =>
            children.Length == 1 ? children[0]
            : children[1];

        private object CompileBinaryOperator(object[] children) =>
            children.Length == 1 ? children[0]
            : children[0] is string leftError ? leftError
            : children[2] is string rightError ? rightError
            : this.CompileBinaryOperator((int)children[0], (string)children[1], (int)children[2]);

        private object CompileBinaryOperator(long left, string operation, int right) =>
            operation == "+" ? this.ResultOf(left + right)
            : operation == "-" ? this.ResultOf(left - right)
            : operation == "*" && left == int.MinValue && right == int.MinValue ? "<Overflow>"
            : operation == "*" ? this.ResultOf(left * right)
            : operation == "/" && right == 0 ? "<Divide by zero>"
            : operation == "/" ? this.ResultOf(left / right)
            : "<Internal error>";

        private object ResultOf(long x) =>
            x >= int.MinValue && x <= int.MaxValue ? (object)(int)x
            : "<Overflow>";
    }
}