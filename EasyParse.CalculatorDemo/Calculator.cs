using System;
using System.Collections.Generic;
using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    public class Calculator : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, Func<string, object> map)> TerminalMap => new (string, Func<string, object>)[]
        {
            ("number", value => int.TryParse(value, out int result) ? (object)result : new OverflowException()),
        };

        private object A(string openParen, int value, string closedParen) => value;
        private object A(int value) => value;
        private object B(int value) => value;
        private object B(int a, string op, int b) => a + b;
        private object Multiplicative(int value) => value;

        private object Multiplicative(int left, string mulOrDiv, int right) =>
            mulOrDiv == "*" && left == int.MinValue && right == int.MinValue ? new OverflowException()
            : mulOrDiv == "/" && right == 0 ? new DivideByZeroException()
            : this.ResultOf(mulOrDiv == "*" ? left * right : left / right);

        private object Additive(int value) => value;

        private object Additive(int left, string plusOrMinus, int right) =>
            this.ResultOf(plusOrMinus == "+" ? left + right : left - right);

        private int Unit(int value) => value;
        private int Unit(string openParen, int value, string closedParen) => value;

        private object ResultOf(long x) =>
            x >= int.MinValue && x <= int.MaxValue ? (object)(int)x : new OverflowException();
    }
}