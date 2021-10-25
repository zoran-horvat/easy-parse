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

        public string Number(
            [Regex("number", @"\d+")] string value) => 
            value;

        public string Add(
            [Regex("add", @"[+\-]")] string @operator) =>
            @operator;

        public string Multiply(
            [Regex("multiply", @"[*/]")] string @operator) => 
            @operator;

        public string Unit(
            [From(nameof(Number))] string value) => 
            value.ToNumber();

        public string Unit(
            [From(nameof(Add))] string @operator, 
            [From(nameof(Unit))] string value) =>
            @operator == "-" ? value.Invert() : value;

        public string Unit(
            [Literal("(")] string open, 
            [From(nameof(Additive))] string value, 
            [Literal(")")] string close) =>
            value;

        public string Multiplicative(
            [From(nameof(Unit))] string value) => 
            value;

        public string Multiplicative(
            [From(nameof(Multiplicative))] string a, 
            [From(nameof(Multiply))] string @operator, 
            [From(nameof(Unit))] string b) =>
            @operator == "*" ? a.Multiply(b) : a.Divide(b);

        public string Additive(
            [From(nameof(Multiplicative))] string value) => 
            value;
            
        public string Additive(
            [From(nameof(Additive))] string left, 
            [From(nameof(Add))] string @operator, 
            [From(nameof(Multiplicative))] string right) =>
            @operator == "+" ? left.Add(right) : left.Subtract(right);

        [Start] public string Expression(
            [From(nameof(Additive))] string value) => 
            value;
    }
}
