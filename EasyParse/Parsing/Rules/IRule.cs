using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    public interface IRule<T> : IProductionBuilder<T>
    {
        NonTerminal Head { get; }
        IEnumerable<Production> Productions { get; }
        IEnumerable<Production> Expand();
    }
}