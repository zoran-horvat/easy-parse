using System;
using System.Runtime.CompilerServices;
using EasyParse.Fluent;
using EasyParse.Fluent.Rules;
using EasyParse.Fluent.Symbols;

namespace EasyParse
{
    public abstract class PartialFluentGrammar
    {
        protected IEmptyRule Rule([CallerMemberName] string nonTerminalName = "") =>
            new EmptyRule(new NonTerminalName(nonTerminalName));

        protected Symbol Symbol(Func<IRule> nonTerminal) =>
            new RecursiveNonTerminalSymbol(nonTerminal);
    }
}
