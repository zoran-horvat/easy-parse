using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    class FullAdditiveParenthesizer : MethodMapSymbolCompiler
    {
        private string TerminalNumber(string value) => value;

        private string Expr(string value) => value;

        private string Expr(string a, string op, string b) => $"({a} {op} {b})";
    }
}
