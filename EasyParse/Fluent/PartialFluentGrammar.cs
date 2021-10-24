using System;
using System.Runtime.CompilerServices;
using EasyParse.Fluent.Rules;
using EasyParse.Fluent.Symbols;

namespace EasyParse.Fluent
{
    public abstract class PartialFluentGrammar
    {
        protected IEmptyRule Rule([CallerMemberName] string nonTerminalName = "") =>
            new EmptyRule(new NonTerminalName(nonTerminalName));

        protected Symbol Symbol(Func<IRule> nonTerminal) =>
            new RecursiveNonTerminalSymbol(nonTerminal);
    }
}
