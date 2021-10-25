using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.Fluent;

namespace EasyParse.Native
{
    internal static class ReflectionExtensions
    {
        internal static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute =>
            method.CustomAttributes.Any(attribute => typeof(TAttribute).IsAssignableFrom(attribute.AttributeType));

        internal static Type SameReturnType(this IEnumerable<MethodInfo> methods) =>
            methods.Select(method => method.ReturnType)
                .Distinct()
                .Select((type, index) => index == 0 ? type : methods.First().DifferentOverloadReturnTypes<Type>())
                .First();

        internal static IEnumerable<NonTerminalName> AsDistinctNonTerminals(this IEnumerable<MethodInfo> methods) =>
            methods.Select(method => new NonTerminalName(method.Name)).Distinct();

        internal static IEnumerable<T> AssertSingle<T>(this IEnumerable<T> sequence, Func<T> failOnSecond) =>
            sequence
                .Select((item, index) => index == 0 ? item : failOnSecond());

        internal static (NonTerminalName name, Type type) AsSingleNonTerminal(this IEnumerable<MethodInfo> methods, Func<(NonTerminalName, Type)> failOnSecond) =>
            methods
                .GroupBy(method => new NonTerminalName(method.Name), method => method.ReturnType)
                .Select(group => (name: group.Key, type: group.First()))
                .AssertSingle(failOnSecond)
                .First();

        internal static IEnumerable<MethodInfo> WithAttribute<TAttribute>(this IEnumerable<MethodInfo> methods) where TAttribute : Attribute =>
            methods.Where(method => method.CustomAttributes.Any(attribute => typeof(TAttribute).IsAssignableFrom(attribute.AttributeType)));

        internal static string PrintableCapitalised(this ParameterInfo parameter) =>
            parameter.Printable("Parameter");

        internal static string Printable(this ParameterInfo parameter) =>
            parameter.Printable("parameter");

        private static string Printable(this ParameterInfo parameter, string parameterWord) =>
            $"{parameterWord} '{parameter.ParameterType.Name} {parameter.Name}' of method '{parameter.Member.Name}'";
    }
}