using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace EasyParse.CalculatorDemo
{
    class CorrectiveArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns =>
            new[] {new Regex(@"\s")};

        public string Unit([R("number", @"\d+")] string number) => number.ToNumber();
        public string Unit([L("-")] string minus, string unit) => unit.Invert();
        public string Unit([L("(")] string open, string additive, [L(")")] string close) => additive;

        public string Multiplicative(string unit) => unit;
        public string Multiplicative(string multiplicative, [L("*", "/")] string op, string unit) =>
            op == "*" ? multiplicative.Multiply(unit) : multiplicative.Divide(unit);

        public string Additive(string multiplicative) => multiplicative;
        public string Additive(string additive, [L("+", "-")] string op, string multiplicative) =>
            op == "+" ? additive.Add(multiplicative) : additive.Subtract(multiplicative);

        [Start] public string Expression(string additive) => additive;
    }
}
