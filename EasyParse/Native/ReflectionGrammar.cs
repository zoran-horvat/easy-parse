using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyParse.Fluent;
using EasyParse.Parsing;
using System;
using EasyParse.Fluent.Symbols;
using EasyParse.Fluent.Rules;
using System.Collections.Immutable;

namespace EasyParse.Native
{
    public abstract class ReflectionGrammar
    {
        protected abstract IEnumerable<Regex> IgnorePatterns { get; }

        private IEnumerable<RegexSymbol> IgnoreSymbols =>
            this.IgnorePatterns.Select(pattern => new RegexSymbol(pattern.ToString(), pattern, typeof(string), x => x));

        public Parser BuildParser() => null;

        public IEnumerable<string> ToGrammarFileContent() =>
            new GrammarToGrammarFileFormatter().Convert(this.IgnoreSymbols, this.StartSymbol, this.GetProductions());

        private IEnumerable<Production> GetProductions() =>
            this.GetProductions(this.GetNonTerminals(), this.GetNonTerminalTypes());

        private IEnumerable<Production> GetProductions(HashSet<NonTerminalName> nonTerminals, IDictionary<Type, List<NonTerminalName>> nonTerminalTypes) =>
            this.SelectMethods<NonTerminalAttribute>().SelectMany(method => this.ToProductions(method, nonTerminals, nonTerminalTypes));

        private IDictionary<Type, List<NonTerminalName>> GetNonTerminalTypes() =>
            this.SelectMethods<NonTerminalAttribute>()
                .GroupBy(method => method.ReturnType)
                .ToDictionary(group => group.Key, group => group.Select(this.ToNonTerminal).ToList());

        private HashSet<NonTerminalName> GetNonTerminals() =>
            this.SelectMethods<NonTerminalAttribute>().Select(this.ToNonTerminal).ToHashSet();

        private NonTerminalName ToNonTerminal(MethodInfo method) =>
            new NonTerminalName(method.Name);

        private NonTerminalName StartSymbol =>
            this.SelectMethods<StartAttribute>()
                .Select(this.AsNonTerminal)
                .DefaultIfEmpty<MethodInfo>(() => throw new InvalidOperationException("Start symbol not defined on grammar"))
                .Select(method => new NonTerminalName(method.Name))
                .First();

        private IEnumerable<Production> ToProductions(MethodInfo method, HashSet<NonTerminalName> nonTerminals, IDictionary<Type, List<NonTerminalName>> nonTerminalTypes)
        {
            List<ImmutableList<Symbol>> bodies = new List<ImmutableList<Symbol>>() { ImmutableList<Symbol>.Empty };

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bodies = this
                    .ToSymbols(nonTerminals, nonTerminalTypes, parameter)
                    .SelectMany(symbol => bodies.Select(body => body.Add(symbol)))
                    .ToList();
            }

            return bodies.Select(body => this.ToProduction(method, body));
        }

        private IEnumerable<Symbol> ToSymbols(HashSet<NonTerminalName> nonTerminals, IDictionary<Type, List<NonTerminalName>> nonTerminalTypes, ParameterInfo parameter) =>
            this.ToSymbols(parameter, this.GetSymbolAttribute(nonTerminals, nonTerminalTypes, parameter, parameter.GetCustomAttributes<SymbolAttribute>().ToArray()));

        private IEnumerable<Symbol> ToSymbols(ParameterInfo parameter, SymbolAttribute attribute) =>
            attribute is RegexAttribute regex ? new Symbol[] { RegexSymbol.Create<string>(regex.Name, regex.Expression, x => x) }
            : attribute is LiteralAttribute literal ? new Symbol[] { new LiteralSymbol(literal.Value) }
            : attribute is FromAttribute @from ? throw new InvalidOperationException("Non-terminal symbols are not supported yet")
            : throw new InvalidOperationException($"Unsupported parameter attribute {attribute.GetType().Name}");

        private SymbolAttribute GetSymbolAttribute(
            HashSet<NonTerminalName> nonTerminals, IDictionary<Type, List<NonTerminalName>> nonTerminalTypes, 
            ParameterInfo parameter, SymbolAttribute[] attributes) =>
            attributes.Length == 0 ? this.ToFromAttribute(nonTerminals, nonTerminalTypes, parameter)
            : attributes.Length == 1 && attributes[0] is FromAttribute @from ? this.Valid(nonTerminals, parameter, @from)
            : attributes.Length == 1 ? attributes[0]
            : throw new InvalidOperationException($"Parameter {parameter.Name} of method {parameter.Member.Name} has multiple symbol attributes defined");

        private SymbolAttribute ToFromAttribute(HashSet<NonTerminalName> nonTerminals, IDictionary<Type, List<NonTerminalName>> nonTerminalTypes, ParameterInfo parameter) =>
            new FromAttribute(this.Valid(nonTerminals, this.NonTerminalsFor(nonTerminalTypes, parameter.ParameterType), parameter));

        private SymbolAttribute Valid(HashSet<NonTerminalName> validNonTerminals, ParameterInfo parameter, FromAttribute attribute) =>
            new FromAttribute(this.Valid(validNonTerminals, attribute.NonTerminals, parameter));

        private IEnumerable<NonTerminalName> Valid(HashSet<NonTerminalName> validNonterminals, IEnumerable<NonTerminalName> nonTerminals, ParameterInfo parameter) =>
            nonTerminals
                .Where(nonTerminal => !validNonterminals.Contains(nonTerminal))
                .Select<NonTerminalName, IEnumerable<NonTerminalName>>(nonTerminal => throw new InvalidOperationException(
                    $"Parameter {parameter.Name} of method {parameter.Member.Name} is referencing a nonexistent nonterminal symbol {nonTerminal.Name}"))
                .DefaultIfEmpty(nonTerminals)
                .First();

        private IEnumerable<NonTerminalName> NonTerminalsFor(IDictionary<Type, List<NonTerminalName>> nonTerminalTypes, Type parameterType) =>
            nonTerminalTypes.TryGetValue(parameterType, out List<NonTerminalName> nonTerminals) ? nonTerminals
            : nonTerminalTypes.Where(pair => parameterType.IsAssignableFrom(pair.Key)).SelectMany(pair => pair.Value);

        private Production ToProduction(MethodInfo method, ImmutableList<Symbol> body) =>
            new Production(ToNonTerminal(method), body, this.ToTransform(method));

        private Transform ToTransform(MethodInfo method) =>
            new Transform(method.ReturnType, this.ParameterTypes(method).ToArray(), this.ToTransformFunction(method));

        private IEnumerable<Type> ParameterTypes(MethodInfo method) =>
            method.GetParameters().Select(parameter => parameter.ParameterType);

        private Func<object[], object> ToTransformFunction(MethodInfo method) =>
            (object[] arguments) => method.Invoke(this, arguments);
        private MethodInfo AsNonTerminal(MethodInfo method) =>
            method.CustomAttributes.Any(attribute => typeof(NonTerminalAttribute).IsAssignableFrom(attribute.AttributeType)) ? method
            : throw new ArgumentException($"Method {method.Name} must be decorated with NonTerminalAttribute");

        private IEnumerable<MethodInfo> SelectMethods<TAttribute>() where TAttribute : Attribute => 
            this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.CustomAttributes.Any(attribute => typeof(TAttribute).IsAssignableFrom(attribute.AttributeType)));
    }
}