using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyParse
{
    public class GrammarLoader
    {
        public IEnumerable<string> From(string filePath) =>
            File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("#"));
    }
}
