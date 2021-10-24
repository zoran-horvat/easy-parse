using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;

namespace EasyParse.CalculatorDemo
{
    class ReflectionArithmeticGrammar : ReflectionGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => new[] {new Regex(@"\s+")};

        [Start] [NonTerminal] protected int Number([Regex("number", @"\d+")]string number) => int.Parse(number);
    }
}
