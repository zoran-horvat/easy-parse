using EasyParse.Parsing;

namespace EasyParse.WordAnalysisDemo
{
    class LongestWordsSelector : MethodMapCompiler
    {
        private string TerminalWord(string value) => value;

        private string Text(string lineLongest) => lineLongest;

        private string Text(string textLongest, string lineLongest) =>
            $"{textLongest}, {lineLongest}";

        private string Line(string endOfLine) => string.Empty;

        private string Line(string longest, string endOfLine) => longest;

        private string Words(string word) => word;

        private string Words(string longest, string next) =>
            next.Length > longest.Length ? next : longest;
    }
}
