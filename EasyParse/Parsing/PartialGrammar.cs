using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    public abstract class PartialGrammar
    {
        protected RegexSymbol WhiteSpace() =>
            new("white space", new Regex(@"\s+"));

        protected IEmptyRule<T> Rule<T>([CallerMemberName] string nonTerminalName = "") =>
            new EmptyRule<T>(new NonTerminal(nonTerminalName));

    }
}
