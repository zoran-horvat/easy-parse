using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    class AdditiveCompiler : MethodMapCompiler
    {
        // Corresponds to grammar line: number matches @'\d+';
        private int TerminalNumber(string value) => int.Parse(value);

        // Corresponds to grammar line: Expr -> number;
        public int Expr(int number) => number;

        // Corresponds to grammar lines:
        // Expr -> Expr '+' number;
        // Expr -> Expr @'-' number;
        public int Expr(int a, string op, int b) =>
            op == "+" ? a + b : a - b;

    }
}
