using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;

namespace EasyParse.CalculatorDemo
{
    class ReflectionArithmeticGrammar : ReflectionGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => new[] {new Regex(@"\s+")};

        [NonTerminal] protected int Number([Regex("number", @"\d+")] string number) => int.Parse(number);

        [NonTerminal] protected int Unit([From(nameof(Number))] int number) => number;
        [NonTerminal] protected int Unit([Literal("-")] string minus, [From(nameof(Number))] int number) => -number;
        [NonTerminal] protected int Unit([Literal("(")] string open, [From(nameof(Additive))] int value, [Literal(")")] string close) => value;

        [NonTerminal] protected int Additive([From(nameof(Multiplicative), nameof(Added), nameof(Subtracted))] int value) => value;
        [NonTerminal] protected int Added([From(nameof(Additive))] int a, [Literal("+")] string plus, [From(nameof(Multiplicative))] int b) => a + b;
        [NonTerminal] protected int Subtracted([From(nameof(Additive))] int a, [Literal("-")] string minus, [From(nameof(Multiplicative))] int b) => a - b;

        [NonTerminal] protected int Multiplicative([From(nameof(Unit), nameof(Multiplied), nameof(Divided))] int value) => value;
        [NonTerminal] protected int Multiplied([From(nameof(Multiplicative))] int a, [Literal("*")] string star, [From(nameof(Unit))] int b) => a * b;
        [NonTerminal] protected int Divided([From(nameof(Multiplicative))] int a, [Literal("/")] string slash, [From(nameof(Unit))] int b) => a / b;

        [Start] [NonTerminal] protected int Expression([From(nameof(Additive))] int value) => value;
    }
}
