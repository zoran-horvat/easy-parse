using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    public interface IRule : IProductionBuilder
    {
        NonTerminal Head { get; }
        IEnumerable<Production> Productions { get; }
        IEnumerable<Production> Expand();
    }
}