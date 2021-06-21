using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.Parsing.Nodes.Errors;
using EasyParse.Text;

namespace EasyParse.Parsing
{
    public abstract class MethodMapCompiler : ICompiler
    {
        private IEnumerable<(string terminal, Func<string, object> map)> TerminalMap { get; }

        protected MethodMapCompiler()
        {
            this.TerminalMap = this.FindTerminalMethods().ToList();
        }

        public object CompileTerminal(string label, string value) =>
            this.TerminalMap
                .Where(tuple => tuple.terminal.Equals(label, StringComparison.OrdinalIgnoreCase))
                .Select(tuple => tuple.map(value))
                .DefaultIfEmpty(value)
                .First();

        public object CompileNonTerminal(Location location, string label, object[] children)
        {
            try 
            {
                object result = this.Compile(location, label, children);
                return result is Exception ex ? this.ToResult(location, ex) : result;
            }
            catch (Exception ex)
            {
                return this.ToResult(location, ex);
            }
        }

        private object Compile(Location location, string methodName, params object[] children)
        {
            try
            {
                return 
                    children.OfType<CompileError>().FirstOrDefault() is CompileError error ? error
                    : children.OfType<Exception>().FirstOrDefault() is Exception ex ? this.ToResult(location, ex)
                    : this.FindMethods(methodName, children).FirstOrDefault() is MethodInfo method ? method.Invoke(this, children)
                    : this.Fail(location, methodName, children);
            }
            catch (Exception ex)
            {
                return this.ToResult(location, ex);
            }
        }

        private object ToResult(Location location, Exception ex)
        {
            string message = ex is TargetInvocationException invocation ? invocation.InnerException.Message
                : ex.Message;
            return new CompileError(location, message);
        }

        private IEnumerable<MethodInfo> FindMethods(string name, object[] arguments) =>
            this.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.DeclaringType?.IsSubclassOf(typeof(MethodMapCompiler)) ?? false)
                .Where(method => string.Equals(name, method.Name, StringComparison.InvariantCultureIgnoreCase))
                .Where(method => !method.ContainsGenericParameters)
                .Where(method => method.DeclaringType?.IsSubclassOf(typeof(MethodMapCompiler)) ?? false)
                .Where(method => this.CanBind(method, arguments));

        private IEnumerable<(string label, Func<string, object> method)> FindTerminalMethods() =>
            this.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.DeclaringType?.IsSubclassOf(typeof(MethodMapCompiler)) ?? false)
                .Where(method => method.GetParameters() is ParameterInfo[] parameters && parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                .Where(method => typeof(object).IsAssignableFrom(method.ReturnType))
                .Where(method => method.Name.StartsWith("Terminal", StringComparison.OrdinalIgnoreCase))
                .Where(method => method.Name.Length > "Terminal".Length)
                .Select(method => (
                    method.Name.Substring("Terminal".Length), 
                    (Func<string, object>) (value => method.Invoke(this, new object[] {value}))));

        private bool CanBind(MethodInfo method, object[] arguments) =>
            this.CanBind(method.GetParameters(), arguments);

        private bool CanBind(ParameterInfo[] argumentDefinitions, object[] arguments) =>
            argumentDefinitions.Length == arguments.Length &&
            argumentDefinitions.Zip(arguments, (definition, argument) => (definition: definition, value: argument))
                .All(pair =>
                    pair.value?.GetType() is Type argumentType && pair.definition.ParameterType.IsAssignableFrom(argumentType)
                    || pair.value is null && (pair.definition.ParameterType.IsClass || pair.definition.ParameterType.IsInterface));

        private object Fail(Location location, string label, IEnumerable<object> arguments) =>
            new CompileError(location, label, arguments);

        private string Join(IEnumerable<object> arguments) =>
            string.Join(" ", arguments.Select(arg => $"[{this.ToString(arg)}]").ToArray());

        private string ToString(object argument) =>
            argument is null ? "<null>"
            : $"{argument.GetType().Name} {argument}";
    }
}