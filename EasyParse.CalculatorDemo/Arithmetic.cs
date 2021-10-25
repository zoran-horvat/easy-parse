using System;

namespace EasyParse.CalculatorDemo
{
    static class Arithmetic
    {
        private const string Overflow = "Integer overflow";
        private const string DivisionByZero = "Division by zero";

        public static string ToNumber(this string value) =>
            int.TryParse(value, out int number) ? number.ToString("0") : Overflow;

        public static string Invert(this string value) =>
            long.TryParse(value, out long number) ? (-number).ToNumber() : Overflow;

        public static string Add(this string a, string b) =>
            Binary(a, b, (a, b) => (a + b).ToNumber());

        private static string Add(long a, long b) => (a + b).ToNumber();

        private static string ToNumber(this long x) =>
            x >= int.MinValue && x <= int.MaxValue ? x.ToString("0") : Overflow;

        public static string Subtract(this string a, string b) => 
            Binary(a, b, (a, b) => (a - b).ToNumber());

        public static string Multiply(this string a, string b) =>
            Binary(a, b, (a, b) => a > int.MinValue || b > int.MinValue ? (a * b).ToNumber() : Overflow);

        public static string Divide(this string a, string b) =>
            Binary(a, b, (a, b) => a != 0 && b != 0 ? (a / b).ToNumber() : DivisionByZero);

        private static string Binary(string a, string b, Func<long, long, string> operation) =>
            int.TryParse(a, out int left) ? Binary(left, b, operation) : a;

        private static string Binary(int a, string b, Func<long, long, string> operation) =>
            int.TryParse(b, out int right) ? operation(a, right) : b;
    }
}
