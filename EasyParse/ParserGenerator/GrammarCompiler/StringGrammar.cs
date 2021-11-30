using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class StringGrammar : NativeGrammar
    {
        protected override IEnumerable<Regex> IgnorePatterns => Enumerable.Empty<Regex>();

        public string NewLine([R("newLine", @"\\n")] string _) => "\n";
        public string CarriageReturn([R("carriageReturn", @"\\r")] string _) => "\r";
        public string Tab([R("tab", @"\\t")] string _) => "\t";
        public string Backslash([R("backslash", @"\\\\")] string _) => "\\";
        public string Quote([R("quote", @"\\'")] string _) => "'";
        public string Plaintext([R("plaintext", @"([^\\]+)|(\\.)")] string value) => value;

        [Start] public string String(string segment) => segment;
        public string String(string @string, string segment) => @string + segment;

        public string Segment([From(nameof(Plaintext), nameof(NewLine), nameof(CarriageReturn), nameof(Tab), nameof(Backslash), nameof(Quote))] string value) => value;
    }
}
