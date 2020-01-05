using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyParse.Parsing
{
    public abstract class MethodMapCompiler : ICompiler
    {
        protected abstract IEnumerable<(string terminal, Func<string, object> map)> TerminalMap { get; }

        public object CompileTerminal(string label, string value) =>
            this.TerminalMap
                .Where(tuple => tuple.terminal == label)
                .Select(tuple => tuple.map(value))
                .DefaultIfEmpty(value)
                .First();

        public object CompileNonTerminal(string label, object[] children) => 
            this.Compile(label, children);

        private object Compile(string methodName, params object[] children) =>
            children.OfType<Exception>().FirstOrDefault() is Exception exc ? exc
            : this.FindMethods(methodName, children).FirstOrDefault() is MethodInfo method ? method.Invoke(this, children)
            : this.Fail(methodName, children);

        private IEnumerable<MethodInfo> FindMethods(string name, object[] arguments) =>
            this.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => string.Equals(name, method.Name, StringComparison.InvariantCultureIgnoreCase))
                .Where(method => !method.ContainsGenericParameters)
                .Where(method => method.DeclaringType?.IsSubclassOf(typeof(MethodMapCompiler)) ?? false)
                .Where(method => this.CanBind(method, arguments));

        private bool CanBind(MethodInfo method, object[] arguments) =>
            this.CanBind(method.GetParameters(), arguments);

        private bool CanBind(ParameterInfo[] argumentDefinitions, object[] arguments) =>
            argumentDefinitions.Length == arguments.Length &&
            argumentDefinitions.Zip(arguments, (definition, argument) => (definition: definition, value: argument))
                .All(pair =>
                    pair.value?.GetType() is Type argumentType && pair.definition.ParameterType.IsAssignableFrom(argumentType)
                    || pair.value is null && (pair.definition.ParameterType.IsClass || pair.definition.ParameterType.IsInterface));

        private object Fail(string label, IEnumerable<object> arguments) =>
            new Exception($"Cannot map {label} -> {this.Join(arguments)}");

        private string Join(IEnumerable<object> arguments) =>
            string.Join(" ", arguments.Select(arg => $"[{this.ToString(arg)}]").ToArray());

        private string ToString(object argument) =>
            argument is null ? "<null>"
            : $"{argument.GetType().Name} {argument}";
    }
}