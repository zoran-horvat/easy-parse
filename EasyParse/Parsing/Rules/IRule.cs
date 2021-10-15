using System;
using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    public interface IRule : IProductionBuilder
    {
        NonTerminalName Head { get; }
        Type Type { get; }
        IEnumerable<Production> Productions { get; }
        IEnumerable<Production> Expand();
    }
}