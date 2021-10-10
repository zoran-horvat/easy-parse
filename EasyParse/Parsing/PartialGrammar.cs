using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    public abstract class PartialGrammar
    {
        protected RegexSymbol WhiteSpace() =>
            RegexSymbol.Create("white space", new Regex(@"\s+"), x => x);

        protected IEmptyRule Rule([CallerMemberName] string nonTerminalName = "") =>
            new EmptyRule(new NonTerminal(nonTerminalName));

    }
}
