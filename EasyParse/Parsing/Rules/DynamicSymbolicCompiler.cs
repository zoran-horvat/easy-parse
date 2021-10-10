using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Parsing.Nodes.Errors;
using EasyParse.Parsing.Rules.Symbols;
using EasyParse.Text;

namespace EasyParse.Parsing.Rules
{
    class DynamicSymbolicCompiler : ISymbolCompiler
    {
        public DynamicSymbolicCompiler(IEnumerable<Production> productions)
        {
            this.Productions = WithValidTypes(productions.ToList());
            this.TerminalTransforms =
                RegexSymbols(this.Productions).ToDictionary(symbol => symbol.Name, symbol => symbol.Transform);
        }

        private IEnumerable<Production> Productions { get; }
        private Dictionary<string, Func<string, object>> TerminalTransforms { get; }

        private static List<Production> WithValidTypes(List<Production> productions)
        {
            Dictionary<NonTerminal, Type> nonTerminalTypes = NonTerminalTypes(productions);

            productions.SelectMany(production => UnassignableTypes(production, nonTerminalTypes))
                .ToList()
                .ForEach(tuple => throw new ArgumentException(
                    $"Cannot pass {tuple.argument.Name} as {tuple.transformParameter.Name} in rule {tuple.production}"));

            return productions;
        }

        private static Dictionary<NonTerminal, Type> NonTerminalTypes(IEnumerable<Production> productions) =>
            productions.Select(production => (symbol: production.Head, type: production.ReturnType))
                .Distinct()
                .ToDictionary(tuple => tuple.symbol, tuple => tuple.type);

        private static IEnumerable<(Production production, Type argument, Type transformParameter)> UnassignableTypes(
            Production production, Dictionary<NonTerminal, Type> nonTerminalTypes) =>
            ComponentTypes(production, nonTerminalTypes)
                .Zip(production.Transform.ArgumentTypes, (value, transform) => (production, value, transform))
                .Where(tuple => !tuple.transform.IsAssignableFrom(tuple.value));

        private static IEnumerable<Type> ComponentTypes(
            Production production, Dictionary<NonTerminal, Type> nonTerminalTypes) =>
            production.Body.Select(symbol => TypeOf(symbol, nonTerminalTypes));

        private static Type TypeOf(Symbol symbol, Dictionary<NonTerminal, Type> nonTerminalTypes) =>
            symbol is NonTerminalSymbol nonTerminal ? nonTerminalTypes[nonTerminal.Head] 
            : symbol.Type;

        private static IEnumerable<RegexSymbol> RegexSymbols(IEnumerable<Production> productions) =>
            productions
                .SelectMany(production => production.Body)
                .OfType<RegexSymbol>()
                .GroupBy(symbol => symbol.Name)
                .Select(group => group.First());

        public object CompileTerminal(string label, string value) => 
            this.TerminalTransforms.TryGetValue(label, out Func<string, object> transform) ? transform(value) 
            : (object)value;

        public object CompileNonTerminal(Location location, string label, object[] children) =>
            this.TransformsFor(label, this.TypesOf(children))
                .DefaultIfEmpty(() => this.CompileErrorTransform(location, label, children))
                .Select(transform => transform(children))
                .First();

        private IEnumerable<Func<object[], object>> TransformsFor(string label, Type[] children) =>
            this.Productions
                .Where(production => production.Head.Name == label)
                .Where(production => production.Transform.IsApplicableTo(children))
                .Select(production => production.Transform.Function);

        private Func<object[], object> CompileErrorTransform(Location location, string label, object[] children) =>
            children.OfType<CompileError>()
                .Select<CompileError, Func<object[], object>>(error => (object[] _) => (object)error)
                .DefaultIfEmpty(() => (object[] arguments) => (object)new CompileError(location, label, arguments))
                .First();
                
                

        private Type[] TypesOf(object[] values) =>
            values.Select(value => value?.GetType() ?? typeof(object)).ToArray();
    }
}
