using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyParse.Text
{
    internal class Text : Plaintext
    {
        public Text(IEnumerable<string> lines) 
            : base(lines.Aggregate(new StringBuilder(), (text, line) => text.Append($"{line}\n")).ToString())
        {
        }

        public override Location Beginning => new TextLocation(0, 0, 0);

        protected override Location LocationFor(int offset) =>
            offset >= this.Content.Length ? EndOfText.Value
                : new LineLocation(offset);
    }
}