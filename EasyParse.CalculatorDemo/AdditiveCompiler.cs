using System;
using System.Collections.Generic;
using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    class AdditiveCompiler : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap =>
            new (string, Func<string, object>)[]
            {
                // Corresponds to grammar line: number matches @'\d+';
                ("number", n => int.Parse(n)),
            };

        // Corresponds to grammar line: Expr -> number;
        public int Expr(int number) => number;

        // Corresponds to grammar lines:
        // Expr -> Expr '+' number;
        // Expr -> Expr @'-' number;
        public int Expr(int a, string op, int b) =>
            op == "+" ? a + b : a - b;

    }
}
