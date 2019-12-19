using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EasyParse.LexicalAnalysis
{
    class Pattern
    {
        private Regex Expression { get; }
        private IEnumerable<string> LexemeLabel { get; }
     
        public Pattern(string expression) : this(expression, new string[0])
        {
        }

        public Pattern(string expression, string lexemeLabel) : this(expression, new[] {lexemeLabel})
        {
        }

        private Pattern(string expression, string[] lexemeLabel)
        {
            this.Expression = new Regex(expression);
            this.LexemeLabel = lexemeLabel;
        }
    }
}
