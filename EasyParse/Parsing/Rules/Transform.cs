using System;
using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    public class Transform
    {
        public Transform(Type returnType, IEnumerable<Type> argumentTypes, Func<object[], object> function)
        {
            this.ReturnType = returnType;
            this.ArgumentTypes = argumentTypes;
            this.Function = function;
        }

        public Type ReturnType { get; }
        public IEnumerable<Type> ArgumentTypes { get; }
        public Func<object[], object> Function { get; }

        public Transform WithReturnType(Type type) =>
            type == this.ReturnType ? this
            : new Transform(type, this.ArgumentTypes, this.Function);
    }
}
