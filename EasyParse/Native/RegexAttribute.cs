using System;
using System.Text.RegularExpressions;

namespace EasyParse.Native
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class RegexAttribute : Attribute
    {
        public RegexAttribute(string expression)
        {
            this.Expression = new Regex(expression);
        }

        public Regex Expression { get; }
    }
}