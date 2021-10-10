using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public class Transform
    {
        public Transform(Type returnType, Type[] argumentTypes, Func<object[], object> function)
            : this(returnType, argumentTypes.Length, (IEnumerable<Type>)argumentTypes, function)
        {
        }

        private Transform(
            Type returnType, int argumentsCount, 
            IEnumerable<Type> argumentTypes, Func<object[], object> function)
        {
            this.ReturnType = returnType;
            this.ArgumentsCount = argumentsCount;
            this.ArgumentTypes = argumentTypes;
            this.Function = function;
        }

        public Type ReturnType { get; }
        public int ArgumentsCount { get; }
        public IEnumerable<Type> ArgumentTypes { get; }
        public Func<object[], object> Function { get; }

        public Transform WithReturnType(Type type) =>
            type == this.ReturnType ? this
            : new Transform(type, this.ArgumentsCount, this.ArgumentTypes, this.Function);

        public bool IsApplicableTo(Type[] arguments) =>
            arguments.Length == this.ArgumentsCount &&
            arguments.Zip(this.ArgumentTypes, (value, type) => (value, type))
                .All(tuple => tuple.type.IsAssignableFrom(tuple.value));
    }
}
