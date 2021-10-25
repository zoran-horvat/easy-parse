using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace EasyParse.CalculatorDemo
{
    class ReflectionArithmeticGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => 
            new[] {new Regex(@"\s+")};

        public int Unit([R("number", @"\d+")] string number) => int.Parse(number);
        public int Unit([L("-")] string minus, int unit) => -unit;
        public int Unit([L("(")] string open, int additive, [L(")")] string close) => additive;

        public int Additive(int multiplicative) => multiplicative;
        public int Additive(int additive, [L("+", "-")] string op, int multiplicative) =>
            op == "+" ? additive + multiplicative : additive - multiplicative;

        public int Multiplicative(int unit) => unit;
        public int Multiplicative(int multiplicative, [L("*", "/")] string op, int unit) =>
            op == "*" ? multiplicative * unit : multiplicative / unit;

        [Start] public int Expression(int additive) => additive;
    }
}
