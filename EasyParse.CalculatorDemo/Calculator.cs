using System;
using System.Collections.Generic;
using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    public class Calculator : MethodMapCompiler
    {
        protected override IEnumerable<(string terminal, string methodName)> TerminalMap => new[]
        {
            ("n", nameof(Integer)),
        };

        private object Integer(string value) =>
            int.TryParse(value, out int result) ? (object)result
            : new OverflowException();

        private object M(int value) => value;

        private object M(int left, string mulOrDiv, int right) =>
            mulOrDiv == "*" && left == int.MinValue && right == int.MinValue ? new OverflowException()
            : mulOrDiv == "/" && right == 0 ? new DivideByZeroException()
            : mulOrDiv == "*" ? this.ResultOf(left * right)
            : this.ResultOf(left / right);

        private object A(int value) => value;

        private object A(int left, string plusOrMinus, int right) =>
            plusOrMinus == "+" ? this.ResultOf(left + right)
            : this.ResultOf(left - right);

        private int U(int n) => n;

        private int U(string openParen, int value, string closedParen) => value;

        private object ResultOf(long x) =>
            x >= int.MinValue && x <= int.MaxValue ? (object)(int)x
            : new OverflowException();
    }
}