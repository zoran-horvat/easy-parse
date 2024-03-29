using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using EasyParse.Fluent;
using EasyParse.Native.Annotations;
using System.Text.RegularExpressions;

namespace EasyParse.Native
{
    internal class GrammarMethodsMap
    {
        public GrammarMethodsMap(IEnumerable<MethodInfo> methods)
        {
            List<MethodInfo> productions = ProductionRules(methods).ToList();

            this.ProductionMethods = productions;

            this.NonTerminalToType = productions
                .GroupBy(production => new NonTerminalName(production.Name), production => production)
                .ToDictionary(group => group.Key, group => group.SameReturnType());

            this.NonTerminals = productions.AsDistinctNonTerminals().ToHashSet();

            this.TypeToNonTerminals = productions
                .GroupBy(method => method.ReturnType, method => new NonTerminalName(method.Name))
                .ToDictionary(group => group.Key, group => group.ToList());

            (this.StartSymbol, this.StartSymbolType) = productions
                .WithAttribute<StartAttribute>()
                .AsSingleNonTerminal(Fail.MultipleStartSymbols<(NonTerminalName, Type)>);
        }

        public NonTerminalName StartSymbol { get; }

        public Type StartSymbolType { get; }

        public IEnumerable<MethodInfo> ProductionMethods { get; }
        private IDictionary<NonTerminalName, Type> NonTerminalToType { get; }
        private HashSet<NonTerminalName> NonTerminals { get; }
        private IDictionary<Type, List<NonTerminalName>> TypeToNonTerminals { get; }

        public Type TypeFor(NonTerminalName nonTerminal) =>
            this.NonTerminalToType.TryGetValue(nonTerminal, out Type type) ? type
            : nonTerminal.UnknownType<Type>();

        public SymbolAttribute Valid(ParameterInfo parameter, FromAttribute attribute) =>
            new FromAttribute(this.Valid(attribute.NonTerminals, parameter));

        private IEnumerable<NonTerminalName> Valid(IEnumerable<NonTerminalName> nonTerminals, ParameterInfo parameter) =>
            this.Assignable(this.Existing(nonTerminals, parameter), parameter);

        private IEnumerable<NonTerminalName> Assignable(IEnumerable<NonTerminalName> nonTerminals, ParameterInfo toParameter) =>
            nonTerminals.Select(nonTerminal => this.Assignable(nonTerminal, toParameter));

        public SymbolAttribute ToFromAttribute(ParameterInfo parameter) =>
            new FromAttribute(this.ToExistingNonTerminalName(parameter));

        private NonTerminalName ToExistingNonTerminalName(ParameterInfo parameter) =>
            this.GetCandidateNonTerminals(parameter)
                .Where(name => this.NonTerminals.Contains(name))
                .DefaultIfEmpty(() => Fail.NoAssignableNonTerminals<NonTerminalName>(parameter))
                .First();

        private IEnumerable<NonTerminalName> GetCandidateNonTerminals(ParameterInfo parameter) =>
            new[] { parameter.ToNonTerminalName(), parameter.ToNonTerminalNameWithNoTrailingDigits() };

        private NonTerminalName Assignable(NonTerminalName assign, ParameterInfo toParameter) =>
            this.NonTerminalToType[assign] == toParameter.ParameterType ? assign
            : toParameter.UnassignableNonTerminal<NonTerminalName>(assign, this.NonTerminalToType[assign]);

        public IEnumerable<NonTerminalName> Existing(IEnumerable<NonTerminalName> nonTerminals, ParameterInfo assignedTo) =>
            nonTerminals.Select(nonTerminal => this.Existing(nonTerminal, assignedTo));

        private NonTerminalName Existing(NonTerminalName nonTerminal, ParameterInfo assignedTo) =>
            this.NonTerminals.Contains(nonTerminal) ? nonTerminal 
            : assignedTo.NonExistentTerminal<NonTerminalName>(nonTerminal);

        private static IEnumerable<MethodInfo> ProductionRules(IEnumerable<MethodInfo> methods) =>
            methods.Where(method => method.IsPublic || method.HasAttribute<StartAttribute>() || method.HasAttribute<NonTerminalAttribute>());
    }
}