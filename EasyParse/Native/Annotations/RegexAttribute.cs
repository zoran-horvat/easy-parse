using System.Text.RegularExpressions;

namespace EasyParse.Native.Annotations
{
    public class RegexAttribute : SymbolAttribute
    {
        public RegexAttribute(string name, string expression)
        {
            Name = name;
            Expression = new Regex(expression);
        }

        public Regex Expression { get; }
        public string Name { get; }
    }
}