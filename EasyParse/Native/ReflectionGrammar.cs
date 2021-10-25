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
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Native.Annotations;

namespace EasyParse.Native
{
    public abstract class ReflectionGrammar
    {
        protected abstract IEnumerable<Regex> IgnorePatterns { get; }

        private IEnumerable<RegexSymbol> IgnoreSymbols =>
            this.IgnorePatterns.Select(pattern => new RegexSymbol(pattern.ToString(), pattern, typeof(string), x => x));

        public Parser BuildParser() =>
            FluentGrammar.BuildParser(this.IgnoreSymbols, this.StartSymbol, this.GetProductions());

        public IEnumerable<string> ToGrammarFileContent() =>
            new GrammarToGrammarFileFormatter().Convert(this.IgnoreSymbols, this.StartSymbol, this.GetProductions());

        public Compiler<T> BuildCompiler<T>() =>
            this.BuildCompiler<T>(this.StartSymbolMethod.ReturnType);

        public Compiler<T> BuildCompiler<T>(Type startSymbolType) =>
            typeof(T).IsAssignableFrom(startSymbolType) ? this.BuildParser().ToCompiler<T>(this.Compiler)
            : throw new InvalidOperationException(
                $"Cannot create compiler for type {typeof(T).Name} from grammar which produces type {startSymbolType.Name}");

        private ISymbolCompiler Compiler => 
            new DynamicSymbolicCompiler(this.GetProductions());

        private IEnumerable<Production> GetProductions() =>
            this.GetProductions(this.GetNonTerminals(), this.GetTypesToNonTerminals(), this.GetNonTerminalsTypes());

        private IEnumerable<Production> GetProductions(
            HashSet<NonTerminalName> nonTerminals, 
            IDictionary<Type, List<NonTerminalName>> typeToNonTerminals,
            IDictionary<NonTerminalName, Type> nonTerminalToType) =>
            this.SelectMethods<NonTerminalAttribute>()
                .SelectMany(method => this.ToProductions(method, nonTerminals, typeToNonTerminals, nonTerminalToType))
                .Select((production, offset) => production.WithReference(RuleReference.CreateOrdinal(offset + 1)));

        private IDictionary<NonTerminalName, Type> GetNonTerminalsTypes() =>
            this.SelectMethods<NonTerminalAttribute>()
                .Select(method => (name: new NonTerminalName(method.Name), type: method.ReturnType))
                .GroupBy(pair => pair.name)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(pair => pair.type).Aggregate((a, b) => this.CommonType(group.Key, a, b)));

        private Type CommonType(NonTerminalName nonTerminal, Type a, Type b) =>
            a.IsAssignableFrom(b) ? a
            : b.IsAssignableFrom(a) ? b
            : throw new InvalidOperationException(
                $"Nonterminal symbol {nonTerminal.Name} is declared with types {a.Name} and {b.Name} which cannot be assigned to one another");

        private IDictionary<Type, List<NonTerminalName>> GetTypesToNonTerminals() =>
            this.SelectMethods<NonTerminalAttribute>()
                .GroupBy(method => method.ReturnType)
                .ToDictionary(group => group.Key, group => group.Select(this.ToNonTerminal).ToList());

        private HashSet<NonTerminalName> GetNonTerminals() =>
            this.SelectMethods<NonTerminalAttribute>().Select(this.ToNonTerminal).ToHashSet();

        private NonTerminalName ToNonTerminal(MethodInfo method) =>
            new NonTerminalName(method.Name);

        private NonTerminalName StartSymbol =>
            new NonTerminalName(this.StartSymbolMethod.Name);

        private MethodInfo StartSymbolMethod =>
            this.SelectMethods<StartAttribute>()
                .Select(this.AsNonTerminal)
                .DefaultIfEmpty<MethodInfo>(() => throw new InvalidOperationException("Start symbol not defined on grammar"))
                .First();

        private IEnumerable<Production> ToProductions(
            MethodInfo method, 
            HashSet<NonTerminalName> nonTerminals, 
            IDictionary<Type, List<NonTerminalName>> typeToNonTerminals,
            IDictionary<NonTerminalName, Type> nonTerminalToType)
        {
            List<ImmutableList<Symbol>> bodies = new List<ImmutableList<Symbol>>() { ImmutableList<Symbol>.Empty };

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bodies = this
                    .ToSymbols(nonTerminals, typeToNonTerminals, nonTerminalToType, parameter)
                    .SelectMany(symbol => bodies.Select(body => body.Add(symbol)))
                    .ToList();
            }

