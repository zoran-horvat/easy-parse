using System;
using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    public class Calculator : MethodMapCompiler
    {
        private object TerminalNumber(string value) =>
            int.TryParse(value, out int result) ? (object)result : new OverflowException();

        private object Expression(object result) => result;
        private object Mul(int value) => value;

        private object Mul(int left, string mulOrDiv, int right) =>
            mulOrDiv == "*" && left == int.MinValue && right == int.MinValue ? new OverflowException()
            : mulOrDiv == "/" && right == 0 ? new DivideByZeroException()
            : this.ResultOf(mulOrDiv == "*" ? left * right : left / right);

        private object Add(int value) => value;

        private object Add(int left, string plusOrMinus, int right) =>
            this.ResultOf(plusOrMinus == "+" ? left + right : left - right);

        private int Unit(int value) => value;
        private int Unit(string openParen, int value, string closedParen) => value;

        private object ResultOf(long x) =>
            x >= int.MinValue && x <= int.MaxValue ? (object)(int)x : new OverflowException();
    }
}