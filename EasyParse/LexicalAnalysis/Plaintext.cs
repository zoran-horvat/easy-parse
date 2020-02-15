using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyParse.LexicalAnalysis
{
    public class Plaintext
    {
        public string Content { get; }
        public int Length => this.Content.Length;

        public Plaintext(string content)
        {
            this.Content = content;
        }

        public Plaintext(IEnumerable<string> lines)
            : this(lines.Aggregate(new StringBuilder(), (text, line) => text.Append($"{line}\n")).ToString())
        {
        }

        public Location LocationFor(int contentOffset) =>
            new LineLocation(contentOffset);
    }
}
