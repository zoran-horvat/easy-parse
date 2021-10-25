using System;

namespace EasyParse.Native.Annotations
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class NonTerminalAttribute : Attribute
    {
    }
}