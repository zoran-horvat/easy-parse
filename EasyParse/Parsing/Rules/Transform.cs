using System;
using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    public class Transform
    {
        public Type ReturnType { get; }
        public IEnumerable<Type> ArgumentTypes { get; }
        public Func<object[], object> Function { get; }

        public Transform(Type returnType, IEnumerable<Type> argumentTypes, Func<object[], object> function)
        {
            this.ReturnType = returnType;
            this.ArgumentTypes = argumentTypes;
            this.Function = function;
        }
    }
}
