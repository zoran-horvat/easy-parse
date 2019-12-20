using System.Collections.Generic;
using System.IO;

namespace ParserCompiler.TextGenerationDemo
{
    static class TextReaderExtensions
    {
        public static IEnumerable<string> ReadLinesUntil(this TextReader reader, string end)
        {
            while (reader.ReadLine() is string line && !string.IsNullOrEmpty(line) && line != end)
            {
                yield return line;
            }
        }
    }
}
