using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyParse.Text
{
    internal class Text : Plaintext
    {
        private int[] LineOffset { get; }

        public Text(IEnumerable<string> lines) : this(lines.ToArray())
        {
        }

        private Text(string[] lines)
            : base(lines.Aggregate(new StringBuilder(), (text, line) => text.Append($"{line}\n")).ToString())
        {
            this.LineOffset = new int[lines.Length];
            for (int i = 1; i < lines.Length; i += 1)
                this.LineOffset[i] = this.LineOffset[i - 1] + lines[i - 1].Length + 1;
        }

        public override Location Beginning => new TextLocation(0, 0, 0);

        protected override Location LocationFor(int offset) =>
            offset >= this.Content.Length ? EndOfText.Value
            : this.OffsetToLocation(offset, this.OffsetToLineIndex(offset));

        private Location OffsetToLocation(int offset, int lineIndex) =>
            new TextLocation(offset, lineIndex, offset - this.LineOffset[lineIndex]);

        private int OffsetToLineIndex(int offset)
        {
            int lower = 0;
            int upper = this.LineOffset.Length;

            while (lower < upper - 1)
            {
                int middle = (lower + upper) / 2;
                if (this.LineOffset[middle] > offset)
                    upper = middle;
                else
                    lower = middle;
            }

            return lower;
        }
    }
}