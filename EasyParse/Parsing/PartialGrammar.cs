using System;
using System.Runtime.CompilerServices;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    public abstract class PartialGrammar
    {
        protected IEmptyRule Rule([CallerMemberName] string nonTerminalName = "") =>
            new EmptyRule(new NonTerminalName(nonTerminalName));

        protected Symbol Symbol(Func<IRule> nonTerminal) => 
            new RecursiveNonTerminalSymbol(nonTerminal);
    }
}
