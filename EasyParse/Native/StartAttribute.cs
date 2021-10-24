using System;

namespace EasyParse.Native
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class StartAttribute : Attribute
    {
    }
}