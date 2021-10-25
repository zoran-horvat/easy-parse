using System;
using System.Reflection;
using EasyParse.Fluent;

namespace EasyParse.Native
{
    internal static class Fail
    {
        internal static T NonExistentTerminal<T>(this ParameterInfo referencedBy, NonTerminalName nonTerminal) =>
            Throw<T>($"{referencedBy.PrintableCapitalised()} is referencing a nonexistent nonterminal symbol {nonTerminal.Name}");

        internal static T UnassignableNonTerminal<T>(this ParameterInfo assignTo, NonTerminalName nonTerminal, Type type) =>
            Throw<T>($"Cannot assign nonterminal {type.Name} {nonTerminal.Name} to {assignTo.Printable()}");

        internal static T NoAssignableNonTerminals<T>(this ParameterInfo assignTo) =>
            Throw<T>($"No nonterminal symbols found assignable to {assignTo.Printable()}");

        internal static T UnknownType<T>(this NonTerminalName nonTerminal) => 
            Throw<T>($"Could not determine type for nonterminal symbol {nonTerminal}");

        internal static T DifferentOverloadReturnTypes<T>(this MethodInfo method) =>
            Throw<T>($"Overloads of method {method.Name} must all return the same type");

        internal static T MultipleStartSymbols<T>() =>
            Throw<T>("Grammar is defining multiple start symbols");

        internal static T CompilerGrammarTypesMismatch<T>(Type compilerType, Type startSymbolType) =>
            Throw<T>($"Cannot create compiler for type {compilerType.Name} from grammar which produces type {startSymbolType.Name}");

        public static T Throw<T>(string message) =>
            throw new InvalidOperationException(message);
    }
}