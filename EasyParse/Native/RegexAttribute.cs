using System.Text.RegularExpressions;

namespace EasyParse.Native
{
    public class RegexAttribute : SymbolAttribute
    {
        public RegexAttribute(string name, string expression)
        {
            this.Name = name;
            this.Expression = new Regex(expression);
        }

        public Regex Expression { get; }
        public string Name {get;}
    }
}