using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace EasyParse.CalculatorDemo
{
    class CorrectiveArithmeticGrammar : ReflectionGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns =>
            new[] {new Regex(@"\s")};

        public string Number([R("number", @"\d+")] string value) => 
            value;

        public string Add([R("add", @"[+\-]")] string @operator) =>
            @operator;

        public string Multiply([R("multiply", @"[*/]")] string @operator) => 
            @operator;

        public string Unit(string number) => 
            number.ToNumber();

        public string Unit(string add, string unit) =>
            add == "-" ? unit.Invert() : unit;

        public string Unit([L("(")] string open, string additive, [L(")")] string close) =>
            additive;

        public string Multiplicative(string unit) => unit;

        public string Multiplicative(string multiplicative, string multiply, string unit) =>
            multiply == "*" ? multiplicative.Multiply(unit) : multiplicative.Divide(unit);

        public string Additive(string multiplicative) => multiplicative;
            
        public string Additive(string additive, string add, string multiplicative) =>
            add == "+" ? additive.Add(multiplicative) : additive.Subtract(multiplicative);

        [Start] public string Expression(string additive) => additive;
    }
}