            return bodies.Select(body => this.ToProduction(method, body));
        }

        private IEnumerable<Symbol> ToSymbols(
            HashSet<NonTerminalName> nonTerminals, 
            IDictionary<Type, List<NonTerminalName>> typeToNonTerminals, 
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            ParameterInfo parameter) =>
            this.ToSymbols(
                nonTerminalToType, parameter,
                this.GetSymbolAttribute(nonTerminals, typeToNonTerminals, nonTerminalToType, parameter, parameter.GetCustomAttributes<SymbolAttribute>().ToArray()));

        private IEnumerable<Symbol> ToSymbols(
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            ParameterInfo parameter, SymbolAttribute attribute) =>
            attribute is RegexAttribute regex ? new Symbol[] { RegexSymbol.Create<string>(regex.Name, regex.Expression, x => x) }
            : attribute is LiteralAttribute literal ? new Symbol[] { new LiteralSymbol(literal.Value) }
            : attribute is FromAttribute @from ? this.ToNonTerminalSymbols(nonTerminalToType, @from.NonTerminals)
            : throw new InvalidOperationException($"Unsupported parameter attribute {attribute.GetType().Name}");

        private IEnumerable<Symbol> ToNonTerminalSymbols(
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            IEnumerable<NonTerminalName> nonTerminals) =>
            this.ToRules(nonTerminalToType, nonTerminals).Select(rule => new NonTerminalSymbol(rule));

        private IEnumerable<IRule> ToRules(
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            IEnumerable<NonTerminalName> nonTerminals) =>
            nonTerminals.Select(nonTerminal => new RulePlaceholder(nonTerminal, nonTerminalToType[nonTerminal]));

        private SymbolAttribute GetSymbolAttribute(
            HashSet<NonTerminalName> nonTerminals,
            IDictionary<Type, List<NonTerminalName>> typeToNonTerminals, 
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            ParameterInfo parameter, SymbolAttribute[] attributes) =>
            attributes.Length == 0 ? this.ToFromAttribute(nonTerminals, typeToNonTerminals, nonTerminalToType, parameter)
            : attributes.Length == 1 && attributes[0] is FromAttribute @from ? this.Valid(nonTerminals, nonTerminalToType, parameter, @from)
            : attributes.Length == 1 ? attributes[0]
            : throw new InvalidOperationException($"Parameter {parameter.Name} of method {parameter.Member.Name} has multiple symbol attributes defined");

        private SymbolAttribute ToFromAttribute(
            HashSet<NonTerminalName> nonTerminals, 
            IDictionary<Type, List<NonTerminalName>> typeToNonTerminals, 
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            ParameterInfo parameter) =>
            new FromAttribute(this.Valid(nonTerminals, nonTerminalToType, this.NonTerminalsFor(typeToNonTerminals, parameter.ParameterType), parameter));

        private SymbolAttribute Valid(
            HashSet<NonTerminalName> validNonTerminals,
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            ParameterInfo parameter, FromAttribute attribute) =>
            new FromAttribute(this.Valid(validNonTerminals, nonTerminalToType, attribute.NonTerminals, parameter));

        private IEnumerable<NonTerminalName> Valid(
            HashSet<NonTerminalName> validNonterminals,
            IDictionary<NonTerminalName, Type> nonTerminalToType,
            IEnumerable<NonTerminalName> nonTerminals, ParameterInfo parameter) =>
            nonTerminals
                .Select(nonTerminal => 
                    validNonterminals.Contains(nonTerminal) ? nonTerminal
                    : throw new InvalidOperationException(
                        $"Parameter {parameter.Name} of method {parameter.Member.Name} is referencing a nonexistent nonterminal symbol {nonTerminal.Name}"))
                .Select(nonTerminal => nonTerminalToType[nonTerminal].IsAssignableFrom(parameter.ParameterType) ? nonTerminal
                    : throw new InvalidOperationException(
                        $"Parameter {parameter.Name} of method {parameter.Member.Name} is of type {parameter.ParameterType.Name} which cannot be assigned " +
                        $"to nonterminal symbol declared as {nonTerminalToType[nonTerminal].Name}"));

        private IEnumerable<NonTerminalName> NonTerminalsFor(IDictionary<Type, List<NonTerminalName>> typeToNonTerminals, Type parameterType) =>
            typeToNonTerminals.TryGetValue(parameterType, out List<NonTerminalName> nonTerminals) ? nonTerminals
            : typeToNonTerminals.Where(pair => parameterType.IsAssignableFrom(pair.Key)).SelectMany(pair => pair.Value);

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